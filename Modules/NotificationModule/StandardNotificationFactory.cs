using System.Collections.Generic;

namespace System_Parkingowy.Modules.NotificationModule
{
    public class StandardNotificationFactory : INotificationFactory
    {
        public INotifier CreateEmailNotifier()
        {
            return new EmailNotifier();
        }

        public INotifier CreateSmsNotifier()
        {
            return new SmsNotifier();
        }

        public INotifier CreatePushNotifier()
        {
            return new PushNotifier();
        }

        public void Send(string recipient, string messageContent)
        {
            CreateEmailNotifier().SendMessage(recipient, messageContent);
            CreateSmsNotifier().SendMessage(recipient, messageContent);
            CreatePushNotifier().SendMessage(recipient, messageContent);
        }

        public List<INotifier> CreateAllNotifiers()
        {
            return new List<INotifier> { new EmailNotifier(), new SmsNotifier(), new PushNotifier() };
        }
    }
} 