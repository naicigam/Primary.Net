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

            OrderStatus receivedOrderData = null;
            OrderId newOrderId = null;
            var receivedDataSemaphore = new SemaphoreSlim(0, 1);
            socket.OnData = ((api, orderData) =>
            {
                if (newOrderId != null && orderData.OrderReport.ClientOrderId == newOrderId.ClientOrderId)
                {
                    receivedOrderData = orderData.OrderReport;
                    receivedDataSemaphore.Release();
                }
            });
            await socket.Start();

            // Send order
            Order order = Build.AnOrder(Api);
            newOrderId = await Api.SubmitOrder(Api.DemoAccount, order);

            // Wait until data arrives
            receivedDataSemaphore.Wait();

            Assert.That(receivedOrderData.Account.Id, Is.EqualTo(Api.DemoAccount));
            Assert.That(receivedOrderData.ClientOrderId, Is.EqualTo(newOrderId.ClientOrderId));
            Assert.That(receivedOrderData.InstrumentId.Symbol, Is.EqualTo(order.InstrumentId.Symbol));
            Assert.That(receivedOrderData.InstrumentId.Market, Is.EqualTo(order.InstrumentId.Market));
            Assert.That(receivedOrderData.Price, Is.EqualTo(order.Price));
            Assert.That(receivedOrderData.TransactionTime, Is.Not.EqualTo(default(long)));
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

            var socketTask = socket.Start();

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
            socket.OnData = null;

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

            var socketTask = socket.Start();
            while (!socket.IsRunning)
            {
                Thread.Sleep(10);
            }

            var exception = Assert.ThrowsAsync<Exception>(async () => { await socketTask; });
            Assert.That(exception.Message, Does.Contain(invalidAccount));
        }

        [Test]
        [Timeout(10000)]
        public async Task OrdersCanBeSentAndIdentifiedUsingWebSocketClientOrderId()
        {
            using var socket = Api.CreateOrderDataSocket(new[] { Api.DemoAccount });

            OrderStatus receivedOrderData = null;
            string newOrderWebSocketClientId = Build.RandomString();
            var receivedDataSemaphore = new SemaphoreSlim(0, 1);
            socket.OnData = ((api, orderData) =>
            {
                if (orderData.OrderReport.WebSocketClientOrderId == newOrderWebSocketClientId)
                {
                    receivedOrderData = orderData.OrderReport;
                    receivedDataSemaphore.Release();
                }
            });
            await socket.Start();

            // Send order
            Order order = Build.AnOrder(Api);
            order.WebSocketClientOrderId = newOrderWebSocketClientId;

            socket.SubmitOrder(Api.DemoAccount, order);

            // Wait until data arrives
            receivedDataSemaphore.Wait();

            Assert.That(receivedOrderData.Account.Id, Is.EqualTo(Api.DemoAccount));
            Assert.That(receivedOrderData.InstrumentId.Symbol, Is.EqualTo(order.InstrumentId.Symbol));
            Assert.That(receivedOrderData.InstrumentId.Market, Is.EqualTo(order.InstrumentId.Market));
            Assert.That(receivedOrderData.Price, Is.EqualTo(order.Price));
            Assert.That(receivedOrderData.TransactionTime, Is.Not.EqualTo(default(long)));
        }

        [Test]
        [Timeout(10000)]
        public async Task OrdersCanBeCancelled()
        {
            using var socket = Api.CreateOrderDataSocket(new[] { Api.DemoAccount });

            OrderStatus receivedOrderData = default;
            var newOrderWebSocketClientId = Build.RandomString();
            var clientOrderId = string.Empty;
            var receivedDataSemaphore = new SemaphoreSlim(0, 1);
            socket.OnData = ((_, orderData) =>
            {
                // Get the new order id
                var order = orderData.OrderReport;
                if (order.WebSocketClientOrderId == newOrderWebSocketClientId)
                {
                    clientOrderId = order.ClientOrderId;
                }

                // Cancel the order
                if (order.ClientOrderId == clientOrderId)
                {
                    if (order.Status == Status.Cancelled)
                    {
                        receivedOrderData = order;
                        receivedDataSemaphore.Release();
                    }
                    else if (order.Status != Status.PendingCancel)
                    {
                        Task.Run(() => socket.CancelOrder(order));
                    }
                }
            });
            await socket.Start();

            // Send order
            Order order = Build.AnOrder(Api);
            order.WebSocketClientOrderId = newOrderWebSocketClientId;

            socket.SubmitOrder(Api.DemoAccount, order);

            // Wait until data arrives
            await receivedDataSemaphore.WaitAsync();

            Assert.That(receivedOrderData.Account.Id, Is.EqualTo(Api.DemoAccount));
            Assert.That(receivedOrderData.Status, Is.EqualTo(Status.Cancelled));
        }
    }
}