using System;

namespace System_Parkingowy.Modules.NotificationModule
{
    // Usługa wysyłania e-maili
    public class NotificationService : INotification
    {
        public void SendVerificationEmail(string email)
        {
            Console.WriteLine($"[NotificationService] Wysłano e-mail weryfikacyjny na adres: {email}");
        }
    }
}