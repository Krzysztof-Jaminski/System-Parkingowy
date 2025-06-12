using System;

namespace System_Parkingowy.Modules.PaymentModule
{
    // Konkretna implementacja płatności kartą kredytową
    public class CreditCardPayment : IPayment
    {
        public void Process()
        {
            Console.WriteLine("Przetwarzanie płatności kartą kredytową.");
        }
    }
} 