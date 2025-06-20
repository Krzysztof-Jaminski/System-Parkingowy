using Microsoft.AspNetCore.Mvc;
using System_Parkingowy.Modules.BookingModule;
using System_Parkingowy.Modules.PaymentModule;
using System_Parkingowy.Modules.DatabaseModule;

namespace System_Parkingowy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public PaymentController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("pay/{reservationId}")]
        public IActionResult PayForReservation(int reservationId, [FromQuery] string method)
        {
            PaymentFactory factory = method?.ToLower() switch
            {
                "blik" => new BLIKPaymentFactory(),
                "paypal" => new PayPalPaymentFactory(),
                _ => new CreditCardPaymentFactory(),
            };
            _bookingService.PayForReservation(reservationId, factory);
            return Ok("Payment processed");
        }
    }
} 