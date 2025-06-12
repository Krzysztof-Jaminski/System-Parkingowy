namespace System_Parkingowy.Modules.PaymentModule
{
    // Interfejs wspólnego zachowania dla wszystkich metod płatności
    public interface IPayment
    {
        void Process();
    }
} 