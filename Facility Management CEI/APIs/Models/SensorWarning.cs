using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class SensorWarning
    {
        #region Entity Properties
        public int Id { get; set; }
        //[DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }  //check it in database
        public string Description { get; set; }  
        public bool Investigated { get; set; }
        public Priority Priority { get; set; }
        //[DataType(DataType.Date)]
        public DateTime? InvestigatDate { get; set; }
        #endregion

        #region Link with other
        [Required]
        public int SensorId { get; set; }
        public Sensor Sensor { get; set; }
        public int? AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        #endregion
    }
}
