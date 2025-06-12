using System;
using System_Parkingowy.Modules.AuthModule;
using System_Parkingowy.Modules.BookingModule;
using System_Parkingowy.Modules.DatabaseModule;
using System_Parkingowy.Modules.NotificationModule;
using System_Parkingowy.Modules.PaymentModule;
using Models;

class Program
{
    static void Main()
    {
        var DbService = new DatabaseService();
        var notificationService = new NotificationService(new StandardNotificationFactory());
        var authService = new AuthService(DbService, notificationService);
        var reservationManager = new ReservationManager(DbService, notificationService);

        Console.WriteLine("\n=== SCENARIUSZ 1: REZERWACJA I ANULOWANIE ===");
        reservationManager.SearchParkingSpot("Location A");

        var user1 = RegisterAndVerifyUser(authService, DbService, "user1@example.com", "Pass123");
        if (user1 == null) return;

        var spot = DbService.GetSpotById(1);
        var reservation1 = new Reservation(DbService.GetNextReservationId(), user1.Id, spot, 
            DateTime.Now.Date.AddHours(10), DateTime.Now.Date.AddHours(12), 50.0m);
        
        reservationManager.MakeReservation(reservation1);
        reservationManager.PayForReservation(reservation1.Id, new CreditCardPaymentFactory());
        reservationManager.CancelReservation(reservation1.Id);

        Console.WriteLine("\n=== SCENARIUSZ 2: DWIE REZERWACJE W RÓŻNYCH TERMINACH ===");
        reservationManager.SearchParkingSpot("Location A");

        var user2 = RegisterAndVerifyUser(authService, DbService, "user2@example.com", "Pass456");
        var user3 = RegisterAndVerifyUser(authService, DbService, "user3@example.com", "Pass789");
        if (user2 == null || user3 == null) return;

        var reservation2 = new Reservation(DbService.GetNextReservationId(), user2.Id, spot,
            DateTime.Now.Date.AddHours(8), DateTime.Now.Date.AddHours(10), 40.0m);
        var reservation3 = new Reservation(DbService.GetNextReservationId(), user3.Id, spot,
            DateTime.Now.Date.AddHours(14), DateTime.Now.Date.AddHours(16), 40.0m);

        reservationManager.MakeReservation(reservation2);
        reservationManager.PayForReservation(reservation2.Id, new CreditCardPaymentFactory());
        reservationManager.MakeReservation(reservation3);

        Console.WriteLine($"\nStatus rezerwacji 2: {reservationManager.GetReservationStatus(reservation2.Id)}");
        Console.WriteLine($"Status rezerwacji 3: {reservationManager.GetReservationStatus(reservation3.Id)}");
        Console.ReadKey();
    }

    private static User RegisterAndVerifyUser(AuthService authService, DatabaseService dbService, string email, string password)
    {
        authService.Register(new UserData(email, password));
        authService.Verify(email);
        return dbService.GetUserByEmail(email);
    }
}