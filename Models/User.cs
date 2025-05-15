namespace Models
{
    // Reprezentuje użytkownika systemu / kierowce
    public class User
    {
        public string Email { get; }
        public string Password { get; }
        public bool IsVerified { get; private set; }

        public User(string email, string password)
        {
            Email = email;
            Password = password;
            IsVerified = false;
        }

        public void Verify()
        {
            IsVerified = true;
        }
    }
}