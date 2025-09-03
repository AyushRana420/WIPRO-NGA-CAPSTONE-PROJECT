using Microsoft.EntityFrameworkCore;
using Fracto.Backend.Models;

namespace Fracto.Data
{
    public class FractoDbContext : DbContext
    {
        public FractoDbContext(DbContextOptions<FractoDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Specialization> Specializations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints here
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.user)
                .WithMany(u => u.appointments)
                .HasForeignKey(a => a.userId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.doctor)
                .WithMany(d => d.appointments)
                .HasForeignKey(a => a.doctorId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.user)
                .WithMany(u => u.ratings)
                .HasForeignKey(r => r.userId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.doctor)
                .WithMany(d => d.ratings)
                .HasForeignKey(r => r.doctorId);
        }
    }
}