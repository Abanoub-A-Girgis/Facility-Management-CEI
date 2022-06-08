using API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Moldels_DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public UserType Type { get; set; }
        public int? BuildingId { get; set; }//test the null
        public int? SuperId { get; set; }//test the null
    }
}
