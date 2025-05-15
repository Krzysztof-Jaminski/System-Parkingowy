using System;

namespace System_Parkingowy.Modules.DatabaseModule
{
    // Dane przekazywane przez interfejs do rezerwacji
    public class ReservationData
    {
        public string SpotId { get; set; }
        public string Location { get; set; }
        public string UserEmail { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ReservationData(string userEmail, string spotId, string location, DateTime start, DateTime end)
        {
            SpotId = spotId;
            Location = location;
            StartTime = start;
            EndTime = end;
            UserEmail = userEmail;
        }
    }
}