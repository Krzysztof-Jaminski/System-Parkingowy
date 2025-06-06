using System;
using Models;
using System_Parkingowy.Modules.DatabaseModule;
using System.Collections.Generic;

namespace System_Parkingowy.Modules.BookingModule
{
    // Menedżer rezerwacji
    public class ReservationManager : IBookingService
    {
        private readonly IDatabaseService _Db;
        private readonly List<Reservation> _reservations = new();

        public ReservationManager(IDatabaseService db)
        {
            _Db = db;
        }

        public void SearchParkingSpot(string location)
        {
            var spots = _Db.SearchSpots(location);
            if (spots.Count == 0)
            {
                Console.WriteLine($"[ReservationManager] Brak dostępnych miejsc w lokalizacji: {location}");
            }
            else
            {
                Console.WriteLine($"[ReservationManager] Dostępne miejsca w lokalizacji {location}:");
                foreach (var spot in spots)
                {
                    Console.WriteLine($" - {spot.Id}");
                }
            }
        }

        public void MakeReservation(Reservation reservation)
        {
            if (!reservation.ParkingSpot.Available)
            {
                Console.WriteLine($"[ReservationManager] Miejsce {reservation.ParkingSpot.Id} nie jest dostępne do rezerwacji.");
                return;
            }

            var user = _Db.GetUserById(reservation.UserId);
            if (user == null)
            {
                Console.WriteLine($"[ReservationManager] Użytkownik o id {reservation.UserId} nie istnieje.");
                return;
            }

            try
            {
                reservation.ParkingSpot.MarkOccupied();
                reservation.Confirm();
                _reservations.Add(reservation);
                Console.WriteLine($"[ReservationManager] Miejsce {reservation.ParkingSpot.Id} zostało zarezerwowane od {reservation.StartTime} do {reservation.EndTime} przez użytkownika {reservation.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReservationManager] Błąd rezerwacji: {ex.Message}");
            }
        }

        public void EditReservation(int id, DateTime newStart, DateTime newEnd)
        {
            var reservation = _reservations.Find(r => r.Id == id);
            if (reservation == null)
            {
                Console.WriteLine($"[ReservationManager] Nie znaleziono rezerwacji o id {id}.");
                return;
            }
            reservation.StartTime = newStart;
            reservation.EndTime = newEnd;
            Console.WriteLine($"[ReservationManager] Rezerwacja {id} została edytowana.");
        }

        public void CancelReservation(int id)
        {
            var reservation = _reservations.Find(r => r.Id == id);
            if (reservation == null)
            {
                Console.WriteLine($"[ReservationManager] Nie znaleziono rezerwacji o id {id}.");
                return;
            }
            try
            {
                reservation.ParkingSpot.MarkFree();
                reservation.Cancel();
                Console.WriteLine($"[ReservationManager] Rezerwacja {id} została anulowana.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReservationManager] Błąd anulowania rezerwacji: {ex.Message}");
            }
        }
    }
}