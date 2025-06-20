using Swashbuckle.AspNetCore.Filters;

namespace System_Parkingowy.Modules.PaymentModule
{
    // Abstrakcyjna klasa fabrykująca, która deklaruje metodę fabrykującą
    public abstract class PaymentFactory
    {
        public abstract IPayment CreatePayment();
    }

    public class PaymentResponseExample : IExamplesProvider<string>
    {
        public string GetExamples() => "Payment processed";
    }
} 