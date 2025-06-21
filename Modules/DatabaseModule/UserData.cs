using Swashbuckle.AspNetCore.Filters;

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

    public class UserDataExample : IExamplesProvider<UserData>
    {
        public UserData GetExamples() => new UserData("example@email.com", "Example123");
    }

    public class AuthRegisterResponseExample : IExamplesProvider<string>
    {
        public string GetExamples() => "User registered";
    }

    public class AuthLoginResponseExample : IExamplesProvider<string>
    {
        public string GetExamples() => "[AuthModule] Logowanie udane.";
    }
}