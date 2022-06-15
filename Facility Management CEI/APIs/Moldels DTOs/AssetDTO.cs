using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class AssetDTO
    {
        #region EntityProperties

       
        public int Id { get; set; }
        public string Name { get; set; }
        #endregion

        #region Link with other entities
        
        public int? SpaceId { get; set; }
        public int FloorId { get; set; }
        
        #endregion
    }
}
