namespace System_Parkingowy.Modules.NotificationModule
{
    public interface INotificationFactory
    {
        INotifier CreateEmailNotifier();
        INotifier CreateSmsNotifier();
        INotifier CreatePushNotifier();
    }
} 