using Microsoft.AspNetCore.Mvc;
using System_Parkingowy.Modules.BookingModule;
using System_Parkingowy.Modules.DatabaseModule;
using Models;
using System.Linq;
using System.Collections.Generic;

namespace System_Parkingowy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ParkingDbContext _context;

        public ReservationController(IBookingService bookingService, ParkingDbContext context)
        {
            _bookingService = bookingService;
            _context = context;
        }

        private bool IsAdmin() => HttpContext.Items["User"] is Models.User u && u.Role == "Admin";
        private int? GetUserId() => (HttpContext.Items["User"] as Models.User)?.Id;

        [HttpGet("status/{id}")]
        public ActionResult<string> GetStatus(int id)
        {
            var status = _bookingService.GetReservationStatus(id);
            return status.ToString();
        }

        /// <summary>
        /// [Driver] Rezerwuje miejsce parkingowe.
        /// </summary>
        [HttpPost]
        public IActionResult Reserve([FromBody] Reservation reservation)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == reservation.UserId);
            if (user == null)
                return NotFound("User not found");
            var spot = _context.ParkingSpots.Find(reservation.ParkingSpotId);
            if (spot == null)
                return NotFound("Spot not found");
            reservation.UserId = user.Id;
            reservation.ParkingSpotId = spot.Id;
            reservation.TotalPrice = 50.0m;
            _bookingService.MakeReservation(reservation);
            return Ok("Reservation created");
        }

        /// <summary>
        /// [Driver] Pobiera rezerwacje użytkownika.
        /// </summary>
        [HttpGet("user/{userId}")]
        public IActionResult GetUserReservations(int userId)
        {
            return Ok(_context.Reservations.Where(r => r.UserId == userId).ToList());
        }

        /// <summary>
        /// [Admin] Pobiera wszystkie rezerwacje.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllReservations()
        {
            return Ok(_context.Reservations.ToList());
        }

        [HttpDelete("{id}")]
        public IActionResult CancelReservation(int id)
        {
            var reservation = _context.Reservations.Find(id);
            var userId = GetUserId();
            if (!IsAdmin() && reservation?.UserId != userId) return Forbid();
            _bookingService.CancelReservation(id);
            return Ok();
        }

        [HttpDelete("my/{id}")]
        public IActionResult CancelMyReservation(int id)
        {
            var reservation = _context.Reservations.Find(id);
            var userId = GetUserId();
            if (reservation == null || reservation.UserId != userId) return Forbid();
            _bookingService.CancelReservation(id);
            return Ok();
        }
    }
} 