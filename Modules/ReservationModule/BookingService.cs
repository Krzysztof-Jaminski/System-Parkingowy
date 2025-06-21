using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models;
using System_Parkingowy.Modules.DatabaseModule;
using System.Collections.Generic;
using System_Parkingowy.Modules.NotificationModule;
using System_Parkingowy.Modules.PaymentModule;

namespace System_Parkingowy.Modules.BookingModule
{
    // Menedżer rezerwacji
    public class ReservationManager : IBookingService
    {
        private readonly ParkingDbContext _context;
        private readonly NotificationService _notificationService;

        public ReservationManager(ParkingDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public void SearchParkingSpot(string location)
        {
            var allSpots = _context.ParkingSpots.Where(s => s.Location == location).ToList();
            // Logika do konsoli usunięta, bo API nie korzysta z konsoli
        }

        public void MakeReservation(Reservation reservation)
        {
            var user = _context.Users.Find(reservation.UserId);
            if (user == null || user.Status != UserStatus.Active)
                return;

            // Sprawdzenie nakładających się rezerwacji
            var overlapping = _context.Reservations
                .Where(r => r.ParkingSpotId == reservation.ParkingSpotId &&
                    (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Paid || r.Status == ReservationStatus.Pending) &&
                    (reservation.StartTime < r.EndTime && reservation.EndTime > r.StartTime))
                .ToList();
            if (overlapping.Any())
            {
                _notificationService.SendNotifications(user.Email, $"Twoja proba rezerwacji miejsca {reservation.ParkingSpotId} na dzien {reservation.StartTime.ToShortDateString()} zakonczyla sie niepowodzeniem, poniewaz miejsce jest juz zarezerwowane w podanym przedziale czasowym.", NotificationType.Email);
                return;
            }

            try
            {
                reservation.Status = ReservationStatus.Confirmed;
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                _notificationService.SendNotifications(user.Email, $"Twoja rezerwacja dla miejsca {reservation.ParkingSpotId} na dzien {reservation.StartTime.ToShortDateString()} zostala potwierdzona!", NotificationType.Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReservationManager] Błąd: {ex}");
            }
        }

        public void EditReservation(int id, DateTime newStart, DateTime newEnd)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null)
                return;

            // Sprawdzenie nakładania się czasowego podczas edycji
            var overlapping = _context.Reservations
                .Where(r => r.Id != id && r.ParkingSpotId == reservation.ParkingSpotId && r.Status == ReservationStatus.Confirmed && (newStart < r.EndTime && newEnd > r.StartTime))
                .ToList();
            if (overlapping.Any())
                return;

            reservation.StartTime = newStart;
            reservation.EndTime = newEnd;
            _context.SaveChanges();
        }

        public void CancelReservation(int reservationId)
        {
            var reservation = _context.Reservations.Include(r => r.User).FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null)
                return;

            try
            {
                reservation.Status = ReservationStatus.Cancelled;
                _context.SaveChanges();
                var user = reservation.User;
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                        _notificationService.SendNotifications(user.PhoneNumber, $"Twoja rezerwacja dla miejsca {reservation.ParkingSpotId} na dzien {reservation.StartTime.ToShortDateString()} zostala anulowana.", NotificationType.Sms);
                    else
                        _notificationService.SendNotifications(user.Email, $"Twoja rezerwacja dla miejsca {reservation.ParkingSpotId} na dzien {reservation.StartTime.ToShortDateString()} zostala anulowana.", NotificationType.Email);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReservationManager] Błąd: {ex}");
            }
        }

        public void PayForReservation(int id, PaymentFactory factory)
        {
            var reservation = _context.Reservations.Include(r => r.User).FirstOrDefault(r => r.Id == id);
            if (reservation == null || reservation.Status != ReservationStatus.Confirmed)
                return;

            try
            {
                PaymentProcessor paymentProcessor = new PaymentProcessor(factory);
                paymentProcessor.ProcessPayment();
                reservation.Status = ReservationStatus.Paid;
                _context.SaveChanges();
                var user = reservation.User;
                if (user != null)
                    _notificationService.SendNotifications(user.Email, $"Płatność za rezerwację miejsca {reservation.ParkingSpotId} na dzien {reservation.StartTime.ToShortDateString()} zostala zaakceptowana.", NotificationType.Push);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReservationManager] Błąd: {ex}");
            }
        }

        public ReservationStatus GetReservationStatus(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null)
                return ReservationStatus.Unknown;
            return reservation.Status;
        }
    }
}