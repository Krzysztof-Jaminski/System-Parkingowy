namespace System_Parkingowy.Modules.NotificationModule
{
    public interface INotifier
    {
        void SendMessage(string recipientEmail, string messageContent); // Deklaruje metodę do wysyłania wiadomości
    }
} 