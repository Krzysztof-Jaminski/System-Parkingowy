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

        public void Send(string recipientEmail, string messageContent)
        {
            CreateEmailNotifier().SendMessage(recipientEmail, messageContent);
            CreateSmsNotifier().SendMessage(recipientEmail, messageContent);
            CreatePushNotifier().SendMessage(recipientEmail, messageContent);
        }
    }
} 