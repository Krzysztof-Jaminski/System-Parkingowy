using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Expired,
        Paid,
        Unknown
    }

    // Rezerwacja
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ParkingSpot ParkingSpot { get; set; }
        public int ParkingSpotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public ReservationStatus Status { get; set; }

        public Reservation() {}

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
            if (Status != ReservationStatus.Pending && Status != ReservationStatus.Confirmed && Status != ReservationStatus.Paid)
                throw new InvalidOperationException("Rezerwacja musi być w stanie oczekującym, potwierdzonym lub opłaconym, aby mogła wygasnąć.");
            Status = ReservationStatus.Expired;
        }
    }
} 