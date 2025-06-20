using System;
using System.Collections.Generic;
using Models;
using Microsoft.EntityFrameworkCore;

namespace System_Parkingowy.Modules.DatabaseModule
{
    // Baza danych ze zbiorem użytkowników, miejsc parkingowych i rezerwacji
    public class DatabaseService : IDatabaseService
    {
        private readonly ParkingDbContext _context;

        public DatabaseService(ParkingDbContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        public int GetNextUserId()
        {
            // EF Core automatycznie nadaje IDENTITY, więc niepotrzebne
            return 0;
        }

        public int GetNextReservationId()
        {
            // EF Core automatycznie nadaje IDENTITY, więc niepotrzebne
            return 0;
        }

        public void AddParkingSpot(ParkingSpot spot)
        {
            _context.ParkingSpots.Add(spot);
            _context.SaveChanges();
        }

        public ParkingSpot GetSpotById(int id)
        {
            return _context.ParkingSpots.Find(id);
        }

        public List<ParkingSpot> SearchSpots(string location)
        {
            return _context.ParkingSpots.Where(s => s.Location == location && s.Available).ToList();
        }

        public List<ParkingSpot> GetAllParkingSpots()
        {
            return _context.ParkingSpots.ToList();
        }
    }
}
