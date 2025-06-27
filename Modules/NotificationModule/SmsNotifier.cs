using System;

namespace System_Parkingowy.Modules.NotificationModule
{
    public class SmsNotifier : INotifier
    {
        public void SendMessage(string recipientEmail, string messageContent)
        {
            Console.WriteLine($"[NotificationModule] SMS sent to '{recipientEmail}': '{messageContent}'");
        }

        public void Update(string message)
        {
            SendMessage("default@recipient.com", message);
        }
    }
} 