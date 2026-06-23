using System.Collections.Generic;

namespace Facility_Management_CEI.Services
{
    public class IfcProcessingResult
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
        public List<string> GeneratedIfcFiles { get; set; } = new List<string>();
        public List<string> GeneratedWexBimPaths { get; set; } = new List<string>();
    }
}
