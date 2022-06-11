using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Incident
    {
        #region Entity Properties
        public int Id { get; set; }
        public IncidentStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string Description { get; set; }

        //[DataType(DataType.Date)]//does not make the datetime have hour minutes and seconds 
        public DateTime ReportingTime { get; set; }

        #endregion

        #region Link with other

        public int? AssetId { get; set; }
        public Asset Asset { get; set; }
        public int? SensorWarningId { get; set; }
        public SensorWarning SensorWarning { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<Models.Task> Task { get; set; }
        //Space could has an incident 
        [Required]
        public int SpaceId { get; set; }
        public Space Space { get; set; }
        #endregion
    }
}
