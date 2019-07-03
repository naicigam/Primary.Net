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
            Assert.That(retrievedOrder, Is.Not.EqualTo( default(Order) ));
        }

        [Test]
        public void SubmittingAndOrderWithInvalidInformationGeneratesAnException()
        {
            var order = new Order
            {
                Symbol = "invalid_symbol"
            };

            Assert.ThrowsAsync<Exception>( async () => await _api.SubmitOrder(Api.DemoAccount, order) );
        }

        private Api _api;
    }
}
