using Microsoft.AspNetCore.Mvc;
using System_Parkingowy.Modules.DatabaseModule;
using Models;
using System.Linq;

namespace System_Parkingowy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ParkingDbContext _context;

        public UserController(ParkingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// [Admin] Pobiera wszystkich użytkowników.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            return Ok(_context.Users.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// [Admin] Usuwa użytkownika.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            return NoContent();
        }
    }
} 