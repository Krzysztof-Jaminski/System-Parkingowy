using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using System_Parkingowy.Modules.BookingModule;
using System_Parkingowy.Modules.DatabaseModule;
using System_Parkingowy.Modules.NotificationModule;
using System_Parkingowy.Models;
using Models;

namespace SystemParkingowy.Tests
{
    public class BookingServiceTests
    {
        // Normalny: Użytkownik aktywny rezerwuje dostępne miejsce
        [Fact]
        public void MakeReservation_ShouldAddReservation_WhenSpotIsAvailable()
        {
            var mockDb = new Mock<IDatabaseService>();
            var mockNotification = new Mock<NotificationService>(new StandardNotificationFactory());
            var user = new User(1, "test@example.com", "123456789", "pass");
            user.Activate();
            var spot = new ParkingSpot(1, "A", "Z1");
            mockDb.Setup(d => d.GetUserById(1)).Returns(user);
            mockDb.Setup(d => d.GetAllParkingSpots()).Returns(new List<ParkingSpot> { spot });
            var manager = new ReservationManager(mockDb.Object, mockNotification.Object);
            var reservation = new Reservation(1, 1, spot, DateTime.Now, DateTime.Now.AddHours(1), 0);
            manager.MakeReservation(reservation);
            Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
        }

        // Błędny: Użytkownik zablokowany próbuje zarezerwować miejsce
        [Fact]
        public void MakeReservation_ShouldNotAdd_WhenUserIsBlocked()
        {
            var mockDb = new Mock<IDatabaseService>();
            var mockNotification = new Mock<NotificationService>(new StandardNotificationFactory());
            var user = new User(2, "blocked@example.com", "123456789", "pass");
            user.Activate();
            user.Block();
            var spot = new ParkingSpot(2, "B", "Z2");
            mockDb.Setup(d => d.GetUserById(2)).Returns(user);
            mockDb.Setup(d => d.GetAllParkingSpots()).Returns(new List<ParkingSpot> { spot });
            var manager = new ReservationManager(mockDb.Object, mockNotification.Object);
            var reservation = new Reservation(2, 2, spot, DateTime.Now, DateTime.Now.AddHours(1), 0);
            manager.MakeReservation(reservation);
            Assert.NotEqual(ReservationStatus.Confirmed, reservation.Status);
        }

        // Błędny: Użytkownik nie istnieje
        [Fact]
        public void MakeReservation_ShouldNotAdd_WhenUserDoesNotExist()
        {
            var mockDb = new Mock<IDatabaseService>();
            var mockNotification = new Mock<NotificationService>(new StandardNotificationFactory());
            mockDb.Setup(d => d.GetUserById(3)).Returns((User)null);
            var spot = new ParkingSpot(3, "C", "Z3");
            mockDb.Setup(d => d.GetAllParkingSpots()).Returns(new List<ParkingSpot> { spot });
            var manager = new ReservationManager(mockDb.Object, mockNotification.Object);
            var reservation = new Reservation(3, 3, spot, DateTime.Now, DateTime.Now.AddHours(1), 0);
            manager.MakeReservation(reservation);
            Assert.NotEqual(ReservationStatus.Confirmed, reservation.Status);
        }

        // Błędny: Próba rezerwacji zajętego miejsca (nakładanie się rezerwacji)
        [Fact]
        public void MakeReservation_ShouldSendNotification_WhenOverlapping()
        {
            var mockDb = new Mock<IDatabaseService>();
            var notificationService = new NotificationService(new StandardNotificationFactory());
            var mockObserver = new Mock<IObserver>();
            bool notified = false;
            mockObserver.Setup(o => o.Update(It.Is<string>(msg => msg.Contains("niepowodzeniem")))).Callback(() => notified = true);
            notificationService.Attach(mockObserver.Object);
            var user = new User(4, "overlap@example.com", "123456789", "pass");
            user.Activate();
            var spot = new ParkingSpot(4, "D", "Z4");
            mockDb.Setup(d => d.GetUserById(4)).Returns(user);
            mockDb.Setup(d => d.GetAllParkingSpots()).Returns(new List<ParkingSpot> { spot });
            var manager = new ReservationManager(mockDb.Object, notificationService);
            var reservation1 = new Reservation(4, 4, spot, DateTime.Now, DateTime.Now.AddHours(1), 0);
            var reservation2 = new Reservation(5, 4, spot, DateTime.Now.AddMinutes(30), DateTime.Now.AddHours(2), 0);
            manager.MakeReservation(reservation1);
            manager.MakeReservation(reservation2);
            Assert.True(notified);
        }

        // Graniczny: Rezerwacja na granicy czasu (start == end)
        [Fact]
        public void MakeReservation_ShouldNotAdd_WhenStartEqualsEnd()
        {
            var mockDb = new Mock<IDatabaseService>();
            var mockNotification = new Mock<NotificationService>(new StandardNotificationFactory());
            var user = new User(5, "edge@example.com", "123456789", "pass");
            user.Activate();
            var spot = new ParkingSpot(5, "E", "Z5");
            mockDb.Setup(d => d.GetUserById(5)).Returns(user);
            mockDb.Setup(d => d.GetAllParkingSpots()).Returns(new List<ParkingSpot> { spot });
            var manager = new ReservationManager(mockDb.Object, mockNotification.Object);
            var now = DateTime.Now;
            var reservation = new Reservation(6, 5, spot, now, now, 0);
            manager.MakeReservation(reservation);
            Assert.NotEqual(ReservationStatus.Confirmed, reservation.Status);
        }
    }
} 