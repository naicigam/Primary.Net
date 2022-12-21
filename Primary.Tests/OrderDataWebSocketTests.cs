using NUnit.Framework;
using Primary.Data.Orders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Primary.Tests
{
    [TestFixture]
    internal class OrderDataWebSocketTests : TestWithApi
    {
        [Test]
        [Timeout(10000)]
        public async Task SubscriptionToOrdersDataCanBeCreated()
        {
            using var socket = Api.CreateOrderDataSocket(new[] { Api.DemoAccount });

            OrderStatus retrievedData = null;
            socket.OnData = ((api, orderData) => retrievedData = orderData.OrderReport);
            await socket.Start();

            // Send order
            Order order = Build.AnOrder(Api);
            await Api.SubmitOrder(Api.DemoAccount, order);

            // Wait until data arrives
            while (retrievedData == null)
            {
                Thread.Sleep(100);
            }

            Assert.That(retrievedData.Account.Id, Is.EqualTo(Api.DemoAccount));
            Assert.That(retrievedData.InstrumentId.Symbol, Is.EqualTo(order.InstrumentId.Symbol));
            Assert.That(retrievedData.InstrumentId.Market, Is.EqualTo(order.InstrumentId.Market));
            Assert.That(retrievedData.Price, Is.EqualTo(order.Price));
            Assert.That(retrievedData.TransactionTime, Is.Not.EqualTo(default(long)));
        }

        [Test]
        [Timeout(10000)]
        public async Task SubscriptionToOrdersDataCanBeCancelled()
        {
            // Used to cancel the task
            using var cancellationSource = new CancellationTokenSource();

            // Create and start the web socket
            using var socket = Api.CreateOrderDataSocket(new[] { Api.DemoAccount }, cancellationSource.Token);
            Assert.That(!socket.IsRunning);
            socket.OnData += ((api, orderData) => { });

            var socketTask = await socket.Start();

            // Wait until it is running
            while (!socket.IsRunning)
            {
                Thread.Sleep(10);
            }

            cancellationSource.Cancel();

            try
            {
                await socketTask;
                Assert.Fail();
            }
            catch (OperationCanceledException)
            {
                Assert.That(!socket.IsRunning);
            }
        }

        [Test]
        [Timeout(10000)]
        public void SubscriptionToOrdersCannotBeStartedUnlessDataCallbackIsProvided()
        {
            using var socket = Api.CreateOrderDataSocket(new[] { Api.DemoAccount });
            socket.OnData += null;

            var exception = Assert.ThrowsAsync<Exception>(socket.Start);
            Assert.That(exception.Message, Does.Contain(ErrorMessages.CallbackNotSet));
        }

        [Test]
        [Timeout(10000)]
        public async Task TryingToStartSocketWithAnInvalidAccountTriggersAnError()
        {
            var invalidAccount = Build.RandomString();

            using var socket = Api.CreateOrderDataSocket(new[] { invalidAccount });
            socket.OnData += ((api, orderData) => { });

            var socketTask = await socket.Start();
            while (!socket.IsRunning)
            {
                Thread.Sleep(10);
            }

            var exception = Assert.ThrowsAsync<Exception>(async () => { await socketTask; });
            Assert.That(exception.Message, Does.Contain(invalidAccount));
        }
    }
}