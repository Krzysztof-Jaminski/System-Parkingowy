using System;
using Models;
using System_Parkingowy.Modules.PaymentModule;

namespace System_Parkingowy.Modules.BookingModule
{
    // Interfejs usługi rezerwacji
    public interface IBookingService
    {
        void SearchParkingSpot(string location);      // Wyszukiwanie miejsca
        void MakeReservation(Reservation reservation);   // Rezerwacja miejsca
        void EditReservation(int id, DateTime newStart, DateTime newEnd); // Edycja rezerwacji
        void CancelReservation(int id);            // Anulowanie rezerwacji
        void PayForReservation(int id, PaymentFactory factory); // Nowa metoda do płatności za rezerwację
        ReservationStatus GetReservationStatus(int id);
    }
}