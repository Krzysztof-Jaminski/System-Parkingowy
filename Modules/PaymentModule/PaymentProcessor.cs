using System;

namespace System_Parkingowy.Modules.PaymentModule
{
    // Klasa używająca fabryki do tworzenia obiektów płatności
    public class PaymentProcessor
    {
        private readonly PaymentFactory _factory;

        public PaymentProcessor(PaymentFactory factory)
        {
            _factory = factory;
        }

        public void ProcessPayment()
        {
            IPayment payment = _factory.CreatePayment();
            Console.Write("[PaymentModule] ");
            payment.Process();
        }
    }
} 