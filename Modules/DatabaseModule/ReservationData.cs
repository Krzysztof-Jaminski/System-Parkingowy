using System;
using Swashbuckle.AspNetCore.Filters;

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

    public class ReservationDataExample : IExamplesProvider<ReservationData>
    {
        public ReservationData GetExamples()
        {
            return new ReservationData(
                userEmail: "adam1@gmail.com",
                spotId: "1",
                location: "Location A",
                start: DateTime.Now.Date.AddHours(10),
                end: DateTime.Now.Date.AddHours(12)
            );
        }
    }

    public class ReservationResponseExample : IExamplesProvider<string>
    {
        public string GetExamples() => "Reservation created";
    }
}