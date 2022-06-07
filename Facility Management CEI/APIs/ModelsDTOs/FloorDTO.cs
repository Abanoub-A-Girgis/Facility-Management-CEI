using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class FloorDTO
    {
        public int Id { get; set; }
        public string FloorName { get; set; } //name 
        public int BuildingId { get; set; }//we must have it here in case we want to add through the dto
    }
}
