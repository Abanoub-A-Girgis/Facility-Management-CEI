using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Floor
    {
        #region Entity Properties
        [DatabaseGenerated(DatabaseGeneratedOption.None)]//to make the Id changeable that takes the values from Ifc not autonumberd from DB
        public int Id { get; set; }
        public string FloorName { get; set; } //name 
        public string Path { get; set; }//Floor WexBIM File Path
        #endregion

        #region Link with other entities
        
        public int BuildingId { get; set; }
        public Building Building { get; set; }
        //[Required]
        public List<Space> Spaces { get; set; }
        #endregion


    }
}
