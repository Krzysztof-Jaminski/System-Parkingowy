using Microsoft.AspNetCore.Mvc;
using System_Parkingowy.Modules.DatabaseModule;
using Models;
using System;
using System.Linq;
using System.Collections.Concurrent;

namespace System_Parkingowy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly ParkingDbContext _context;
        // Prosta historia zmian zajętości (symulacja, w realu z bazy)
        private static ConcurrentDictionary<int, List<bool>> _history = new();

        public PredictionController(ParkingDbContext context) { _context = context; }

        [HttpGet("{spotId}")]
        public ActionResult GetPrediction(int spotId, [FromQuery] DateTime date)
        {
            // Prosta symulacja: jeśli w historii w tym dniu było zajęte >50% czasu, przewidujemy zajętość
            var reservations = _context.Reservations.Where(r => r.ParkingSpotId == spotId && r.StartTime.Date == date.Date).ToList();
            var occupiedHours = reservations.Sum(r => (r.EndTime - r.StartTime).TotalHours);
            var prediction = occupiedHours > 6 ? "Occupied" : "Available";
            return Ok(new { SpotId = spotId, Date = date.Date, Prediction = prediction });
        }

        // Metoda do aktualizacji historii (można wywołać z symulatora lub innego miejsca)
        public static void UpdateHistory(int spotId, bool occupied)
        {
            var list = _history.GetOrAdd(spotId, _ => new List<bool>());
            lock (list)
            {
                list.Add(occupied);
                if (list.Count > 10) list.RemoveAt(0); // trzymamy ostatnie 10 zmian
            }
        }

        /// <summary>
        /// [Driver] Prognoza zajętości miejsca parkingowego (prosty model: średnia z ostatnich 10 zmian).
        /// </summary>
        [HttpGet("predict/{spotId}")]
        public IActionResult PredictSpot(int spotId)
        {
            if (!_history.TryGetValue(spotId, out var list) || list.Count == 0)
                return Ok(new { spotId, prediction = 0.0 });
            double prediction = list.Average(x => x ? 1.0 : 0.0);
            return Ok(new { spotId, prediction });
        }
    }
} 