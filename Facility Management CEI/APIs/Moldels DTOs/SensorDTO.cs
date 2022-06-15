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
        #region Entity Properties
        
        public int Id { get; set; }
        public string Name { get; set; }
        public SensorType SensorType { get; set; }
        #endregion

        #region Link with other
        
        public int SpaceId { get; set; }//onDelete: ReferentialAction.NoAction);//we made this (space must migration)//try modelBuilder in AplicationDBContext ClientCascade
        
        public int? AssetId { get; set; }
       
        #endregion
    }
}
