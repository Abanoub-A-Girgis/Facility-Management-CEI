using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Sensor
    {
        #region Entity Properties
        [DatabaseGenerated(DatabaseGeneratedOption.None)]//to make the Id changeable
        public int Id { get; set; }
        public string Name { get; set; }
        public SensorType SensorType { get; set; }
        #endregion

        #region Link with other
        [Required]
        public int SpaceId { get; set; }//onDelete: ReferentialAction.NoAction);//we made this (space must migration)//try modelBuilder in AplicationDBContext ClientCascade
        public Space Space { get; set; }
        public List<SensorWarning> SensorWarnings { get; set; }
        public int? AssetId { get; set; }
        public Asset Asset { get; set; }
        #endregion
    }
}
