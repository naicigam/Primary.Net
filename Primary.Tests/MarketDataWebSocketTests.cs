using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Primary.Data;

namespace Primary.Tests
{
    [TestFixture]
    internal class MarketDataWebSocketTests : TestWithApi
    {
        [Test]
        [Timeout(10000)]
        public async Task SubscriptionToMarketDataCanBeCreated()
        {
            var instrument = new Instrument()
            {
                Market = "ROFX",
                Symbol = Build.DollarFutureSymbol()
            };

            // Subscribe to market data
            using var socket = Api.CreateMarketDataSocket(new[] { instrument }, new[] { Entry.Close } , 1, 1);
            
            MarketData retrievedData = null;
            socket.OnData = ( (api, marketData) => retrievedData = marketData );
            await socket.Start();

            // Wait until data arrives
            while (retrievedData == null)
            {
                Thread.Sleep(100);
            }

            Assert.That(retrievedData.Instrument.Market, Is.Not.Null.And.Not.Empty);
            Assert.That(retrievedData.Instrument.Symbol, Is.Not.Null.And.Not.Empty);
            Assert.That(retrievedData.Timestamp, Is.Not.EqualTo(default(long)));

            var close = retrievedData.Data.Close;            
            Assert.That(close.Price, Is.Not.EqualTo(default(float)));
            Assert.That(close.DateTime, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        [Timeout(10000)]
        public async Task SubscriptionToMarketDataCanBeCancelled()
        {
            var instrument = new Instrument()
            {
                Market = "ROFX",
                Symbol = Build.DollarFutureSymbol()
            };

            // Subscribe to bids and offers
            var entries = new[] { Entry.Bids, Entry.Offers };

            // Used to cancel the task
            using var cancellationSource = new CancellationTokenSource();

            // Create and start the web socket
            using var socket = Api.CreateMarketDataSocket(new[] { instrument }, entries, 1, 1, cancellationSource.Token);
            Assert.That(!socket.IsRunning);

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
        [Ignore("ReMarkets does not push index data.")]
        [Timeout(10000)]
        public async Task SubscriptionToIndexMarketDataCanBeCreated()
        {
            // Get a dollar future
            var instruments = await Api.GetAllInstruments();
            var instrument = instruments.Last( i => i.Symbol == "I.RFX20" );

            // Subscribe to all entries
            var entries = new[] { Entry.IndexValue };
            using var socket = Api.CreateMarketDataSocket(new[] { instrument }, entries, 1, 1);
            
            MarketData retrievedData = null;
            socket.OnData = ( (api, marketData) => 
                                    retrievedData = (marketData.Data.IndexValue != null ? marketData : null)
            );
            await socket.Start();

            // Wait until data arrives
            while (retrievedData == null)
            {
                Thread.Sleep(100);
            }

            Assert.That(retrievedData.Instrument.Market, Is.Not.Null.And.Not.Empty);
            Assert.That(retrievedData.Instrument.Symbol, Is.Not.Null.And.Not.Empty);
            Assert.That(retrievedData.Timestamp, Is.Not.EqualTo(default(long)));

            Assert.That(retrievedData.Data.IndexValue, Is.Not.Null.And.Not.Empty);
        }

        public static Entry[] AllEntries = { 
            Entry.Bids,
            Entry.Offers,
            Entry.Last,
            Entry.Open,
            Entry.Close,
            Entry.SettlementPrice,
            Entry.SessionHighPrice,
            Entry.SessionLowPrice,
            Entry.Volume,
            Entry.OpenInterest,
            Entry.IndexValue,
            Entry.EffectiveVolume,
            Entry.NominalVolume
        };
    }
}
