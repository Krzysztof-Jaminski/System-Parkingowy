using Microsoft.EntityFrameworkCore;
using Models;

namespace System_Parkingowy.Modules.DatabaseModule
{
    public class ParkingDbContext : DbContext
    {
        public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "adam1@gmail.com", PhoneNumber = "123456789", Password = "passwd123", Status = UserStatus.Active, Role = "Admin" },
                new User { Id = 2, Email = "anna2@gmail.com", PhoneNumber = "987654321", Password = "passwd22", Status = UserStatus.Active, Role = "Driver" }
            );

            modelBuilder.Entity<ParkingSpot>().HasData(
                new ParkingSpot { Id = 1, Location = "Location A", Zone = "A", Available = true },
                new ParkingSpot { Id = 2, Location = "Location A", Zone = "A", Available = true },
                new ParkingSpot { Id = 3, Location = "Location A", Zone = "A", Available = true },
                new ParkingSpot { Id = 4, Location = "Location A", Zone = "A", Available = true },
                new ParkingSpot { Id = 5, Location = "Location A", Zone = "A", Available = true },
                new ParkingSpot { Id = 6, Location = "Location B", Zone = "B", Available = true },
                new ParkingSpot { Id = 7, Location = "Location C", Zone = "C", Available = true }
            );
        }
    }
} 