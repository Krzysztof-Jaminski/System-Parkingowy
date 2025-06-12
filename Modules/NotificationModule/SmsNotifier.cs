using System;

namespace System_Parkingowy.Modules.NotificationModule
{
    public class SmsNotifier : INotifier
    {
        public void SendMessage(string recipientEmail, string messageContent)
        {
            Console.WriteLine($"[NotificationModule] Wysłano SMS do \"{recipientEmail}\" : \"{messageContent}\""); 
        }
    }
} 