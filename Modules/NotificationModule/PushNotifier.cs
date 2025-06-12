using System;

namespace System_Parkingowy.Modules.NotificationModule
{
    public class PushNotifier : INotifier
    {
        public void SendMessage(string recipientEmail, string messageContent)
        {
            Console.WriteLine($"[NotificationModule] Wysłano powiadomienie Push do \"{recipientEmail}\" : \"{messageContent}\"");
        }
    }
} 