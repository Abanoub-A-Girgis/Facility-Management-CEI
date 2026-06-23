using System;

namespace Facility_Management_CEI.Configuration
{
    public class IfcProcessingOptions
    {
        public string UploadDirectory { get; set; } = "data";
        public long MaxUploadBytes { get; set; } = 2147483648;
        public string[] AllowedExtensions { get; set; } = new[] { ".ifc" };
        public string[] AllowedContentTypes { get; set; } = new[]
        {
            "application/octet-stream",
            "application/ifc",
            "application/x-step",
            "text/plain"
        };
        public bool ValidateIfcHeader { get; set; } = true;
        public int HeaderValidationBytes { get; set; } = 8192;

        public bool IsAllowedExtension(string extension)
        {
            foreach (var allowedExtension in AllowedExtensions)
            {
                if (string.Equals(extension, allowedExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsAllowedContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return true;
            }

            foreach (var allowedContentType in AllowedContentTypes)
            {
                if (string.Equals(contentType, allowedContentType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
