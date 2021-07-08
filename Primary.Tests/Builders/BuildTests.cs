using System.Threading.Tasks;
using NUnit.Framework;
using Primary.Data.Orders;

namespace Primary.Tests.Builders
{
    [TestFixture]
    internal class BuildTests : TestWithApi
    {
        #region API builder

        [Test]
        public async Task ApiIsBuiltLoggedIn()
        {
            var api = await Build.AnApi();
            Assert.That(api.AccessToken, Is.Not.Null.And.Not.Empty);
        }

        #endregion

        [Test]
        public async Task OrderCanBeBuildReadyToBeSubmitted()
        {
            // Submit an order
            var order = Build.AnOrder(Api);
            var orderId = await Api.SubmitOrder(Api.DemoAccount, order);
            Assert.That( orderId, Is.Not.EqualTo( default(ulong) ) );

            // Retrieve the order
            var retrievedOrder = await Api.GetOrderStatus(orderId);

            Assert.That(retrievedOrder.Status, 
                        Is.Not.EqualTo(Status.Rejected).And.Not.EqualTo(Status.Cancelled), 
                        retrievedOrder.StatusText
            );
        }
    }
}
