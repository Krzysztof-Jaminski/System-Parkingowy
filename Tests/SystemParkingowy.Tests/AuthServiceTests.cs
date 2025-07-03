using System;
using Moq;
using Xunit;
using System_Parkingowy.Modules.AuthModule;
using System_Parkingowy.Modules.DatabaseModule;
using System_Parkingowy.Modules.NotificationModule;
using System_Parkingowy.Models;

namespace SystemParkingowy.Tests
{
    public class AuthServiceTests
    {
        // Normalny: Rejestracja nowego użytkownika
        [Fact]
        public void Register_ShouldAddUser_WhenDataIsValid()
        {
            var mockDb = new Mock<IDatabaseService>();
            var mockNotification = new Mock<NotificationService>(new StandardNotificationFactory());
            mockDb.Setup(d => d.GetUserByEmail("new@example.com")).Returns((User)null);
            mockDb.Setup(d => d.GetNextUserId()).Returns(1);
            var service = new AuthService(mockDb.Object, mockNotification.Object);
            var data = new UserData("new@example.com", "Test123");
            service.Register(data);
            mockDb.Verify(d => d.AddUser(It.Is<User>(u => u.Email == "new@example.com")), Times.Once);
        }

        // Błędny: Rejestracja z istniejącym emailem
        [Fact]
        public void Register_ShouldNotAddUser_WhenEmailExists()
        {
            var mockDb = new Mock<IDatabaseService>();
            var mockNotification = new Mock<NotificationService>(new StandardNotificationFactory());
            mockDb.Setup(d => d.GetUserByEmail("exists@example.com")).Returns(new User(1, "exists@example.com", "", "pass"));
            var service = new AuthService(mockDb.Object, mockNotification.Object);
            var data = new UserData("exists@example.com", "Test123");
            service.Register(data);
            mockDb.Verify(d => d.AddUser(It.IsAny<User>()), Times.Never);
        }

        // Błędny: Rejestracja ze słabym hasłem
        [Fact]
        public void Register_ShouldNotAddUser_WhenPasswordIsWeak()
        {
            var mockDb = new Mock<IDatabaseService>();
            var mockNotification = new Mock<NotificationService>(new StandardNotificationFactory());
            mockDb.Setup(d => d.GetUserByEmail("weak@example.com")).Returns((User)null);
            var service = new AuthService(mockDb.Object, mockNotification.Object);
            var data = new UserData("weak@example.com", "abc");
            service.Register(data);
            mockDb.Verify(d => d.AddUser(It.IsAny<User>()), Times.Never);
        }

        // Normalny: Logowanie poprawne
        [Fact]
        public void Login_ShouldReturnSuccess_WhenCredentialsAreCorrectAndActive()
        {
            var mockDb = new Mock<IDatabaseService>();
            var user = new User(1, "login@example.com", "", "Test123");
            user.Activate();
            mockDb.Setup(d => d.GetUserByEmail("login@example.com")).Returns(user);
            var service = new AuthService(mockDb.Object, new Mock<NotificationService>(new StandardNotificationFactory()).Object);
            var data = new UserData("login@example.com", "Test123");
            var result = service.Login(data);
            Assert.Contains("Logowanie udane", result);
        }

        // Błędny: Logowanie z nieistniejącym emailem
        [Fact]
        public void Login_ShouldReturnError_WhenEmailDoesNotExist()
        {
            var mockDb = new Mock<IDatabaseService>();
            mockDb.Setup(d => d.GetUserByEmail("notfound@example.com")).Returns((User)null);
            var service = new AuthService(mockDb.Object, new Mock<NotificationService>(new StandardNotificationFactory()).Object);
            var data = new UserData("notfound@example.com", "Test123");
            var result = service.Login(data);
            Assert.Contains("Niepoprawny e-mail", result);
        }

        // Błędny: Logowanie z niepoprawnym hasłem
        [Fact]
        public void Login_ShouldReturnError_WhenPasswordIsWrong()
        {
            var mockDb = new Mock<IDatabaseService>();
            var user = new User(2, "wrongpass@example.com", "", "Test123");
            user.Activate();
            mockDb.Setup(d => d.GetUserByEmail("wrongpass@example.com")).Returns(user);
            var service = new AuthService(mockDb.Object, new Mock<NotificationService>(new StandardNotificationFactory()).Object);
            var data = new UserData("wrongpass@example.com", "Wrong");
            var result = service.Login(data);
            Assert.Contains("Niepoprawne hasło", result);
        }

        // Błędny: Logowanie z różnymi statusami użytkownika
        [Theory]
        [InlineData(UserStatus.Pending, "nie jest zweryfikowane")]
        [InlineData(UserStatus.Blocked, "jest zablokowane")]
        [InlineData(UserStatus.Deleted, "zostało usunięte")]
        public void Login_ShouldReturnError_WhenUserStatusIsNotActive(UserStatus status, string expectedMsg)
        {
            var mockDb = new Mock<IDatabaseService>();
            var user = new User(3, "status@example.com", "", "Test123");
            if (status == UserStatus.Pending) { /* domyślny */ }
            else if (status == UserStatus.Blocked) { user.Activate(); user.Block(); }
            else if (status == UserStatus.Deleted) { user.Delete(); }
            mockDb.Setup(d => d.GetUserByEmail("status@example.com")).Returns(user);
            var service = new AuthService(mockDb.Object, new Mock<NotificationService>(new StandardNotificationFactory()).Object);
            var data = new UserData("status@example.com", "Test123");
            var result = service.Login(data);
            Assert.Contains(expectedMsg, result);
        }

        // Normalny: Weryfikacja istniejącego użytkownika
        [Fact]
        public void Verify_ShouldActivateUser_WhenUserExists()
        {
            var mockDb = new Mock<IDatabaseService>();
            var user = new User(4, "verify@example.com", "", "Test123");
            // domyślnie Pending
            mockDb.Setup(d => d.GetUserByEmail("verify@example.com")).Returns(user);
            var service = new AuthService(mockDb.Object, new Mock<NotificationService>(new StandardNotificationFactory()).Object);
            service.Verify("verify@example.com");
            Assert.Equal(UserStatus.Active, user.Status);
        }

        // Błędny: Weryfikacja nieistniejącego użytkownika
        [Fact]
        public void Verify_ShouldNotThrow_WhenUserDoesNotExist()
        {
            var mockDb = new Mock<IDatabaseService>();
            mockDb.Setup(d => d.GetUserByEmail("notfound@example.com")).Returns((User)null);
            var service = new AuthService(mockDb.Object, new Mock<NotificationService>(new StandardNotificationFactory()).Object);
            var ex = Record.Exception(() => service.Verify("notfound@example.com"));
            Assert.Null(ex);
        }
    }
} 