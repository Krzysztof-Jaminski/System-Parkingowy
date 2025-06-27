namespace System_Parkingowy.Modules.PaymentModule
{
    // Interfejs wspólnego zachowania dla wszystkich metod płatności
    // IPayment pełni rolę Adaptera (dla integracji) i i interfejsu strategii (różne sposoby realizacji płatności)
    public interface IPayment
    {
        void Process();
    }
} 