namespace Facility_Management_CEI.Configuration
{
    public class AdminSeedOptions
    {
        public bool Enabled { get; set; } = true;
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; } = "Admin";
        public string Password { get; set; }
        public bool ResetPasswordOnStartup { get; set; }
    }
}
