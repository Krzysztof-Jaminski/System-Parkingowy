using System_Parkingowy.Modules.DatabaseModule;

namespace System_Parkingowy.Modules.BookingModule
{
    // Interfejs usługi rezerwacji
    public interface IBookingService
    {
        void SearchParkingSpot(string location);      // Wyszukiwanie miejsca
        void MakeReservation(ReservationData data);   // Rezerwacja miejsca
        void EditReservation(string id, ReservationData newData); // Edycja rezerwacji
        void CancelReservation(string id);            // Anulowanie rezerwacji
    }
}