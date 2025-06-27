using System;

namespace System_Parkingowy.Modules.NotificationModule
{
    // Abstract Factory
    public class EmailNotifier : INotifier
    {
        public void SendMessage(string recipientEmail, string messageContent)
        {
            Console.WriteLine($"[NotificationModule] Email sent to '{recipientEmail}': '{messageContent}'");
        }

        public void Update(string message)
        {
            SendMessage("default@recipient.com", message);
        }
    }
} 