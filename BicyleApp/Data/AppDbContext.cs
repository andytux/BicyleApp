using BicyleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using static BicyleApp.Data.Enums.Enum;

namespace BicyleApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Ride> Rides { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique ride per user per day
            modelBuilder.Entity<Ride>()
                .HasIndex(r => new { r.UserId, r.Date })
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue(UserRole.User);
        }
    }
}
