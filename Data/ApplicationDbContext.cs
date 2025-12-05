<<<<<<< HEAD
using FitnessApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

=======
using Microsoft.EntityFrameworkCore;
using FitnessApp.Models;
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162


namespace FitnessApp.Data
{
<<<<<<< HEAD
    public class ApplicationDbContext : IdentityDbContext
=======
    public class ApplicationDbContext : DbContext
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Gym> Gyms { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<TrainerService>? TrainerServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TrainerService>()
                .HasKey(ts => new { ts.TrainerId, ts.ServiceId });

            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainerServices)
                .HasForeignKey(ts => ts.TrainerId);

            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.Service)
                .WithMany(s => s.TrainerServices)
                .HasForeignKey(ts => ts.ServiceId);
        }


        
    }
}
