using API.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Facility_Management_CEI.APIs.Models
{
    public class LogUser : IdentityUser
    {

        //public LogUser(string userName)
        //            : base(userName)
        //{

        //}
        //[Required]
        public override string Id { get; set; }        
        public AppUser AppUser { get; set; }

    }
}
