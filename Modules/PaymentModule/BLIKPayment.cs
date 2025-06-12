using System;

namespace System_Parkingowy.Modules.PaymentModule
{
    public class BLIKPayment : IPayment
    {
        public void Process()
        {
            Console.WriteLine("Przetwarzanie płatności BLIK.");
        }
    }
} 