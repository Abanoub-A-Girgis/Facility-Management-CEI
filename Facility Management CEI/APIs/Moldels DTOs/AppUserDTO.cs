using API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class AppUserDTO
    {
        #region Entity Properties
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType Type { get; set; }
        #endregion

        #region Link with other
       
        public int? BuildingId { get; set; } //new//may may user one to many can be null
       
       
        public int? SuperId { get; set; }//user SuperVisor Id
        
        public string LogUserId { get; set; }
       
        #endregion
    }
}
