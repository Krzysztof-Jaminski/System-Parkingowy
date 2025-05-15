using System;
using Models;
using System_Parkingowy.Modules.DatabaseModule;

namespace System_Parkingowy.Modules.BookingModule
{
    // Usługa rezerwacji
    public class BookingService : IBookingService
    {
        private readonly IDatabaseService _Db;

        public BookingService(IDatabaseService parkingDb)
        {
            _Db = parkingDb;
        }

        public void SearchParkingSpot(string location)
        {
            var spots = _Db.SearchSpots(location);
            if (spots.Count == 0)
            {
                Console.WriteLine($"[BookingService] Brak dostępnych miejsc w lokalizacji: {location}");
            }
            else
            {
                Console.WriteLine($"[BookingService] Dostępne miejsca w lokalizacji {location}:");
                foreach (var spot in spots)
                {
                    Console.WriteLine($" - {spot.Id}");
                }
            }
        }

        public void MakeReservation(ReservationData data)
        {
            var spot = _Db.GetSpotById(data.SpotId);
            if (spot == null)
            {
                Console.WriteLine($"[BookingService] Miejsce {data.SpotId} nie istnieje.");
                return;
            }

            if (spot.IsReserved)
            {
                Console.WriteLine($"[BookingService] Miejsce {data.SpotId} jest już zarezerwowane.");
                return;
            }

            var user = _Db.GetUserByEmail(data.UserEmail);
            if (user == null)
            {
                Console.WriteLine($"[BookingService] Użytkownik o adresie e-mail {data.UserEmail} nie istnieje.");
                return;
            }

            spot.Reserve(data);
            Console.WriteLine($"[BookingService] Miejsce {data.SpotId} zostało zarezerwowane od {data.StartTime} do {data.EndTime} przez {data.UserEmail}");
        }

        public void EditReservation(string id, ReservationData newData)
        {
            var spot = _Db.GetSpotById(id);
            if (spot != null && spot.IsReserved && spot.Id == newData.SpotId)
            {
                Console.WriteLine($"[BookingService] Rezerwacja na miejscu {id} została edytowana.");
            }
            else
            {
                Console.WriteLine($"[BookingService] Nie znaleziono aktywnej rezerwacji {id} lub dane do edycji są błędne.");
            }
            spot.Reserve(newData);
        }

        public void CancelReservation(string id)
        {
            var spot = _Db.GetSpotById(id);
            if (spot != null && spot.IsReserved)
            {
                spot.CancelReservation();
                Console.WriteLine($"[BookingService] Rezerwacja na miejscu {id} została anulowana.");
            }
            else
            {
                Console.WriteLine($"[BookingService] Nie znaleziono aktywnej rezerwacji dla miejsca {id}.");
            }
        }
    }
}