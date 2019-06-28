using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Primary.Data;

namespace Primary.Tests
{
    class OrdersTests
    {
        [OneTimeSetUp]
        public async Task Login()
        {
            _api = new Api(Api.DemoEndpoint);
            await _api.Login(Api.DemoUsername, Api.DemoPassword);
        }

        [Test]
        [Ignore("WIP")]
        public async Task OrdersCanBeEnteredAndRetrieved()
        {
            var instruments = await _api.GetAllInstruments();
            var dollarFuture = instruments.First(c => c.Symbol.StartsWith("DO"));

            var order = new Order()
            {
                Symbol = dollarFuture.Symbol,
                Expiration = OrderExpiration.Day,
                Type = OrderType.Limit,
                Account = Api.DemoAccount,
            };

            var orderId = await _api.SubmitOrder(order);
            Assert.That(orderId, Is.Not.Null.And.Not.Empty);

            var retrievedOrder = await _api.GetOrder(orderId);
            Assert.That(retrievedOrder.Symbol, Is.EqualTo(order.Symbol));
            Assert.That(retrievedOrder.Expiration, Is.EqualTo(order.Expiration));
            Assert.That(retrievedOrder.Type, Is.EqualTo(order.Type));
            Assert.That(retrievedOrder.Account, Is.EqualTo(order.Account));
        }

        private Api _api;
    }
}
