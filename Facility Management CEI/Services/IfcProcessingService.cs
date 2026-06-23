using Facility_Management_CEI.Configuration;
using IfcToolbox.Core.Analyse;
using IfcToolbox.Core.Editors;
using IfcToolbox.Core.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.ModelGeometry.Scene;

namespace Facility_Management_CEI.Services
{
    public class IfcProcessingService : IIfcProcessingService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IfcProcessingOptions _options;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<IfcProcessingService> _logger;

        public IfcProcessingService(
            IWebHostEnvironment environment,
            IOptions<IfcProcessingOptions> options,
            IStringLocalizer<SharedResource> localizer,
            ILogger<IfcProcessingService> logger)
        {
            _environment = environment;
            _options = options.Value;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IfcProcessingResult> SaveSplitAndConvertAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            ValidateUpload(file);

            var uploadRoot = GetUploadRoot();
            Directory.CreateDirectory(uploadRoot);

            var fileName = CreateSafeFileName(file.FileName, uploadRoot);
            var fullPath = Path.Combine(uploadRoot, fileName);

            await using (var fileStream = File.Create(fullPath))
            {
                await file.CopyToAsync(fileStream, cancellationToken);
            }

            _logger.LogInformation("Saved IFC upload {FileName} to {Path}", fileName, fullPath);
            ValidateIfcHeader(fullPath);

            try
            {
                // XBIM processing is synchronous and CPU/file-system heavy. This keeps the request pipeline
                // responsive enough for now; a durable background job is still recommended for large models.
                return await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var splitResult = SplitByBuildingStorey(fullPath, true);
                    foreach (var ifcFile in splitResult.GeneratedIfcFiles)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ConvertToWexBim(ifcFile);
                    }

                    return new IfcProcessingResult
                    {
                        FileName = fileName,
                        FullPath = fullPath,
                        RelativePath = ToWebPath(fullPath),
                        GeneratedIfcFiles = splitResult.GeneratedIfcFiles,
                        GeneratedWexBimPaths = splitResult.GeneratedWexBimPaths
                    };
                }, cancellationToken);
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("IFC processing was cancelled for {FileName}", fileName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IFC parsing, splitting, or WexBIM conversion failed for {FileName}", fileName);
                throw new InvalidDataException(_localizer["Server.IfcProcessingFailed"], ex);
            }
        }

        private void ValidateUpload(IFormFile file)
        {
            if (file == null)
            {
                throw new InvalidDataException(_localizer["Server.NoIfcFile"]);
            }

            if (file.Length <= 0)
            {
                throw new InvalidDataException(_localizer["Server.EmptyIfcFile"]);
            }

            if (file.Length > _options.MaxUploadBytes)
            {
                throw new InvalidDataException($"{_localizer["Server.FileTooLarge"]} {_options.MaxUploadBytes} bytes.");
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !_options.IsAllowedExtension(extension))
            {
                throw new InvalidDataException(_localizer["Server.OnlyIfc"]);
            }

            if (!_options.IsAllowedContentType(file.ContentType))
            {
                throw new InvalidDataException(_localizer["Server.InvalidContentType"]);
            }
        }

        private string GetUploadRoot()
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var fullWebRoot = Path.GetFullPath(webRoot);
            var uploadDirectory = _options.UploadDirectory ?? "data";
            var uploadRoot = Path.IsPathRooted(uploadDirectory)
                ? Path.GetFullPath(uploadDirectory)
                : Path.GetFullPath(Path.Combine(fullWebRoot, uploadDirectory));

            if (!IsSubPathOf(uploadRoot, fullWebRoot))
            {
                throw new InvalidDataException(_localizer["Server.InvalidUploadDirectory"]);
            }

            return uploadRoot;
        }

        private string CreateSafeFileName(string fileName, string uploadRoot)
        {
            var safeName = Path.GetFileName(fileName);
            var baseName = Path.GetFileNameWithoutExtension(safeName);
            var extension = Path.GetExtension(safeName);

            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = "model";
            }

            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                baseName = baseName.Replace(invalidChar, '_');
            }

            var candidate = baseName + extension;
            var candidatePath = Path.GetFullPath(Path.Combine(uploadRoot, candidate));
            if (!IsSubPathOf(candidatePath, uploadRoot))
            {
                throw new InvalidDataException(_localizer["Server.InvalidFileName"]);
            }

            if (!File.Exists(candidatePath))
            {
                return candidate;
            }

            return $"{baseName}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";
        }

        private (List<string> GeneratedIfcFiles, List<string> GeneratedWexBimPaths) SplitByBuildingStorey(
            string filePath,
            bool keepLabel,
            string placeholder = "Level",
            string parentPlaceholder = "Building")
        {
            var newPaths = new List<string>();
            var wexBimPaths = new List<string>();

            using (var model = IfcStore.Open(filePath))
            {
                var buildings = model.Instances.OfType<IIfcBuilding>().ToList();
                for (int j = 0; j < buildings.Count; j++)
                {
                    var buildingStoreys = buildings[j].BuildingStoreys.ToList();
                    for (int i = 0; i < buildingStoreys.Count; i++)
                    {
                        string suffix = ConsoleFile.CreateIndexedSuffix(
                            buildingStoreys[i].Name,
                            buildingStoreys.Count > 1,
                            i,
                            placeholder,
                            true,
                            buildings[j].Name,
                            buildings.Count > 1,
                            j,
                            parentPlaceholder);

                        var generatedFilePath = ConsoleFile.AddSuffixToName(filePath, suffix);
                        var requiredProducts = ProductAnalyse.PrepareRequiredProducts(model, buildingStoreys[i]);
                        InsertCopy.CopyProducts(model, generatedFilePath, requiredProducts, keepLabel);

                        newPaths.Add(generatedFilePath);
                        wexBimPaths.Add(ToWebPath(Path.ChangeExtension(generatedFilePath, "wexBIM")));
                    }
                }
            }

            return (newPaths, wexBimPaths);
        }

        private void ConvertToWexBim(string filePath)
        {
            using (var model = IfcStore.Open(filePath))
            {
                var context = new Xbim3DModelContext(model);
                context.CreateContext();

                var wexBimFullPath = Path.ChangeExtension(filePath, "wexBIM");
                using (var wexBimFile = File.Create(wexBimFullPath))
                using (var wexBimBinaryWriter = new BinaryWriter(wexBimFile))
                {
                    model.SaveAsWexBim(wexBimBinaryWriter);
                }
            }
        }

        private void ValidateIfcHeader(string fullPath)
        {
            if (!_options.ValidateIfcHeader)
            {
                return;
            }

            var bytesToRead = Math.Max(1024, _options.HeaderValidationBytes);
            var buffer = new byte[bytesToRead];
            int read;

            using (var stream = File.OpenRead(fullPath))
            {
                read = stream.Read(buffer, 0, buffer.Length);
            }

            var header = Encoding.ASCII.GetString(buffer, 0, read);
            if (header.IndexOf("ISO-10303-21", StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidDataException(_localizer["Server.InvalidIfcContent"]);
            }
        }

        private string ToWebPath(string fullPath)
        {
            var webRoot = Path.GetFullPath(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"));
            var relativePath = Path.GetRelativePath(webRoot, fullPath);
            return "/" + relativePath.Replace("\\", "/");
        }

        private static bool IsSubPathOf(string path, string root)
        {
            var fullPath = Path.GetFullPath(path)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            var fullRoot = Path.GetFullPath(root)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            return fullPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase);
        }
    }
}
