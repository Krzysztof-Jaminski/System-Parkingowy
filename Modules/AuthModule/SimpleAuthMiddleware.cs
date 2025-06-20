using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System_Parkingowy.Modules.DatabaseModule;

namespace System_Parkingowy.Modules.AuthModule
{
    public class SimpleAuthMiddleware
    {
        private readonly RequestDelegate _next;
        public SimpleAuthMiddleware(RequestDelegate next) { _next = next; }

        public async Task InvokeAsync(HttpContext context, ParkingDbContext db)
        {
            if (context.Request.Headers.TryGetValue("X-User-Email", out var email))
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    context.Items["User"] = user;
                }
            }
            await _next(context);
        }
    }
} 