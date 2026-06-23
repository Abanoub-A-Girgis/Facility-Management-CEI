using System;
using System.Collections.Generic;

namespace Facility_Management_CEI.ApiLayer.Dtos
{
    public class BuildingModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int FloorCount { get; set; }
        public int ElementCount { get; set; }
        public int OpenIssueCount { get; set; }
    }

    public class ModelUploadResponseDto
    {
        public int? ModelId { get; set; }
        public string FileName { get; set; }
        public string RelativePath { get; set; }
        public IReadOnlyList<string> GeneratedIfcFiles { get; set; }
        public IReadOnlyList<string> GeneratedWexBimPaths { get; set; }
    }

    public class FloorDto
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public string Name { get; set; }
        public string WexBimPath { get; set; }
    }

    public class ElementDto
    {
        public int Id { get; set; }
        public int ViewerId { get; set; }
        public string ElementKey { get; set; }
        public string ElementType { get; set; }
        public string Name { get; set; }
        public string Materials { get; set; }
        public int FloorId { get; set; }
        public string FloorName { get; set; }
        public int? SpaceId { get; set; }
        public string SpaceName { get; set; }
        public int OpenIssueCount { get; set; }
    }

    public class ViewerHighlightDto
    {
        public List<int> Severe { get; set; } = new List<int>();
        public List<int> High { get; set; } = new List<int>();
        public List<int> Medium { get; set; } = new List<int>();
        public List<int> Low { get; set; } = new List<int>();
        public List<int> Rooms { get; set; } = new List<int>();
        public Dictionary<string, int[]> SeverityColors { get; set; } = new Dictionary<string, int[]>();
        public int FlashIntervalMs { get; set; } = 750;
    }
}
