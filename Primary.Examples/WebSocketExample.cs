using Primary.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Primary.Examples
{
    internal static class WebSocketExample
    {
        public static async Task Run()
        {
            Console.WriteLine("Connecting to ReMarkets...");

            var api = new Api(Api.DemoEndpoint);
            await api.Login(Api.DemoUsername, Api.DemoPassword);

            // Get a all dollar futures and forwards
            Console.WriteLine("Getting available instruments...");

            var allInstruments = await api.GetAllInstruments();
            var dollarFuturesAndForwards = allInstruments.Where(c => c.Symbol.Contains("DLR/"));

            // Subscribe to bids and offers
            var entries = new[] { Entry.Bids, Entry.Offers };

            Console.WriteLine("Connecting to market data...");

            using var socket = api.CreateMarketDataSocket(dollarFuturesAndForwards, entries, 1, 1);
            socket.OnData = OnMarketData;

            var socketTask = await socket.Start();
            socketTask.Wait();
            await socketTask;
        }

        private static void OnMarketData(Api api, MarketData marketData)
        {
            var bid = default(decimal);
            var offer = default(decimal);

            var bidSize = default(decimal);
            var offerSize = default(decimal);

            if (marketData.Data.Bids != null)
            {
                foreach (var trade in marketData.Data.Bids)
                {
                    bid = trade.Price;
                    bidSize = trade.Size;
                }
            }

            if (marketData.Data.Offers != null)
            {
                foreach (var trade in marketData.Data.Offers)
                {
                    offer = trade.Price;
                    offerSize = trade.Size;
                }
            }

            Console.WriteLine($"({marketData.Timestamp}) " +
                              $"{marketData.InstrumentId.Symbol} -> " +
                              $"{bid} ({bidSize}) --> ${offer - bid} <-- {offer} ({offerSize})"
            );
        }
    }
}
