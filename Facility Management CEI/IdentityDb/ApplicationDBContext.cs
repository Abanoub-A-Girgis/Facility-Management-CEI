using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management_CEI.IdentityDb
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser, IdentityRole,string>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> option)
            :base(option)
        {

        }
    }
}
//this in package manager console will make the migration 
//add-migration MyMigration -context Facility_Management_CEI.IdentityDb.ApplicationDBContext