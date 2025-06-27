using System;

namespace System_Parkingowy.Modules.PaymentModule
{
    // Adapter do integracji z zewnętrznym systemem
    public class PayUAdapter : IPayment
    {
        public void Process()
        {
            Console.WriteLine("Przetwarzanie płatności przez PayU (Adapter)");
            // Wywołanie zewnętrznego API PayU
        }
    }
} 