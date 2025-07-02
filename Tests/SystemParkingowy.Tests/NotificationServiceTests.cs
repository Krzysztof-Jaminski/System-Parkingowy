using Moq;
using Xunit;
using System_Parkingowy.Modules.NotificationModule;

namespace SystemParkingowy.Tests
{
    public class NotificationServiceTests
    {
        // Normalny: Wysłanie powiadomienia do jednego odbiorcy
        [Fact]
        public void SendNotifications_ShouldCallNotify()
        {
            var mockFactory = new Mock<INotificationFactory>();
            var service = new NotificationService(mockFactory.Object);
            var wasCalled = false;
            var observer = new Mock<IObserver>();
            observer.Setup(o => o.Update(It.IsAny<string>())).Callback(() => wasCalled = true);
            service.Attach(observer.Object);
            service.SendNotifications("test@example.com", "Test message", NotificationType.Email);
            Assert.True(wasCalled);
        }

        // Błędny: Próba wysłania powiadomienia do pustego odbiorcy
        [Fact]
        public void SendNotifications_ShouldNotThrow_WhenRecipientIsEmpty()
        {
            var mockFactory = new Mock<INotificationFactory>();
            var service = new NotificationService(mockFactory.Object);
            var observer = new Mock<IObserver>();
            service.Attach(observer.Object);
            var ex = Record.Exception(() => service.SendNotifications("", "Test message", NotificationType.Email));
            Assert.Null(ex); // Nie powinno rzucać wyjątku
        }

        // Błędny: Próba wysłania pustej wiadomości
        [Fact]
        public void SendNotifications_ShouldNotThrow_WhenMessageIsEmpty()
        {
            var mockFactory = new Mock<INotificationFactory>();
            var service = new NotificationService(mockFactory.Object);
            var observer = new Mock<IObserver>();
            service.Attach(observer.Object);
            var ex = Record.Exception(() => service.SendNotifications("test@example.com", "", NotificationType.Email));
            Assert.Null(ex); // Nie powinno rzucać wyjątku
        }

        // Normalny: Wysłanie powiadomienia do wielu odbiorców
        [Fact]
        public void SendNotifications_ShouldNotifyAllObservers()
        {
            var mockFactory = new Mock<INotificationFactory>();
            var service = new NotificationService(mockFactory.Object);
            var observer1 = new Mock<IObserver>();
            var observer2 = new Mock<IObserver>();
            bool called1 = false, called2 = false;
            observer1.Setup(o => o.Update(It.IsAny<string>())).Callback(() => called1 = true);
            observer2.Setup(o => o.Update(It.IsAny<string>())).Callback(() => called2 = true);
            service.Attach(observer1.Object);
            service.Attach(observer2.Object);
            service.SendNotifications("test@example.com", "Test message", NotificationType.Email);
            Assert.True(called1 && called2);
        }

        // Błędny: Notyfikator rzuca wyjątek
        [Fact]
        public void SendNotifications_ShouldHandleObserverException()
        {
            var mockFactory = new Mock<INotificationFactory>();
            var service = new NotificationService(mockFactory.Object);
            var observer = new Mock<IObserver>();
            observer.Setup(o => o.Update(It.IsAny<string>())).Throws(new System.Exception("Observer error"));
            service.Attach(observer.Object);
            var ex = Record.Exception(() => service.SendNotifications("test@example.com", "Test message", NotificationType.Email));
            Assert.Null(ex); // Powinno obsłużyć wyjątek
        }
    }
} 