using System_Parkingowy.Modules.DatabaseModule;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace Models
{
    // Reprezentuje miejsce parkingowe
    public class ParkingSpot
    {
        [Key]
        public int Id { get; set; }
        public string Location { get; set; }
        public string Zone { get; set; }
        public bool Available { get; set; }
        public ICollection<Reservation> Reservations { get; set; }

        public ParkingSpot(int id, string location, string zone)
        {
            Id = id;
            Location = location;
            Zone = zone;
            Available = true;
        }

        public ParkingSpot() {}

        public void MarkFree()
        {
            Available = true;
        }

        public void MarkOccupied()
        {
            Available = false;
        }
    }

    public class ParkingSpotListResponseExample : IExamplesProvider<IEnumerable<ParkingSpot>>
    {
        public IEnumerable<ParkingSpot> GetExamples() => new List<ParkingSpot>
        {
            new ParkingSpot { Id = 1, Location = "Location A", Zone = "A", Available = true },
            new ParkingSpot { Id = 2, Location = "Location A", Zone = "A", Available = true }
        };
    }

    public class ParkingSpotResponseExample : IExamplesProvider<ParkingSpot>
    {
        public ParkingSpot GetExamples() => new ParkingSpot { Id = 0, Location = "Example Location", Zone = "X", Available = true };
    }
}