using System;
using System.Collections.Generic;
using Models;

namespace System_Parkingowy.Modules.DatabaseModule
{
    // Baza danych ze zbiorem użytkowników, miejsc parkingowych i rezerwacji
    public class DatabaseService : IDatabaseService
    {
        private readonly Dictionary<int, User> _users = new();
        private readonly Dictionary<int, ParkingSpot> _spots = new();
        private int _nextUserId = 3;

        public DatabaseService()
        {
            AddUser(new User(1, "adam1@gmail.com", "123456789", "passwd123"));
            AddUser(new User(2, "anna2@gmail.com", "987654321", "passwd22"));

            AddParkingSpot(new ParkingSpot(1, "Location A", "A"));
            AddParkingSpot(new ParkingSpot(2, "Location A", "A"));
            AddParkingSpot(new ParkingSpot(3, "Location A", "A"));
            AddParkingSpot(new ParkingSpot(4, "Location A", "A"));
            AddParkingSpot(new ParkingSpot(5, "Location A", "A"));
            AddParkingSpot(new ParkingSpot(6, "Location B", "B"));
            AddParkingSpot(new ParkingSpot(7, "Location C", "C"));
        }

        public int GetNextUserId()
        {
            return _nextUserId++;
        }

        // Dodanie nowego użytkownika
        public void AddUser(User user)
        {
            if (_users.ContainsKey(user.Id))
            {
                throw new Exception("Użytkownik z tym id już istnieje.");
            }
            _users[user.Id] = user;
        }

        // Pobranie użytkownika po id
        public User GetUserById(int id)
        {
            _users.TryGetValue(id, out var user);
            return user;
        }

        // Pobranie użytkownika po e-mailu
        public User GetUserByEmail(string email)
        {
            foreach (var user in _users.Values)
            {
                if (user.Email == email)
                    return user;
            }
            return null;
        }

        // Dodanie miejsca parkingowego
        public void AddParkingSpot(ParkingSpot spot)
        {
            if (_spots.ContainsKey(spot.Id))
            {
                throw new Exception("Miejsce parkingowe już istnieje.");
            }
            _spots[spot.Id] = spot;
        }

        // Pobranie miejsca parkingowego
        public ParkingSpot GetSpotById(int spotId)
        {
            _spots.TryGetValue(spotId, out var spot);
            return spot;
        }

        // Wyszukiwanie dostępnych miejsc w danej lokalizacji
        public List<ParkingSpot> SearchSpots(string location)
        {
            var result = new List<ParkingSpot>();
            foreach (var spot in _spots.Values)
            {
                if (spot.Location == location && spot.Available)
                {
                    result.Add(spot);
                }
            }
            return result;
        }
    }
}
