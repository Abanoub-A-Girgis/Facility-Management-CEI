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
        public async Task SaveSplitAndConvertAsync_RejectsFileAboveConfiguredLimit()
        {
            var service = CreateService(maxUploadBytes: 2);
            var file = CreateFormFile("model.ifc", new byte[] { 1, 2, 3 });

            var ex = await Assert.ThrowsAsync<InvalidDataException>(() => service.SaveSplitAndConvertAsync(file));

            Assert.Contains("exceeds", ex.Message);
        }

        private static IfcProcessingService CreateService(long maxUploadBytes = 1024)
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
                UploadDirectory = "data",
                MaxUploadBytes = maxUploadBytes,
                AllowedExtensions = new[] { ".ifc" }
            });

            return new IfcProcessingService(environment, options, new TestStringLocalizer(), NullLogger<IfcProcessingService>.Instance);
        }

        private static IFormFile CreateFormFile(string fileName, byte[] content)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "file", fileName);
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
                ["Server.OnlyIfc"] = "Only IFC files are supported."
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
