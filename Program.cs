using System;
using System_Parkingowy.Modules.AuthModule;
using System_Parkingowy.Modules.BookingModule;
using System_Parkingowy.Modules.DatabaseModule;
using System_Parkingowy.Modules.NotificationModule;
using Models;

class Program
{
    static void Main()
    {
        // Setup
        var DbService = new DatabaseService();
        var emailService = new NotificationService();
        var authService = new AuthService(DbService, emailService);
        var reservationManager = new ReservationManager(DbService);

        // Rejestracja
        var userData = new UserData("user@gmail.com", "abc123");
        authService.Register(userData);

        // Logowanie przed weryfikacja
        Console.WriteLine(authService.Login(userData));

        // Symulacja weryfikacji maila przez link
        authService.Verify("user@gmail.com");

        // Logowanie po weryfikacji
        Console.WriteLine(authService.Login(userData));

        // Przykład blokowania i odblokowania użytkownika
        var user = DbService.GetUserByEmail("user@gmail.com");
        if (user != null)
        {
            Console.WriteLine($"Status użytkownika: {user.Status}");
            try
            {
                user.Activate();
                Console.WriteLine($"Status po aktywacji: {user.Status}");
                user.Block();
                Console.WriteLine($"Status po blokadzie: {user.Status}");
                user.Unblock();
                Console.WriteLine($"Status po odblokowaniu: {user.Status}");
                user.Delete();
                Console.WriteLine($"Status po usunięciu: {user.Status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Test] Wyjątek User: {ex.Message}");
            }
        }

        // Rezerwacja
        reservationManager.SearchParkingSpot("Location A");
        var spot = DbService.GetSpotById(1);
        var reservation = new Reservation(1, user.Id, spot, DateTime.Now, DateTime.Now.AddHours(4), 40.0m);
        reservationManager.MakeReservation(reservation);

        // Próba ponownego potwierdzenia (powinien być wyjątek)
        try
        {
            reservation.Confirm();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Test] Wyjątek przy ponownym potwierdzeniu: {ex.Message}");
        }

        // Edycja rezerwacji
        reservationManager.EditReservation(1, DateTime.Now.AddHours(1), DateTime.Now.AddHours(5));

        // Anulowanie rezerwacji
        reservationManager.CancelReservation(1);

        // Próba anulowania ponownie (powinien być wyjątek)
        try
        {
            reservation.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Test] Wyjątek przy ponownym anulowaniu: {ex.Message}");
        }

        // Próba wygaszenia anulowanej rezerwacji (powinien być wyjątek)
        try
        {
            reservation.Expire();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Test] Wyjątek przy wygaszaniu anulowanej rezerwacji: {ex.Message}");
        }

        // Ponowna rezerwacja po anulowaniu
        reservationManager.MakeReservation(reservation);
    }
}