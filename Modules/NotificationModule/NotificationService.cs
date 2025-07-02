using System;
using System.Collections.Generic;
using System_Parkingowy.Modules.NotificationModule;

namespace System_Parkingowy.Modules.NotificationModule
{
    public class NotificationService : ISubject
    {
        private readonly List<IObserver> _observers = new();
        private readonly INotificationFactory _notificationFactory;

        public NotificationService(INotificationFactory notificationFactory)
        {
            _notificationFactory = notificationFactory;
        }

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(string message)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.Update(message);
                }
                catch
                {
                    // Ignoruj wyjątki z observerów, można dodać logowanie
                }
            }
        }

        public void SendNotifications(string recipient, string messageContent, NotificationType type)
        {
            Notify($"[{type}] {recipient}: {messageContent}");
        }
    }
}