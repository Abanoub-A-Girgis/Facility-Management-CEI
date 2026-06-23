using System;

namespace Facility_Management_CEI.Configuration
{
    public class IfcProcessingOptions
    {
        public string UploadDirectory { get; set; } = "data";
        public long MaxUploadBytes { get; set; } = 2147483648;
        public string[] AllowedExtensions { get; set; } = new[] { ".ifc" };

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
    }
}
