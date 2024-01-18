﻿using NUnit.Framework;
using Primary.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Primary.Tests
{
    [TestFixture]
    internal class MarketDataWebSocketTests : TestWithApi
    {
        [Test]
        [Timeout(10000)]
        public async Task SubscriptionToMarketDataCanBeCreated()
        {
            var instrumentId = new InstrumentId()
            {
                Market = "ROFX",
                Symbol = Build.DollarFutureSymbol()
            };

            // Subscribe to market data
            using var socket = Api.CreateMarketDataSocket(new[] { instrumentId }, new[] { Entry.Close }, 1, 1);

            MarketData retrievedData = null;
            var dataReceived = new ManualResetEvent(false);
            socket.OnData = ((api, marketData) => { retrievedData = marketData; dataReceived.Set(); });
            await socket.Start();

            // Wait until data arrives
            dataReceived.WaitOne();

            Assert.That(retrievedData.InstrumentId.Market, Is.Not.Null.And.Not.Empty);
            Assert.That(retrievedData.InstrumentId.Symbol, Is.Not.Null.And.Not.Empty);
            Assert.That(retrievedData.Timestamp, Is.Not.EqualTo(default(long)));

            var close = retrievedData.Data.Close;
            Assert.That(close.Price, Is.Not.EqualTo(default(float)));
            Assert.That(close.DateTime, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        [Timeout(10000)]
        public async Task SubscriptionToMarketDataCanBeCancelled()
        {
            var instrumentId = new InstrumentId()
            {
                Market = "ROFX",
                Symbol = Build.DollarFutureSymbol()
            };

            // Subscribe to bids and offers
            var entries = new[] { Entry.Bids, Entry.Offers };

            // Used to cancel the task
            using var cancellationSource = new CancellationTokenSource();

            // Create and start the web socket
            using var socket = Api.CreateMarketDataSocket(new[] { instrumentId }, entries, 1, 1, cancellationSource.Token);
            socket.OnData += ((api, orderData) => { });

            Assert.That(!socket.IsRunning);

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
        [Ignore("ReMarkets does not push index data.")]
        [Timeout(10000)]
        public async Task SubscriptionToIndexMarketDataCanBeCreated()
        {
            // Get a dollar future
            var instruments = await Api.GetAllInstruments();
            var instrument = instruments.Last(i => i.Symbol == "I.RFX20");

            // Subscribe to all entries
            var entries = new[] { Entry.IndexValue };
            using var socket = Api.CreateMarketDataSocket(new[] { instrument }, entries, 1, 1);

            MarketData retrievedData = null;
            var dataReceived = new ManualResetEvent(false);
            socket.OnData = ((api, marketData) => { retrievedData = (marketData.Data.IndexValue != null ? marketData : null); dataReceived.Set(); });

            await socket.Start();

            // Wait until data arrives
            dataReceived.WaitOne();

            Assert.That(retrievedData.InstrumentId.Market, Is.Not.Null.And.Not.Empty);
            Assert.That(retrievedData.InstrumentId.Symbol, Is.Not.Null.And.Not.Empty);
            Assert.That(retrievedData.Timestamp, Is.Not.EqualTo(default(long)));

            Assert.That(retrievedData.Data.IndexValue, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        [Timeout(10000)]
        public void SubscriptionToMarketDataCannotBeStartedUnlessDataCallbackIsProvided()
        {
            var instrumentId = new InstrumentId()
            {
                Market = "ROFX",
                Symbol = Build.DollarFutureSymbol()
            };
            var entries = new[] { Entry.IndexValue };
            using var socket = Api.CreateMarketDataSocket(new[] { instrumentId }, entries, 1, 1);
            socket.OnData = null;

            var exception = Assert.ThrowsAsync<Exception>(socket.Start);
            Assert.That(exception.Message, Does.Contain(ErrorMessages.CallbackNotSet));
        }

        [Test]
        [Timeout(10000)]
        public async Task TryingToSubscribeToMarketDataForAnInvalidInstrumentTriggersAnError()
        {
            var invalidInstrumentId = new InstrumentId()
            {
                Market = "ROFX",
                Symbol = Build.RandomString()
            };
            var entries = new[] { Entry.IndexValue };
            using var socket = Api.CreateMarketDataSocket(new[] { invalidInstrumentId }, entries, 1, 1);
            socket.OnData += ((_, _) => { });

            var socketTask = socket.Start();
            while (!socket.IsRunning)
            {
                Thread.Sleep(10);
            }

            var exception = Assert.ThrowsAsync<Exception>(async () => { await socketTask; });
            Assert.That(exception.Message, Does.Contain(invalidInstrumentId.Symbol));
        }
    }
}
