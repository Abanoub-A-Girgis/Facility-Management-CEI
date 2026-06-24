using System.Collections.Generic;

namespace Facility_Management_CEI.Configuration
{
    public class ViewerOptions
    {
        public int FlashIntervalMs { get; set; } = 750;

        public Dictionary<string, int[]> SeverityColors { get; set; } = new Dictionary<string, int[]>
        {
            ["Severe"] = new[] { 220, 38, 38, 255 },
            ["High"] = new[] { 249, 115, 22, 255 },
            ["Medium"] = new[] { 234, 179, 8, 255 },
            ["Low"] = new[] { 22, 163, 74, 255 }
        };
    }
}
