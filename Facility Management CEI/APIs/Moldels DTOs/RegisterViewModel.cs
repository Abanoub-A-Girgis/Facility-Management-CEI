using API.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Facility_Management_CEI.Models
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            var EnumMember = Enum.GetNames(typeof(UserType));
            Roles = new List<SelectListItem>();
            foreach (var Role in EnumMember)
            {
                if (Role != "AccountManager")
                {
                    Roles.Add(new SelectListItem(Role, Role));
                }
            }
        }
        //public int Id { get; set; } //the id is auto generated
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        //public UserType Type { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public string Role { get; set; }
        public int? BuildingId { get; set; }//test the null
        public int? SuperId { get; set; }//test the null
        public IFormFile ProfilePicture { get; set; }
    }
    
}
