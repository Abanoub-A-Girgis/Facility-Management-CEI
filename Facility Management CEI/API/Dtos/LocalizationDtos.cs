using System.Collections.Generic;

namespace Facility_Management_CEI.ApiLayer.Dtos
{
    public class LocalizationDto
    {
        public string Culture { get; set; }
        public string Direction { get; set; }
        public Dictionary<string, string> Translations { get; set; } = new Dictionary<string, string>();
    }
}
