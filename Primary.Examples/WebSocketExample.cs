using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Primary.Data;

namespace Primary.Examples
{
    internal static class WebSocketExample
    {
        private static async Task Main()
        {
            Console.WriteLine("Connecting to ReMarkets...");

            var api = new Api(Api.DemoEndpoint);
            await api.Login(Api.DemoUsername, Api.DemoPassword);

            // Get a all dollar futures
            Console.WriteLine("Getting available instruments...");

            var allIInstruments = await api.GetAllInstruments();

            var symbols = new[]
            {
                "DOOct19",
                "DONov19",
                "DODic19"
            };
            var dollarFuture = allIInstruments.Where(c => symbols.Contains(c.Symbol));

            // Subscribe to bids and offers
            var entries = new[] { Entry.Bids, Entry.Offers };

            Console.WriteLine("Connecting to market data...");

            using (var socket = api.CreateSocket(dollarFuture, entries, 1, 1))
            {
                socket.OnMarketData = OnMarketData;

                var socketTask = await socket.Start();
                socketTask.Wait();
                await socketTask;
            }
        }

        private static void OnMarketData(MarketData marketData)
        {
            var bid = default(decimal);
            var offer = default(decimal);

            var bidSize = default(decimal);
            var offerSize = default(decimal);

            foreach (var (entry, trades) in marketData.Data)
            {
                foreach (var trade in trades)
                {
                    if (entry == Entry.Bids)
                    {
                        bid = trade.Price;
                        bidSize = trade.Size;
                    }
                    else if (entry == Entry.Offers)
                    {
                        offer = trade.Price;
                        offerSize = trade.Size;
                    }
                }
            }

            Console.WriteLine($"({marketData.Timestamp}) " +
                              $"{marketData.Instrument.Symbol} -> " +
                              $"{bid} ({bidSize}) --> ${offer - bid} <-- {offer} ({offerSize})"
            );
        }
    }
}
