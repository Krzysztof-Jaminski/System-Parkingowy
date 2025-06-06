using Models;
using System.Collections.Generic;


namespace System_Parkingowy.Modules.DatabaseModule
{
    // Interfejs bazy danych
    public interface IDatabaseService
    {
        void AddUser(User user);
        User GetUserByEmail(string email);
        User GetUserById(int id);
        int GetNextUserId();
        List<ParkingSpot> SearchSpots(string location);
        ParkingSpot GetSpotById(int id);
        void AddParkingSpot(ParkingSpot spot);
    }
}