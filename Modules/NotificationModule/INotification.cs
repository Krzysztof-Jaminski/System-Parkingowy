namespace System_Parkingowy.Modules.NotificationModule
{
    // Interfejs do wysyłania e-maili
    public interface INotification
    {
        void SendVerificationEmail(string email);
    }
}