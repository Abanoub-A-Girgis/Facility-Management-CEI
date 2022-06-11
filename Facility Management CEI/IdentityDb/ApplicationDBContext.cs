using API.Models;
using Facility_Management_CEI.APIs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management_CEI.IdentityDb
{
    public class ApplicationDBContext : IdentityDbContext<LogUser, IdentityRole,string>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> option)
            :base(option)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<API.Models.Task>()
                .HasOne<AppUser>(c => c.CreatedBy)
                .WithMany(t => t.TasksCreated)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.ClientSetNull);//ClientSetNull or ClientCascade will make no action in the database any way
        }
       
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Space> Spaces { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorWarning> SensorWarnings { get; set; }
        public DbSet<API.Models.AppUser> AppUsers { get; set; }
        public DbSet<LogUser> LogUsers { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<API.Models.Task> Tasks { get; set; }//Models.Task because there is an ambigousity between threading.Task and our task class
    }
}
//this in package manager console will make the migration 
//add-migration MyMigration -context Facility_Management_CEI.IdentityDb.ApplicationDBContext