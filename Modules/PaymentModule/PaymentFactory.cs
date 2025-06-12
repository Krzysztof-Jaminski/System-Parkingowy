namespace System_Parkingowy.Modules.PaymentModule
{
    // Abstrakcyjna klasa fabrykująca, która deklaruje metodę fabrykującą
    public abstract class PaymentFactory
    {
        public abstract IPayment CreatePayment();
    }
} 