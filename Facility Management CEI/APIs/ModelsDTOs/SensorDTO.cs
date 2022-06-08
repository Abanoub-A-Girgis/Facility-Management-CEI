using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class SensorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SpaceId { get; set; }
        public string SensorType { get; set; }
        public int? AssetId { get; set; }
    }
}
