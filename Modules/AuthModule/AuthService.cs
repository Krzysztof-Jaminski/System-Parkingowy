using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System_Parkingowy.Modules.DatabaseModule;
using System_Parkingowy.Modules.NotificationModule;

namespace System_Parkingowy.Modules.AuthModule
{
    // Usługa autoryzacji, łączy bazę danych oraz moduł powiadomień
    public class AuthService : IAuthService
    {
        private readonly ParkingDbContext _context;
        private readonly NotificationService _notificationService;

        public AuthService(ParkingDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public void Register(UserData data, string role = "Driver")
        {
            if (_context.Users.Any(u => u.Email == data.Email))
            {
                Console.WriteLine($"[AuthModule] Rejestracja nieudana: {data.Email} już istnieje.");
                return;
            }

            if (!IsValidPassword(data.Password))
            {
                Console.WriteLine($"[AuthModule] Hasło nie spełnia wymagań bezpieczeństwa  min. 6 znaków, litera + cyfra.");
                return;
            }

            var user = new User { Email = data.Email, Password = data.Password, Status = UserStatus.Pending, Role = role };
            _context.Users.Add(user);
            _context.SaveChanges();
            _notificationService.SendNotifications(data.Email, "Wysłano e-mail weryfikacyjny po rejestracji.", NotificationType.Email);
            Console.WriteLine($"[AuthModule] Rejestracja zakończona sukcesem.");
        }

        public void Register(UserData data)
        {
            Register(data, "Driver");
        }

        public string Login(UserData data)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == data.Email);
            if (user == null)
                return "[AuthModule] Logowanie nieudane: Niepoprawny e-mail.";

            if (user.Password != data.Password)
                return "[AuthModule] Logowanie nieudane: Niepoprawne hasło.";

            if (user.Status == UserStatus.Pending)
                return $"[AuthModule] Logowanie nieudane: Konto {data.Email} nie jest zweryfikowane.";
            if (user.Status == UserStatus.Blocked)
                return $"[AuthModule] Logowanie nieudane: Konto {data.Email} jest zablokowane.";
            if (user.Status == UserStatus.Deleted)
                return $"[AuthModule] Logowanie nieudane: Konto {data.Email} zostało usunięte.";

            if (user.Status != UserStatus.Active)
                return $"[AuthModule] Logowanie nieudane: Konto {data.Email} nie jest aktywne.";

            return $"[AuthModule] Logowanie udane! Witaj {data.Email}.";
        }

        public void Verify(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                Console.WriteLine($"[AuthModule] Nie można zweryfikować: użytkownik {email} nie istnieje.");
                return;
            }
            try
            {
                user.Status = UserStatus.Active;
                _context.SaveChanges();
                Console.WriteLine($"[AuthModule] Konto {email} zostało zweryfikowane i aktywowane.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthModule] Błąd weryfikacji: {ex.Message}");
            }
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 6 && password.Any(char.IsLetter) && password.Any(char.IsDigit);
        }
    }
}