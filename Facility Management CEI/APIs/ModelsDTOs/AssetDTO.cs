using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class AssetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SpaceId { get; set; }//needed here
        public int FloorId { get; set; }
    }
}
