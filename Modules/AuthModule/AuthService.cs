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
        private readonly INotification _emailService;

        public AuthService(IDatabaseService userDb, INotification emailService)
        {
            _userDb = userDb;
            _emailService = emailService;
        }

        public void Register(UserData data)
        {
            if (_userDb.GetUserByEmail(data.Email) != null)
            {
                Console.WriteLine($"[AuthService] Rejestracja nieudana: {data.Email} już istnieje.");
                return;
            }

            if (!IsValidPassword(data.Password))
            {
                Console.WriteLine($"[AuthService] Hasło nie spełnia wymagań bezpieczeństwa  min. 6 znaków, litera + cyfra.");
                return;
            }

            var user = new User(data.Email, data.Password);
            _userDb.AddUser(user);
            _emailService.SendVerificationEmail(data.Email);
            Console.WriteLine($"[AuthService] Rejestracja zakończona sukcesem.");
        }

        public string Login(UserData data)
        {
            var user = _userDb.GetUserByEmail(data.Email);
            if (user == null)
                return "[AuthService] Logowanie nieudane: Niepoprawny e-mail.";

            if (user.Password != data.Password)
                return "[AuthService] Logowanie nieudane: Niepoprawne hasło.";

            if (!user.IsVerified)
                return $"[AuthService] Logowanie nieudane: Konto {data.Email} nie jest zweryfikowane.";

            return $"[AuthService] Logowanie udane! Witaj {data.Email}.";
        }

        public void Verify(string email)
        {
            var user = _userDb.GetUserByEmail(email);
            if (user == null)
            {
                Console.WriteLine($"[AuthService] Nie można zweryfikować: użytkownik {email} nie istnieje.");
                return;
            }

            user.Verify();
            Console.WriteLine($"[AuthService] Konto {email} zostało zweryfikowane.");
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 6 &&
                   Regex.IsMatch(password, @"[a-zA-Z]") &&
                   Regex.IsMatch(password, @"\d");
        }
    }
}