using System;

namespace System_Parkingowy.Modules.NotificationModule
{
    public class PushNotifier : INotifier
    {
        public void SendMessage(string recipientEmail, string messageContent)
        {
            Console.WriteLine($"[NotificationModule] Push notification sent to '{recipientEmail}': '{messageContent}'");
        }

        public void Update(string message)
        {
            SendMessage("default@recipient.com", message);
        }
    }
} 