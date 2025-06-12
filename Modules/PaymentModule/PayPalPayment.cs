using System;

namespace System_Parkingowy.Modules.PaymentModule
{
    // Konkretna implementacja płatności PayPal
    public class PayPalPayment : IPayment
    {
        public void Process()
        {
            Console.WriteLine("Przetwarzanie płatności PayPal.");
        }
    }
} 