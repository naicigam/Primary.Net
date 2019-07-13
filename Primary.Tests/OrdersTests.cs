using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Primary.Data;
using Primary.Data.Orders;

namespace Primary.Tests
{
    internal class OrdersTests
    {
        [OneTimeSetUp]
        public async Task Login()
        {
            _api = new Api(Api.DemoEndpoint);
            await _api.Login(Api.DemoUsername, Api.DemoPassword);
        }

        [Test]
        public async Task OrdersCanBeEnteredAndRetrieved()
        {
            // Submit an order
            var instruments = await _api.GetAllInstruments();
            
            var order = new Order
            {
                Instrument = instruments.First(),
                Expiration = OrderExpiration.ImmediateOrCancel,
                Type = OrderType.Limit,
                Quantity = 1000,
                Price = 3
            };
            var orderId = await _api.SubmitOrder(Api.DemoAccount, order);
            Assert.That( orderId, Is.Not.EqualTo( default(ulong) ) );

            // Retrieve the order
            var retrievedOrder = await _api.GetOrder(orderId);

            Assert.That(retrievedOrder.Symbol, Is.EqualTo(order.Symbol));
            Assert.That(retrievedOrder.Expiration, Is.EqualTo(order.Expiration));
            Assert.That(retrievedOrder.Type, Is.EqualTo(order.Type));
            Assert.That(retrievedOrder.Quantity, Is.EqualTo(order.Quantity));
            Assert.That(retrievedOrder.Price, Is.EqualTo(order.Price));
        }

        [Test]
        public void SubmittingAnOrderWithInvalidInformationGeneratesAnException()
        {
            var invalidInstrument = new Instrument()
            {
                Symbol = "invalid_symbol",
                Market = "invalid_market"
            };
            var order = new Order { Instrument = invalidInstrument };

            var exception = Assert.ThrowsAsync<Exception>( async () => await _api.SubmitOrder(Api.DemoAccount, order) );
            Assert.That(exception.Message, Does.Contain(invalidInstrument.Symbol));
        }

        [Test]
        public void GettingAnOrderWithInvalidInformationGeneratesAnException()
        {
            var invalidOrderId = new OrderId()
            {
                ClientId = ulong.MaxValue,
                Proprietary = "invalid_proprietary"
            };

            var exception = Assert.ThrowsAsync<Exception>( async () => await _api.GetOrder(invalidOrderId) );
            Assert.That(exception.Message, Does.Contain(invalidOrderId.ClientId.ToString()));
            Assert.That(exception.Message, Does.Contain(invalidOrderId.Proprietary));
        }

        private Api _api;
    }
}
