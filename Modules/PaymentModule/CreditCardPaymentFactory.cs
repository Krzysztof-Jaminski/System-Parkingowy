namespace System_Parkingowy.Modules.PaymentModule
{
    // Konkretna fabryka dla płatności kartą kredytową, implementująca metodę fabrykującą
    public class CreditCardPaymentFactory : PaymentFactory
    {
        public override IPayment CreatePayment()
        {
            return new CreditCardPayment();
        }
    }
} 