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
        public async Task OrdersCanBeEnteredAndRetrieved()
        {
            var order = new Order();
            var orderId = await _api.SubmitOrder(order);
            Assert.That(orderId, Is.Not.Null.And.Not.Empty);

            var retrievedOrder = await _api.GetOrder(orderId);
            Assert.That(retrievedOrder, Is.Not.EqualTo( default(Order) ));
        }

        private Api _api;
    }
}
