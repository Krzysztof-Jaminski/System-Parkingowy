using System;
using Models;

namespace System_Parkingowy.Modules.BookingModule
{
    // Interfejs usługi rezerwacji
    public interface IBookingService
    {
        void SearchParkingSpot(string location);      // Wyszukiwanie miejsca
        void MakeReservation(Reservation reservation);   // Rezerwacja miejsca
        void EditReservation(int id, DateTime newStart, DateTime newEnd); // Edycja rezerwacji
        void CancelReservation(int id);            // Anulowanie rezerwacji
    }
}