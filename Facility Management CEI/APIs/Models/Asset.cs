using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Asset
    {
        #region EntityProperties

        [DatabaseGenerated(DatabaseGeneratedOption.None)]//to make the Id changeable
        public int Id { get; set; }
        public string Name { get; set; }
        public string Materials { get; set; }
        #endregion

        #region Link with other entities
        public Space Space { get; set; }
        public int? SpaceId { get; set; }
        public int FloorId { get; set; }
        public Floor Floor { get; set; }
        public List<Incident> Incident { get; set; }
        //Sensor will only monitor single asset (Pressure gauge on a pipe )
        public Sensor Sensor { get; set; }
        #endregion

    }
}
