using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class FloorDTO
    {
        #region Entity Properties
       
        public int Id { get; set; }
        public string FloorName { get; set; } //name 
        #endregion

        #region Link with other entities

        public int BuildingId { get; set; }
        
        #endregion
    }
}
