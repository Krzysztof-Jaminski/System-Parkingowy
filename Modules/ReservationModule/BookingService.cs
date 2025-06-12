using System;
using Models;
using System_Parkingowy.Modules.DatabaseModule;
using System.Collections.Generic;
using System.Linq;
using System_Parkingowy.Modules.NotificationModule;

namespace System_Parkingowy.Modules.BookingModule
{
    // Menedżer rezerwacji
    public class ReservationManager : IBookingService
    {
        private readonly IDatabaseService _Db;
        private readonly NotificationService _notificationService;
        private readonly List<Reservation> _reservations = new();

        public ReservationManager(IDatabaseService db, NotificationService notificationService)
        {
            _Db = db;
            _notificationService = notificationService;
        }

        public void SearchParkingSpot(string location)
        {
            var allSpots = _Db.GetAllParkingSpots().Where(s => s.Location == location).ToList();

            if (allSpots.Count == 0)
            {
                Console.WriteLine($"[BookingModule] Brak miejsc parkingowych w lokalizacji: {location}");
                return;
            }

            Console.WriteLine($"[BookingModule] Status miejsc parkingowych w lokalizacji {location}:");
            foreach (var spot in allSpots)
            {
                var activeReservations = _reservations.Where(
                    r => r.ParkingSpot.Id == spot.Id && 
                    (r.Status == Models.ReservationStatus.Confirmed || 
                     r.Status == Models.ReservationStatus.Paid ||
                     r.Status == Models.ReservationStatus.Pending))
                    .OrderBy(r => r.StartTime)
                    .ToList();

                if (activeReservations.Any())
                {
                    Console.WriteLine($" - Miejsce {spot.Id} jest zajęte w następujących terminach:");
                    foreach (var res in activeReservations)
                    {
                        Console.WriteLine($"   Od: {res.StartTime.ToShortTimeString()} Do: {res.EndTime.ToShortTimeString()} (Status: {res.Status})");
                    }
                }
                else
                {
                    Console.WriteLine($" - Miejsce {spot.Id} jest dostępne.");
                }
            }
        }

        public void MakeReservation(Reservation reservation)
        {
            var user = _Db.GetUserById(reservation.UserId);
            if (user == null)
            {
                Console.WriteLine($"[BookingModule] Użytkownik o id {reservation.UserId} nie istnieje.");
                return;
            }

            if (user.Status != Models.UserStatus.Active)
            {
                Console.WriteLine($"[BookingModule] Użytkownik o id {reservation.UserId} nie jest aktywny. Status: {user.Status}");
                return;
            }

            // Czyszczenie tylko wygasłych rezerwacji (nie anulowanych)
            _reservations.RemoveAll(r => r.Status == Models.ReservationStatus.Expired);

            Console.WriteLine($"[BookingModule] Sprawdzanie nakładających się rezerwacji dla miejsca {reservation.ParkingSpot.Id} w terminie {reservation.StartTime} - {reservation.EndTime}");
            
            // Sprawdzenie, czy nowa rezerwacja nakłada się na istniejące rezerwacje
            var overlappingReservations = _reservations.Where(r => 
                r.ParkingSpot.Id == reservation.ParkingSpot.Id &&
                (r.Status == Models.ReservationStatus.Confirmed || 
                 r.Status == Models.ReservationStatus.Paid ||
                 r.Status == Models.ReservationStatus.Pending) &&
                (reservation.StartTime < r.EndTime && reservation.EndTime > r.StartTime))
                .ToList();

            if (overlappingReservations.Any())
            {
                Console.WriteLine($"[BookingModule] Wykryto nakładające się rezerwacje:");
                foreach (var existingReservation in overlappingReservations)
                {
                    Console.WriteLine($"[BookingModule] - Rezerwacja ID={existingReservation.Id}, Status={existingReservation.Status}, Termin={existingReservation.StartTime}-{existingReservation.EndTime}");
                }
                Console.WriteLine($"[BookingModule] Miejsce {reservation.ParkingSpot.Id} jest już zarezerwowane w podanym przedziale czasowym.");
                if (user != null)
                {
                    _notificationService.SendNotifications(user.Email, $"Twoja proba rezerwacji miejsca {reservation.ParkingSpot.Id} na dzien {reservation.StartTime.ToShortDateString()} zakonczyla sie niepowodzeniem, poniewaz miejsce jest juz zarezerwowane w podanym przedziale czasowym.", NotificationType.Email);
                }
                return;
            }

            try
            {
                reservation.Confirm();
                _reservations.Add(reservation);
                Console.WriteLine($"[BookingModule] Miejsce {reservation.ParkingSpot.Id} zostało zarezerwowane od {reservation.StartTime} do {reservation.EndTime} przez użytkownika {reservation.UserId}");
                if (user != null)
                {
                    _notificationService.SendNotifications(user.Email, $"Twoja rezerwacja dla miejsca {reservation.ParkingSpot.Id} na dzien {reservation.StartTime.ToShortDateString()} zostala potwierdzona!", NotificationType.Email);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BookingModule] Błąd rezerwacji: {ex.Message}");
            }
        }

        public void EditReservation(int id, DateTime newStart, DateTime newEnd)
        {
            var reservation = _reservations.Find(r => r.Id == id);
            if (reservation == null)
            {
                Console.WriteLine($"[BookingModule] Nie znaleziono rezerwacji o id {id}.");
                return;
            }

            // Sprawdzenie nakładania się czasowego podczas edycji
            foreach (var existingReservation in _reservations)
            {
                if (existingReservation.Id != id && // Nie porównuj z samą sobą
                    existingReservation.ParkingSpot.Id == reservation.ParkingSpot.Id &&
                    existingReservation.Status == Models.ReservationStatus.Confirmed &&
                    (newStart < existingReservation.EndTime && newEnd > existingReservation.StartTime))
                {
                    Console.WriteLine($"[BookingModule] Edycja rezerwacji {id} nieudana: nakłada się na inną rezerwację.");
                    return;
                }
            }

            reservation.StartTime = newStart;
            reservation.EndTime = newEnd;
            Console.WriteLine($"[BookingModule] Rezerwacja {id} została edytowana.");
        }

        public void CancelReservation(int reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null)
            {
                Console.WriteLine($"[BookingModule] Nie znaleziono rezerwacji o id {reservationId}.");
                return;
            }

            try
            {
                var user = _Db.GetUserById(reservation.UserId);
                reservation.Cancel();
                Console.WriteLine($"[BookingModule] Rezerwacja {reservationId} została anulowana.");
                if (user != null)
                {
                    // Wysyłamy SMS tylko jeśli użytkownik ma podany numer telefonu
                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        _notificationService.SendNotifications(user.PhoneNumber, $"Twoja rezerwacja dla miejsca {reservation.ParkingSpot.Id} na dzien {reservation.StartTime.ToShortDateString()} zostala anulowana.", NotificationType.Sms);
                    }
                    else
                    {
                        // Jeśli nie ma numeru telefonu, wysyłamy email
                        _notificationService.SendNotifications(user.Email, $"Twoja rezerwacja dla miejsca {reservation.ParkingSpot.Id} na dzien {reservation.StartTime.ToShortDateString()} zostala anulowana.", NotificationType.Email);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BookingModule] Błąd anulowania rezerwacji: {ex.Message}");
            }
        }

        public void PayForReservation(int id, System_Parkingowy.Modules.PaymentModule.PaymentFactory factory)
        {
            var reservation = _reservations.Find(r => r.Id == id);
            if (reservation == null)
            {
                Console.WriteLine($"[BookingModule] Nie znaleziono rezerwacji o id {id} do opłacenia.");
                return;
            }

            if (reservation.Status != Models.ReservationStatus.Confirmed)
            {
                Console.WriteLine($"[BookingModule] Rezerwacja {id} ma status {reservation.Status} i nie może być opłacona.");
                return;
            }

            try
            {
                // Utworzenie procesora płatności za pomocą dostarczonej fabryki
                System_Parkingowy.Modules.PaymentModule.PaymentProcessor paymentProcessor = new System_Parkingowy.Modules.PaymentModule.PaymentProcessor(factory);
                Console.WriteLine($"[BookingModule] Przetwarzanie płatności za rezerwację {id} dla użytkownika {reservation.UserId}, miejsce {reservation.ParkingSpot.Id}, kwota: {reservation.TotalPrice:C} ({reservation.StartTime} - {reservation.EndTime})...");
                paymentProcessor.ProcessPayment();
                // Po udanej płatności zmieniamy status rezerwacji na Paid
                reservation.Status = Models.ReservationStatus.Paid;
                Console.WriteLine($"[BookingModule] Płatność za rezerwację {id} zakończona sukcesem. Nowy status: {reservation.Status}.");
                var user = _Db.GetUserById(reservation.UserId);
                if (user != null)
                {
                    _notificationService.SendNotifications(user.Email, $"Płatność za rezerwację miejsca {reservation.ParkingSpot.Id} na dzien {reservation.StartTime.ToShortDateString()} zostala zaakceptowana.", NotificationType.Push);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BookingModule] Błąd podczas płatności za rezerwację {id}: {ex.Message}");
            }
        }

        // Nowa metoda pomocnicza do sprawdzania, czy miejsce parkingowe jest aktualnie zajęte
        private bool IsSpotCurrentlyOccupied(int spotId)
        {
            DateTime now = DateTime.Now;
            return _reservations.Any(r => 
                r.ParkingSpot.Id == spotId && 
                r.Status == Models.ReservationStatus.Confirmed && 
                now >= r.StartTime && now < r.EndTime
            );
        }

        public ReservationStatus GetReservationStatus(int id)
        {
            var reservation = _reservations.Find(r => r.Id == id);
            if (reservation == null)
            {
                Console.WriteLine($"[BookingModule] Nie znaleziono rezerwacji o id {id}.");
                return ReservationStatus.Unknown;
            }
            return reservation.Status;
        }
    }
}