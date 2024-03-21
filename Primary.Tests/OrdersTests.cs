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
        public async Task ActiveOrdersCanBeRetrieved()
        {
            // Submit some orders
            Order order = Build.AnOrder(Api);
            var orderId = await Api.SubmitOrder(Api.DemoAccount, order);

            Order anotherOrder = Build.AnOrder(Api);
            var anotherOrderId = await Api.SubmitOrder(Api.DemoAccount, anotherOrder);

            Order otherToBeCancelled = Build.AnOrder(Api);
            var orderToBeCancelledId = await Api.SubmitOrder(Api.DemoAccount, otherToBeCancelled);
            await Api.CancelOrder(orderToBeCancelledId);

            // Retrieve active orders
            var activeOrderStatuses = await Api.GetActiveOrderStatuses(Api.DemoAccount);
            Assert.That(activeOrderStatuses, Has.One.Matches<OrderStatus>(o => o.ClientOrderId == orderId.ClientOrderId));
            Assert.That(activeOrderStatuses, Has.One.Matches<OrderStatus>(o => o.ClientOrderId == anotherOrderId.ClientOrderId));
            Assert.That(activeOrderStatuses, Has.None.Matches<OrderStatus>(o => o.ClientOrderId == orderToBeCancelledId.ClientOrderId));
        }

        [Test]
        public async Task OrderSizeAndPriceCanBeUpdated()
        {
            // Submit an order
            Order order = Build.AnOrder(Api);
            var orderId = await Api.SubmitOrder(Api.DemoAccount, order);

            var orderStatus = await Api.GetOrderStatus(orderId);
            Assert.That(orderStatus.Quantity, Is.EqualTo(order.Quantity));
            Assert.That(orderStatus.Price, Is.EqualTo(order.Price));

            var anotherQuantity = order.Quantity + 1;
            var anotherPrice = order.Price + 1;

            var replacedOrderId = await Api.UpdateOrder(orderId, anotherQuantity, anotherPrice);

            orderStatus = await Api.GetOrderStatus(orderId);
            Assert.That(orderStatus.Status, Is.Not.EqualTo(Status.Filled));

            var replacedOrderStatus = await Api.GetOrderStatus(replacedOrderId);
            Assert.That(replacedOrderStatus.Quantity, Is.EqualTo(anotherQuantity));
            Assert.That(replacedOrderStatus.Price, Is.EqualTo(anotherPrice));
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
            Assert.That(exception.Message, Does.Contain(invalidOrderId.ClientOrderId));
            Assert.That(exception.Message, Does.Contain(invalidOrderId.Proprietary));
        }

        [Test]
        public void UpdatingAnOrderWithInvalidInformationGeneratesAnException()
        {
            var order = new Order { ClientOrderId = "invalid_id", Proprietary = "invalid_proprietary" };

            var exception = Assert.ThrowsAsync<Exception>(async () => await Api.UpdateOrder(order, 0, 0));
            Assert.That(exception.Message, Does.Contain(order.ClientOrderId));
        }

        [Test]
        public void CancellingAnOrderWithInvalidInformationGeneratesAnException()
        {
            var order = new Order { ClientOrderId = "invalid_id", Proprietary = "invalid_proprietary" };

            var exception = Assert.ThrowsAsync<Exception>(async () => await Api.CancelOrder(order));
            Assert.That(exception.Message, Does.Contain(order.ClientOrderId));
        }

        [Test]
        public void GettingActiveOrdersWithInvalidAccountGeneratesAnException()
        {
            const string invalidAccountId = "invalid_account_id";
            Assert.ThrowsAsync<Exception>(async () => await Api.GetActiveOrderStatuses(invalidAccountId));
        }
    }
}
