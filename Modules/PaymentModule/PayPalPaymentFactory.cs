namespace System_Parkingowy.Modules.PaymentModule
{
    // Konkretna fabryka dla płatności PayPal, implementująca metodę fabrykującą
    public class PayPalPaymentFactory : PaymentFactory
    {
        public override IPayment CreatePayment()
        {
            return new PayPalPayment();
        }
    }
} 