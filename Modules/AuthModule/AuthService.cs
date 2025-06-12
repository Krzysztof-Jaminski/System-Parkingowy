using Models;
using System;
using System.Text.RegularExpressions;
using System_Parkingowy.Modules.DatabaseModule;
using System_Parkingowy.Modules.NotificationModule;

namespace System_Parkingowy.Modules.AuthModule
{
    // Usługa autoryzacji, łączy bazę danych oraz moduł powiadomień
    public class AuthService : IAuthService
    {
        private readonly IDatabaseService _userDb;
        private readonly NotificationService _notificationService;

        public AuthService(IDatabaseService userDb, NotificationService notificationService)
        {
            _userDb = userDb;
            _notificationService = notificationService;
        }

        public void Register(UserData data)
        {
            if (_userDb.GetUserByEmail(data.Email) != null)
            {
                Console.WriteLine($"[AuthModule] Rejestracja nieudana: {data.Email} już istnieje.");
                return;
            }

            if (!IsValidPassword(data.Password))
            {
                Console.WriteLine($"[AuthModule] Hasło nie spełnia wymagań bezpieczeństwa  min. 6 znaków, litera + cyfra.");
                return;
            }

            var user = new User(_userDb.GetNextUserId(), data.Email, "", data.Password);
            _userDb.AddUser(user);
            _notificationService.SendNotifications(data.Email, "Wysłano e-mail weryfikacyjny po rejestracji.", NotificationType.Email);
            Console.WriteLine($"[AuthModule] Rejestracja zakończona sukcesem.");
        }

        public string Login(UserData data)
        {
            var user = _userDb.GetUserByEmail(data.Email);
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
            var user = _userDb.GetUserByEmail(email);
            if (user == null)
            {
                Console.WriteLine($"[AuthModule] Nie można zweryfikować: użytkownik {email} nie istnieje.");
                return;
            }
            try
            {
                user.Activate();
                Console.WriteLine($"[AuthModule] Konto {email} zostało zweryfikowane i aktywowane.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthModule] Błąd weryfikacji: {ex.Message}");
            }
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 6 &&
                   Regex.IsMatch(password, @"[a-zA-Z]") &&
                   Regex.IsMatch(password, @"\d");
        }
    }
}