using System;
using System_Parkingowy.Modules.AuthModule;
using System_Parkingowy.Modules.BookingModule;
using System_Parkingowy.Modules.DatabaseModule;
using System_Parkingowy.Modules.NotificationModule;

class Program
{
    static void Main()
    {
        // Setup
        var DbService = new DatabaseService();
        var emailService = new NotificationService();
        var authService = new AuthService(DbService, emailService);
        var bookingService = new BookingService(DbService);

        // Rejestracja
        var userData = new UserData("user@gmail.com", "abc123");
        authService.Register(userData);

        // Logowanie przed weryfikacja
        Console.WriteLine(authService.Login(userData));

        // Symulacja weryfikacji maila przez link
        authService.Verify("user@gmail.com");

        // Logowanie po weryfikacji
        Console.WriteLine(authService.Login(userData));

        // Rezerwacja
        bookingService.SearchParkingSpot("Location A");
        var reservationData = new ReservationData("user@gmail.com","A1","Location A", DateTime.Now, DateTime.Now.AddHours(4));
        bookingService.MakeReservation(reservationData);

        // Próba rezerwacji już zajętego miejsca
        bookingService.MakeReservation(reservationData);

        // Próba edycji rezerwacji z błędnymi danymi
        bookingService.EditReservation("A1", new ReservationData("user2@gmail.com","A2", "Location A", DateTime.Now, DateTime.Now.AddHours(2)));

        // Edycja rezerwacji
        bookingService.EditReservation("A1", new ReservationData("user2@gmail.com", "A1", "Location A", DateTime.Now, DateTime.Now.AddHours(2)));

        // Anulowanie rezerwacji
        bookingService.CancelReservation("A1");

        // Ponowna rezerwacja po anulowaniu
        bookingService.MakeReservation(reservationData);
    }
}