using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Primary.Data;

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
            var instruments = await _api.GetAllInstruments();

            var order = new Order
            {
                Symbol = instruments.First().Symbol,
                Expiration = OrderExpiration.Day,
                Type = OrderType.Limit
            };
            var orderId = await _api.SubmitOrder(Api.DemoAccount, order);
            Assert.That( orderId, Is.Not.EqualTo( default(ulong) ) );

            var retrievedOrder = await _api.GetOrder(orderId);
            Assert.That(retrievedOrder.Symbol, Is.EqualTo(order.Symbol));
            Assert.That(retrievedOrder.Expiration, Is.EqualTo(order.Expiration));
            Assert.That(retrievedOrder.Type, Is.EqualTo(order.Type));
        }

        [Test]
        public void SubmittingAnOrderWithInvalidInformationGeneratesAnException()
        {
            const string invalidSymbol = "invalid_symbol";
            var order = new Order { Symbol = invalidSymbol };

            var exception = Assert.ThrowsAsync<Exception>( async () => await _api.SubmitOrder(Api.DemoAccount, order) );
            Assert.That(exception.Message, Does.Contain(invalidSymbol));
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
