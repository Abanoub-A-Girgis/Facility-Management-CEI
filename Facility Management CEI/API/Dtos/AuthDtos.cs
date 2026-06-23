using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Facility_Management_CEI.ApiLayer.Dtos
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthUserDto
    {
        public string Id { get; set; }
        public int? AppUserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public IReadOnlyList<string> Roles { get; set; }
    }
}
