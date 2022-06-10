using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Space
    {
        #region Entity Properties
        [DatabaseGenerated(DatabaseGeneratedOption.None)]//to make the Id changeable
        public int Id { get; set; }
        public string Name { get; set; }//can be enum
        #endregion

        #region Link with other elements
        [Required]
        public int FloorId { get; set; }
        public Floor Floor { get; set; }
        //Space has many assets 
        public List<Asset> Assets { get; set; }
        //Space has many Sensors
        public List<Sensor> Sensors { get; set; }

        #endregion

    }
}
