using NUnit.Framework;
using Primary.Data;
using Primary.Data.Orders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Primary.Tests
{
    [TestFixture]
    internal class OrdersTests : TestWithApi
    {
        [Test]
        public async Task OrdersCanBeEnteredAndRetrieved()
        {
            // Submit an order
            Order order = Build.AnOrder(Api);
            var orderId = await Api.SubmitOrder(Api.DemoAccount, order);
            Assert.That(orderId, Is.Not.EqualTo(default(ulong)));

            // Retrieve the order
            var retrievedOrder = await Api.GetOrderStatus(orderId);

            Assert.That(retrievedOrder.InstrumentId.Symbol, Is.EqualTo(order.InstrumentId.Symbol));
            Assert.That(retrievedOrder.Expiration, Is.EqualTo(order.Expiration));
            Assert.That(retrievedOrder.Type, Is.EqualTo(order.Type));
            Assert.That(retrievedOrder.Quantity, Is.EqualTo(order.Quantity));
            Assert.That(retrievedOrder.Price, Is.EqualTo(order.Price));
            Assert.That(retrievedOrder.Side, Is.EqualTo(order.Side));
        }

        [Test]
        [Timeout(10000)]
        public async Task OrdersCanBeCancelled()
        {
            Order order = Build.AnOrder(Api);
            var orderId = await Api.SubmitOrder(Api.DemoAccount, order);

            var retrievedOrder = await Api.GetOrderStatus(orderId);
            Assert.That(retrievedOrder.Status, Is.Not.EqualTo(Status.Cancelled));

            await Api.CancelOrder(orderId);

            for (int i = 0; i < 4; i++)
            {
                if (retrievedOrder.Status != Status.Cancelled)
                {
                    retrievedOrder = await Api.GetOrderStatus(orderId);
                    Thread.Sleep(1000);
                }
            }
            Assert.That(retrievedOrder.Status, Is.EqualTo(Status.Cancelled), retrievedOrder.StatusText);
        }

        [Test]
        public void SubmittingAnOrderWithInvalidInformationGeneratesAnException()
        {
            var invalidInstrument = new Instrument()
            {
                Symbol = "invalid_symbol",
                Market = "invalid_market"
            };
            var order = new Order { InstrumentId = invalidInstrument };

            var exception = Assert.ThrowsAsync<Exception>(async () => await Api.SubmitOrder(Api.DemoAccount, order));
            Assert.That(exception.Message, Does.Contain(invalidInstrument.Symbol));
        }

        [Test]
        public void GettingAnOrderWithInvalidInformationGeneratesAnException()
        {
            var invalidOrderId = new OrderId()
            {
                ClientOrderId = "invalid_id",
                Proprietary = "invalid_proprietary"
            };

            var exception = Assert.ThrowsAsync<Exception>(async () => await Api.GetOrderStatus(invalidOrderId));
            Assert.That(exception.Message, Does.Contain(invalidOrderId.ClientOrderId.ToString()));
            Assert.That(exception.Message, Does.Contain(invalidOrderId.Proprietary));
        }
    }
}
