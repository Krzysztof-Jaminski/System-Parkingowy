using System.Collections.Generic;

namespace System_Parkingowy.Modules.NotificationModule
{
    public interface INotificationFactory
    {
        INotifier CreateEmailNotifier();
        INotifier CreateSmsNotifier();
        INotifier CreatePushNotifier();
        List<INotifier> CreateAllNotifiers();
        void Send(string recipient, string messageContent);
    }
} 