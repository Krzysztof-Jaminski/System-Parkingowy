using System_Parkingowy.Modules.DatabaseModule;

namespace Models
{
    // Reprezentuje miejsce parkingowe
    public class ParkingSpot
    {
        public string Id { get; }
        public string Location { get; }
        public bool IsReserved { get; private set; }
        public ReservationData ReservationData { get; set; }

        public ParkingSpot(string id, string location)
        {
            Id = id;
            Location = location;
            IsReserved = false;
        }

        public void Reserve(ReservationData data)
        {
            ReservationData = data;
            IsReserved = true;
        }

        public void CancelReservation()
        {
            IsReserved = false;
        }
    }
}