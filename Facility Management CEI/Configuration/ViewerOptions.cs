using System.Collections.Generic;

namespace Facility_Management_CEI.Configuration
{
    public class ViewerOptions
    {
        public int FlashIntervalMs { get; set; } = 750;

        public Dictionary<string, int[]> SeverityColors { get; set; } = new Dictionary<string, int[]>
        {
            ["Severe"] = new[] { 255, 0, 0, 255 },
            ["High"] = new[] { 255, 160, 16, 255 },
            ["Medium"] = new[] { 255, 224, 32, 255 },
            ["Low"] = new[] { 0, 255, 0, 255 }
        };
    }
}
