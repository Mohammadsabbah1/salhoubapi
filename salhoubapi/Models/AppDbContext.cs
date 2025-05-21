using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
//using Microsoft.EntityFrameworkCore;

namespace salhoubapi.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }
       
        // Add DbSet for each model
        public DbSet<Category> Categories2 { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<MedicalCondition> MedicalConditions { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Reserv> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<acounting> acountings { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
 
            // Define relationships and constraints (example)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Reserv>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservs)
                .HasForeignKey(r => r.UserId);
        }
    }
}
