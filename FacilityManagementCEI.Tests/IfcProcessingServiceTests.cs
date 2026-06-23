using Facility_Management_CEI.Configuration;
using Facility_Management_CEI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FacilityManagementCEI.Tests
{
    public class IfcProcessingServiceTests
    {
        [Fact]
        public async Task SaveSplitAndConvertAsync_RejectsMissingFile()
        {
            var service = CreateService();

            var ex = await Assert.ThrowsAsync<InvalidDataException>(() => service.SaveSplitAndConvertAsync(null));

            Assert.Contains("No IFC file", ex.Message);
        }

        [Fact]
        public async Task SaveSplitAndConvertAsync_RejectsNonIfcExtension()
        {
            var service = CreateService();
            var file = CreateFormFile("model.txt", new byte[] { 1, 2, 3 });

            var ex = await Assert.ThrowsAsync<InvalidDataException>(() => service.SaveSplitAndConvertAsync(file));

            Assert.Contains("Only IFC files", ex.Message);
        }

        [Fact]
        public async Task SaveSplitAndConvertAsync_RejectsUnexpectedContentType()
        {
            var service = CreateService();
            var file = CreateFormFile("model.ifc", IfcHeaderBytes(), "image/png");

            var ex = await Assert.ThrowsAsync<InvalidDataException>(() => service.SaveSplitAndConvertAsync(file));

            Assert.Contains("content type", ex.Message);
        }

        [Fact]
        public async Task SaveSplitAndConvertAsync_RejectsInvalidIfcHeader()
        {
            var service = CreateService();
            var file = CreateFormFile("model.ifc", new byte[] { 1, 2, 3 }, "application/octet-stream");

            var ex = await Assert.ThrowsAsync<InvalidDataException>(() => service.SaveSplitAndConvertAsync(file));

            Assert.Contains("valid IFC STEP", ex.Message);
        }

        [Fact]
        public async Task SaveSplitAndConvertAsync_RejectsUploadDirectoryOutsideWebRoot()
        {
            var service = CreateService(uploadDirectory: "..\\outside");
            var file = CreateFormFile("model.ifc", IfcHeaderBytes(), "application/octet-stream");

            var ex = await Assert.ThrowsAsync<InvalidDataException>(() => service.SaveSplitAndConvertAsync(file));

            Assert.Contains("upload directory", ex.Message);
        }

        [Fact]
        public async Task SaveSplitAndConvertAsync_RejectsFileAboveConfiguredLimit()
        {
            var service = CreateService(maxUploadBytes: 2);
            var file = CreateFormFile("model.ifc", new byte[] { 1, 2, 3 });

            var ex = await Assert.ThrowsAsync<InvalidDataException>(() => service.SaveSplitAndConvertAsync(file));

            Assert.Contains("exceeds", ex.Message);
        }

        [Fact]
        public void NormalizeSeverityColors_ClampsConfiguredColorsAndKeepsAllSeverities()
        {
            var colors = ViewerHighlightMapper.NormalizeSeverityColors(new ViewerOptions
            {
                SeverityColors = new Dictionary<string, int[]>
                {
                    ["Severe"] = new[] { 300, -1, 12 },
                    ["High"] = new[] { 1, 2, 3, 4 },
                    ["Medium"] = new[] { 5, 6, 7, 8 },
                    ["Low"] = new[] { 9, 10, 11, 12 }
                }
            });

            Assert.Equal(new[] { 255, 0, 12, 255 }, colors["Severe"]);
            Assert.Equal(new[] { 1, 2, 3, 4 }, colors["High"]);
            Assert.Equal(new[] { 5, 6, 7, 8 }, colors["Medium"]);
            Assert.Equal(new[] { 9, 10, 11, 12 }, colors["Low"]);
        }

        private static IfcProcessingService CreateService(
            long maxUploadBytes = 1024,
            string uploadDirectory = "data",
            bool validateIfcHeader = true)
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), "FacilityManagementCEI.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            var environment = new TestWebHostEnvironment
            {
                ContentRootPath = tempRoot,
                WebRootPath = Path.Combine(tempRoot, "wwwroot")
            };
            Directory.CreateDirectory(environment.WebRootPath);

            var options = Options.Create(new IfcProcessingOptions
            {
                UploadDirectory = uploadDirectory,
                MaxUploadBytes = maxUploadBytes,
                AllowedExtensions = new[] { ".ifc" },
                ValidateIfcHeader = validateIfcHeader
            });

            return new IfcProcessingService(environment, options, new TestStringLocalizer(), NullLogger<IfcProcessingService>.Instance);
        }

        private static IFormFile CreateFormFile(string fileName, byte[] content)
        {
            return CreateFormFile(fileName, content, "application/octet-stream");
        }

        private static IFormFile CreateFormFile(string fileName, byte[] content, string contentType)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        private static byte[] IfcHeaderBytes()
        {
            return System.Text.Encoding.ASCII.GetBytes("ISO-10303-21;\nHEADER;\nENDSEC;\nDATA;\nENDSEC;\nEND-ISO-10303-21;");
        }

        private class TestWebHostEnvironment : IWebHostEnvironment
        {
            public string EnvironmentName { get; set; }
            public string ApplicationName { get; set; }
            public string WebRootPath { get; set; }
            public IFileProvider WebRootFileProvider { get; set; }
            public string ContentRootPath { get; set; }
            public IFileProvider ContentRootFileProvider { get; set; }
        }

        private class TestStringLocalizer : IStringLocalizer<Facility_Management_CEI.SharedResource>
        {
            private readonly Dictionary<string, string> _values = new Dictionary<string, string>
            {
                ["Server.NoIfcFile"] = "No IFC file was provided.",
                ["Server.EmptyIfcFile"] = "The uploaded IFC file is empty.",
                ["Server.FileTooLarge"] = "The uploaded IFC file exceeds the configured limit.",
                ["Server.OnlyIfc"] = "Only IFC files are supported.",
                ["Server.InvalidContentType"] = "The uploaded file content type is not allowed for IFC uploads.",
                ["Server.InvalidIfcContent"] = "The uploaded file does not look like a valid IFC STEP file.",
                ["Server.InvalidUploadDirectory"] = "The configured IFC upload directory must stay inside the web root so generated viewer files can be served safely.",
                ["Server.InvalidFileName"] = "The uploaded file name is invalid.",
                ["Server.IfcProcessingFailed"] = "The IFC file could not be parsed, split, or converted for viewer use."
            };

            public LocalizedString this[string name] => new LocalizedString(name, _values.TryGetValue(name, out var value) ? value : name, !_values.ContainsKey(name));

            public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, string.Format(this[name].Value, arguments), this[name].ResourceNotFound);

            public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            {
                return _values.Select(item => new LocalizedString(item.Key, item.Value));
            }

            public IStringLocalizer WithCulture(CultureInfo culture)
            {
                return this;
            }
        }
    }
}
