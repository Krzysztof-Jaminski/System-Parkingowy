using System;

namespace Models
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Expired
    }

    // Rezerwacja
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ParkingSpot ParkingSpot { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ReservationStatus Status { get; set; }
        public decimal TotalPrice { get; set; }

        public Reservation(int id, int userId, ParkingSpot parkingSpot, DateTime start, DateTime end, decimal totalPrice)
        {
            Id = id;
            UserId = userId;
            ParkingSpot = parkingSpot;
            StartTime = start;
            EndTime = end;
            Status = ReservationStatus.Pending;
            TotalPrice = totalPrice;
        }

        public void Confirm()
        {
            if (Status != ReservationStatus.Pending)
                throw new InvalidOperationException("Tylko rezerwacja oczekująca może być potwierdzona.");
            Status = ReservationStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status == ReservationStatus.Cancelled)
                throw new InvalidOperationException("Rezerwacja już anulowana.");
            Status = ReservationStatus.Cancelled;
        }

        public void Expire()
        {
            if (Status != ReservationStatus.Pending)
                throw new InvalidOperationException("Tylko rezerwacja oczekująca może wygasnąć.");
            Status = ReservationStatus.Expired;
        }
    }
} 