using Microsoft.AspNetCore.Mvc;
using System_Parkingowy.Modules.DatabaseModule;
using Models;
using System.Linq;

namespace System_Parkingowy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ParkingDbContext _context;
        public SensorController(ParkingDbContext context) { _context = context; }

        [HttpPost("occupancy/{spotId}")]
        public IActionResult SetOccupancy(int spotId, [FromQuery] bool occupied)
        {
            var spot = _context.ParkingSpots.Find(spotId);
            if (spot == null) return NotFound();
            spot.Available = !occupied;
            _context.SaveChanges();
            return Ok(spot);
        }

        [HttpGet("occupancy")]
        public ActionResult GetOccupancy()
        {
            var result = _context.ParkingSpots.Select(s => new { s.Id, s.Location, s.Zone, Occupied = !s.Available }).ToList();
            return Ok(result);
        }
    }
} 