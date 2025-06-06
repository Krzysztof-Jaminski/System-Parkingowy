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
        public int Id { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public string Password { get; }
        public UserStatus Status { get; private set; }

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
    }
}