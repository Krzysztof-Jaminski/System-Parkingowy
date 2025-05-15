using System_Parkingowy.Modules.DatabaseModule;

namespace System_Parkingowy.Modules.AuthModule
{
    // Interfejs usługi autoryzacji
    public interface IAuthService
    {
        void Register(UserData data);       // Rejestracja użytkownika
        string Login(UserData data);        // Logowanie użytkownika
        void Verify(string email);          // Weryfikacja użytkownika
    }
}