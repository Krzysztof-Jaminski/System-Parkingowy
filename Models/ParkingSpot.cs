using System_Parkingowy.Modules.DatabaseModule;

namespace Models
{
    // Reprezentuje miejsce parkingowe
    public class ParkingSpot
    {
        public int Id { get; }
        public string Location { get; }
        public string Zone { get; }
        public bool Available { get; private set; }

        public ParkingSpot(int id, string location, string zone)
        {
            Id = id;
            Location = location;
            Zone = zone;
            Available = true;
        }

        public void MarkFree()
        {
            Available = true;
        }

        public void MarkOccupied()
        {
            Available = false;
        }
    }
}