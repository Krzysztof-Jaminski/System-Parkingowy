using Microsoft.AspNetCore.Mvc;
using System_Parkingowy.Modules.DatabaseModule;
using Models;
using System.Linq;

namespace System_Parkingowy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingSpotController : ControllerBase
    {
        private readonly ParkingDbContext _context;

        public ParkingSpotController(ParkingDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin() => HttpContext.Items["User"] is Models.User u && u.Role == "Admin";

        /// <summary>
        /// [Driver] Pobiera listę wszystkich miejsc parkingowych.
        /// Funkcjonalność: Monitoring zajętości – pozwala kierowcy zobaczyć dostępne miejsca na parkingu.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<ParkingSpot>> GetAll()
        {
            return Ok(_context.ParkingSpots.ToList());
        }

        /// <summary>
        /// [Driver/Admin] Pobiera szczegóły miejsca parkingowego.
        /// Funkcjonalność: Monitoring zajętości – pozwala na podgląd szczegółów wybranego miejsca.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<ParkingSpot> GetById(int id)
        {
            var spot = _context.ParkingSpots.Find(id);
            if (spot == null)
                return NotFound();
            return Ok(spot);
        }

        /// <summary>
        /// [Driver] Wyszukuje dostępne miejsca parkingowe po lokalizacji.
        /// Funkcjonalność: Monitoring zajętości – umożliwia wyszukiwanie wolnych miejsc w wybranej lokalizacji.
        /// </summary>
        [HttpGet("search/{location}")]
        public ActionResult<IEnumerable<ParkingSpot>> SearchByLocation(string location)
        {
            return Ok(_context.ParkingSpots.Where(s => s.Location == location && s.Available).ToList());
        }

        /// <summary>
        /// [Admin] Dodaje nowe miejsce parkingowe.
        /// Funkcjonalność: Panel administracyjny – umożliwia adminowi dodawanie nowych miejsc do systemu.
        /// </summary>
        [HttpPost]
        public IActionResult AddSpot([FromBody] ParkingSpot spot)
        {
            if (!IsAdmin()) return Forbid();
            _context.ParkingSpots.Add(spot);
            _context.SaveChanges();
            return Ok(spot);
        }

        /// <summary>
        /// [Admin] Edytuje miejsce parkingowe.
        /// Funkcjonalność: Panel administracyjny – umożliwia adminowi edycję danych miejsca parkingowego.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult EditSpot(int id, [FromBody] ParkingSpot spot)
        {
            if (!IsAdmin()) return Forbid();
            var existing = _context.ParkingSpots.Find(id);
            if (existing == null) return NotFound();
            existing.Location = spot.Location;
            existing.Zone = spot.Zone;
            existing.Available = spot.Available;
            _context.SaveChanges();
            return Ok(existing);
        }

        /// <summary>
        /// [Admin] Usuwa miejsce parkingowe.
        /// Funkcjonalność: Panel administracyjny – umożliwia adminowi usuwanie miejsc z systemu.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteSpot(int id)
        {
            if (!IsAdmin()) return Forbid();
            var spot = _context.ParkingSpots.Find(id);
            if (spot == null) return NotFound();
            _context.ParkingSpots.Remove(spot);
            _context.SaveChanges();
            return Ok();
        }
    }
} 