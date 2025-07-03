using System;
using Moq;
using Xunit;
using System_Parkingowy.Modules.PaymentModule;

namespace SystemParkingowy.Tests
{
    public class PaymentProcessorTests
    {
        // Normalny: Prawidłowe przetworzenie płatności
        [Fact]
        public void ProcessPayment_ShouldCallProcessOnPayment()
        {
            var mockFactory = new Mock<PaymentFactory>();
            var mockPayment = new Mock<IPayment>();
            mockFactory.Setup(f => f.CreatePayment()).Returns(mockPayment.Object);
            var processor = new PaymentProcessor(mockFactory.Object);
            processor.ProcessPayment();
            mockPayment.Verify(p => p.Process(), Times.Once);
        }

        // Błędny: CreatePayment zwraca null
        [Fact]
        public void ProcessPayment_ShouldThrow_WhenFactoryReturnsNull()
        {
            var mockFactory = new Mock<PaymentFactory>();
            mockFactory.Setup(f => f.CreatePayment()).Returns((IPayment)null);
            var processor = new PaymentProcessor(mockFactory.Object);
            Assert.Throws<NullReferenceException>(() => processor.ProcessPayment());
        }

        // Błędny: IPayment rzuca wyjątek
        [Fact]
        public void ProcessPayment_ShouldThrow_WhenPaymentThrows()
        {
            var mockFactory = new Mock<PaymentFactory>();
            var mockPayment = new Mock<IPayment>();
            mockPayment.Setup(p => p.Process()).Throws(new InvalidOperationException("Błąd płatności"));
            mockFactory.Setup(f => f.CreatePayment()).Returns(mockPayment.Object);
            var processor = new PaymentProcessor(mockFactory.Object);
            Assert.Throws<InvalidOperationException>(() => processor.ProcessPayment());
        }

        // Graniczny: Sprawdzenie, czy CreatePayment jest wywoływane
        [Fact]
        public void ProcessPayment_ShouldCallCreatePaymentOnFactory()
        {
            var mockFactory = new Mock<PaymentFactory>();
            var mockPayment = new Mock<IPayment>();
            mockFactory.Setup(f => f.CreatePayment()).Returns(mockPayment.Object);
            var processor = new PaymentProcessor(mockFactory.Object);
            processor.ProcessPayment();
            mockFactory.Verify(f => f.CreatePayment(), Times.Once);
        }

        // Graniczny: Wielokrotne wywołanie ProcessPayment
        [Fact]
        public void ProcessPayment_CanBeCalledMultipleTimes()
        {
            var mockFactory = new Mock<PaymentFactory>();
            var mockPayment = new Mock<IPayment>();
            mockFactory.Setup(f => f.CreatePayment()).Returns(mockPayment.Object);
            var processor = new PaymentProcessor(mockFactory.Object);
            processor.ProcessPayment();
            processor.ProcessPayment();
            mockPayment.Verify(p => p.Process(), Times.Exactly(2));
        }
    }
} 