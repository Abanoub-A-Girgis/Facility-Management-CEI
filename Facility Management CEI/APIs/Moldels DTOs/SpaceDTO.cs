using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class SpaceDTO
    {
        #region Entity Properties
        
        public int Id { get; set; }
        public string Name { get; set; }//can be enum
        #endregion

        #region Link with other elements
        
        public int FloorId { get; set; }
       

        #endregion
    }
}
