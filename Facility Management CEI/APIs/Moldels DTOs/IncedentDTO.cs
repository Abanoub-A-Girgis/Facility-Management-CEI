using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class IncedentDTO
    {
        public int Id { get; set; }
        public IncidentStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string Description { get; set; }
        public DateTime ReportingTime { get; set; }
        public string Comment { get; set; }

        public int? AssetId { get; set; }
       
        public int? SensorWarningId { get; set; }
       
        public int AppUserId { get; set; }
      
        
        public int SpaceId { get; set; }
        
    }
}
