using API.Enums;

namespace Facility_Management_CEI.Models
{
    public class RegisterViewModel
    {
        //public int Id { get; set; } //the id is auto generated
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        //public UserType Type { get; set; }
        //public int? BuildingId { get; set; }//test the null
        //public int? SuperId { get; set; }//test the null
    }
}
