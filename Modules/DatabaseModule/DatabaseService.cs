using System;
using System.Collections.Generic;
using Models;

namespace System_Parkingowy.Modules.DatabaseModule
{
    // Baza danych ze zbiorem użytowników oraz miejsc parkingowych
    public class DatabaseService : IDatabaseService
    {
        private readonly Dictionary<string, User> _users = new();
        private readonly Dictionary<string, ParkingSpot> _spots = new();

        public DatabaseService()
        {
            AddUser(new User("adam1@gmail.com", "passwd123"));
            AddUser(new User("anna2@gmail.com", "passwd22"));

            AddParkingSpot(new ParkingSpot("A1", "Location A"));
            AddParkingSpot(new ParkingSpot("A2", "Location A"));
            AddParkingSpot(new ParkingSpot("A3", "Location A"));
            AddParkingSpot(new ParkingSpot("A4", "Location A"));
            AddParkingSpot(new ParkingSpot("A5", "Location A"));
            AddParkingSpot(new ParkingSpot("B1", "Location B"));
            AddParkingSpot(new ParkingSpot("C1", "Location C"));
        }

        // Dodanie nowego użytkownika
        public void AddUser(User user)
        {
            if (_users.ContainsKey(user.Email))
            {
                throw new Exception("Użytkownik z tym e-mailem już istnieje.");
            }

            _users[user.Email] = user;
        }

        // Pobranie użytkownika po e-mailu
        public User GetUserByEmail(string email)
        {
            _users.TryGetValue(email, out var user);
            return user;
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
        public ParkingSpot GetSpotById(string spotId)
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
                if (spot.Location == location && !spot.IsReserved)
                {
                    result.Add(spot);
                }
            }

            return result;
        }
    }
}
