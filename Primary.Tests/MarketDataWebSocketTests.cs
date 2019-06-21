using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Primary.Data;

namespace Primary.Tests
{
    [TestFixture]
    internal class MarketDataWebSocketTests
    {
        [OneTimeSetUp]
        public async Task Login()
        {
            _api = new Api(Api.DemoEndpoint);
            await _api.Login(Api.DemoUsername, Api.DemoPassword);
        }

        private Api _api;

        [Test]
        public async Task SubscriptionToMarketDataCanBeCreated()
        {
            // Get a dollar future
            var allIInstruments = await _api.GetAllInstruments();
            var instrument = allIInstruments.First(c => c.Symbol.StartsWith("DO"));

            // Subscribe to bids and offers
            var entries = new[] { Entry.Bids, Entry.Offers };

            using (var socket = _api.CreateSocket(new[] {instrument}, entries, 1, 1))
            {
                MarketData retrievedData = null;
                socket.OnMarketData = (marketData => retrievedData = marketData);
                await socket.Start();
                
                // Wait until data arrives
                while (retrievedData == null)
                {
                    Thread.Sleep(100);
                }

                Assert.That(retrievedData.Data, Is.Not.Empty);
                Assert.That(retrievedData.Instrument.Market, Is.Not.Null.And.Not.Empty);
                Assert.That(retrievedData.Instrument.Symbol, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public async Task SubscriptionToMarketDataCanBeCancelled()
        {
            // Get a dollar future
            var allIInstruments = await _api.GetAllInstruments();
            var instrument = allIInstruments.First(c => c.Symbol.StartsWith("DO"));

            // Subscribe to bids and offers
            var entries = new[] { Entry.Bids, Entry.Offers };

            using (var socket = _api.CreateSocket(new[] {instrument}, entries, 1, 1))
            {
                Assert.That(!socket.IsRunning);

                var socketTask = socket.Start();

                // Wait until it is running
                while (!socket.IsRunning)
                {
                    Thread.Sleep(10);
                }

                socket.Cancel();
                socketTask.Wait();

                Assert.That(!socket.IsRunning);
            }
        }
    }
}
