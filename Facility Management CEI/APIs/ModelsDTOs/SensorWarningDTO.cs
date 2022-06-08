using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class SensorWarningDTO
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public string Description { get; set; }
        public bool Investigated { get; set; }
        public Enums.Priority Priority { get; set; }
        //[DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }//check it in database
        public DateTime? InvestigatDate { get; set; }
        public int? UserId { get; set; }//test the null
    }
}
