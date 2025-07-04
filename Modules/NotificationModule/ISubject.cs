namespace System_Parkingowy.Modules.NotificationModule
{
    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void Notify(string message);
    }
} 