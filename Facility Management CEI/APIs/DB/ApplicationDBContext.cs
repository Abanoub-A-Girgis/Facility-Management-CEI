
using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DB
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Models.Task>()
                .HasOne<User>(c => c.CreatedBy)
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
        public DbSet<User> Users { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }//Models.Task because there is an ambigousity between threading.Task and our task class
    }

}
