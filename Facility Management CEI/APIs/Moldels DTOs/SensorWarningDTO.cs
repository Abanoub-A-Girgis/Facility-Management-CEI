using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class SensorWarningDTO
    {
        #region Entity Properties
        public int Id { get; set; }
       
        public DateTime IssueDate { get; set; }//check it in database
        public string Description { get; set; }
        public bool Investigated { get; set; }
       
        public DateTime? InvestigatDate { get; set; }
        public string Comment { get; set; }

        #endregion

        #region Link with other

        public int SensorId { get; set; }
        
        public int? AppUserId { get; set; }
        

        #endregion
    }
}
