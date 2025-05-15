namespace System_Parkingowy.Modules.DatabaseModule
{
    // Dane użytkownika dostarczane przez interfejs użytkownika
    public class UserData
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserData(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}