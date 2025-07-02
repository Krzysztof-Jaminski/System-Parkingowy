using System;
using System_Parkingowy.Modules.AuthModule;
using System_Parkingowy.Modules.BookingModule;
using System_Parkingowy.Modules.DatabaseModule;
using System_Parkingowy.Modules.NotificationModule;
using System_Parkingowy.Modules.PaymentModule;
using Models;
using System_Parkingowy.Models;

public class ParkingSystemFacade
{
    private readonly ReservationManager _reservationManager;
    private readonly AuthService _authService;
    private readonly DatabaseService _dbService;
    private readonly NotificationService _notificationService;
    public ParkingSystemFacade()
    {
        _dbService = new DatabaseService();
        _notificationService = new NotificationService(new StandardNotificationFactory());
        _authService = new AuthService(_dbService, _notificationService);
        _reservationManager = new ReservationManager(_dbService, _notificationService);
    }
    public User RegisterAndVerifyUser(string email, string password)
    {
        var data = new UserData(email, password);
        _authService.Register(data);
        _authService.Verify(email);
        return _dbService.GetUserByEmail(email);
    }
    public void BookSpot(User user, int spotId, DateTime start, DateTime end)
    {
        var spot = _dbService.GetSpotById(spotId);
        if (spot == null)
        {
            Console.WriteLine($"[Facade] Nie znaleziono miejsca parkingowego o ID {spotId}.");
            return;
        }
        var reservation = new Reservation(_dbService.GetNextReservationId(), user.Id, spot, start, end, 0);
        _reservationManager.MakeReservation(reservation);
    }
    public void PayForReservation(int reservationId, PaymentFactory factory)
    {
        _reservationManager.PayForReservation(reservationId, factory);
    }
    public void CancelReservation(int reservationId)
    {
        _reservationManager.CancelReservation(reservationId);
    }
    public void SetFeeStrategy(IFeeStrategy strategy)
    {
        _reservationManager.SetFeeStrategy(strategy);
    }
}