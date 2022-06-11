using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Building
    {
        #region EntityProperties
        [DatabaseGenerated(DatabaseGeneratedOption.None)]//to make the Id changeable  
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        #endregion

        #region Link with other entities
        //[Required]//Review
        public List<Floor> Floors { get; set; }
        
        public List<AppUser> AppUsers { get; set; }
        #endregion
    }

}
