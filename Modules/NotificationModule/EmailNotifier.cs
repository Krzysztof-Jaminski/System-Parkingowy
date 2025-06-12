using System;

namespace System_Parkingowy.Modules.NotificationModule
{
    // Abstract Factory
    public class EmailNotifier : INotifier
    {
        public void SendMessage(string recipientEmail, string messageContent)
        {
            Console.WriteLine($"[NotificationModule] Wysłano e-mail do \"{recipientEmail}\" : \"{messageContent}\"");
        }
    }
} 