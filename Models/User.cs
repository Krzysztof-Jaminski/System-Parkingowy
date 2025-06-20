using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace Models
{
    public enum UserStatus
    {
        Pending,
        Active,
        Blocked,
        Deleted
    }

    // Reprezentuje użytkownika systemu / kierowcę
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public UserStatus Status { get; set; }
        public string Role { get; set; } // "Driver" lub "Admin"
        public ICollection<Reservation> Reservations { get; set; }

        public User() {}

        public User(int id, string email, string phoneNumber, string password)
        {
            Id = id;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Status = UserStatus.Pending;
        }

        public void Activate()
        {
            if (Status != UserStatus.Pending && Status != UserStatus.Blocked)
                throw new System.InvalidOperationException("Można aktywować tylko użytkownika oczekującego lub zablokowanego.");
            Status = UserStatus.Active;
        }

        public void Block()
        {
            if (Status != UserStatus.Active)
                throw new System.InvalidOperationException("Można zablokować tylko aktywnego użytkownika.");
            Status = UserStatus.Blocked;
        }

        public void Unblock()
        {
            if (Status != UserStatus.Blocked)
                throw new System.InvalidOperationException("Można odblokować tylko zablokowanego użytkownika.");
            Status = UserStatus.Active;
        }

        public void Delete()
        {
            if (Status == UserStatus.Deleted)
                throw new System.InvalidOperationException("Użytkownik już usunięty.");
            Status = UserStatus.Deleted;
        }

        public void Deactivate()
        {
            if (Status != UserStatus.Active)
                throw new System.InvalidOperationException("Można dezaktywować tylko aktywnego użytkownika.");
            Status = UserStatus.Pending;
        }
    }

    public class UserListResponseExample : IExamplesProvider<IEnumerable<User>>
    {
        public IEnumerable<User> GetExamples() => new List<User>
        {
            new User { Id = 1, Email = "adam1@gmail.com", PhoneNumber = "123456789", Password = "passwd123", Status = UserStatus.Active },
            new User { Id = 2, Email = "anna2@gmail.com", PhoneNumber = "987654321", Password = "passwd22", Status = UserStatus.Active }
        };
    }

    public class UserResponseExample : IExamplesProvider<User>
    {
        public User GetExamples() => new User { Id = 1, Email = "adam1@gmail.com", PhoneNumber = "123456789", Password = "passwd123", Status = UserStatus.Active };
    }
}