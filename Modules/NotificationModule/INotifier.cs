using System_Parkingowy.Modules.NotificationModule;

namespace System_Parkingowy.Modules.NotificationModule
{
    public interface INotifier : IObserver
    {
        void SendMessage(string recipient, string messageContent);
    }
} 