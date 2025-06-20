using Microsoft.AspNetCore.Mvc;
using System_Parkingowy.Modules.AuthModule;
using System_Parkingowy.Modules.DatabaseModule;

namespace System_Parkingowy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserData data)
        {
            _authService.Register(data);
            return Ok("User registered");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserData data)
        {
            var result = _authService.Login(data);
            if (result.StartsWith("[AuthModule] Logowanie nieudane"))
                return Unauthorized(result);
            return Ok(result);
        }
    }
} 