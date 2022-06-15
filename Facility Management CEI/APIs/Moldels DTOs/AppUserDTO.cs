using API.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class UserRoles
    {
        public UserRoles()
        {

        }
        public UserRoles(List<IdentityRole> roles)
        {
            Roles = new List<SelectListItem>();
            foreach(var item in roles)
            {
                Roles.Add(new SelectListItem(item.Name,item.Name));
            }
        }
        public string UserName { get; set; }
        public string Role { get; set; }    
        public List<SelectListItem> Roles { get; set; }
    }
}
