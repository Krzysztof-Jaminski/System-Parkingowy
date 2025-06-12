namespace System_Parkingowy.Modules.PaymentModule
{
    // Konkretna fabryka dla płatności BLIK, implementująca metodę fabrykującą
    public class BLIKPaymentFactory : PaymentFactory
    {
        public override IPayment CreatePayment()
        {
            return new BLIKPayment();
        }
    }
} 