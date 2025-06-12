using System;

namespace System_Parkingowy.Modules.NotificationModule
{
    public class NotificationService
    {
        private readonly INotificationFactory _notificationFactory;

        public NotificationService(INotificationFactory notificationFactory)
        {
            _notificationFactory = notificationFactory;
        }

        public void SendNotifications(string recipientIdentifier, string messageContent, NotificationType type)
        {
            INotifier notifier;
            switch (type)
            {
                case NotificationType.Email:
                    notifier = _notificationFactory.CreateEmailNotifier();
                    break;
                case NotificationType.Sms:
                    notifier = _notificationFactory.CreateSmsNotifier();
                    break;
                case NotificationType.Push:
                    notifier = _notificationFactory.CreatePushNotifier();
                    break;
                default:
                    throw new ArgumentException("Nieznany typ powiadomienia.");
            }
            notifier.SendMessage(recipientIdentifier, messageContent);
        }
    }
}