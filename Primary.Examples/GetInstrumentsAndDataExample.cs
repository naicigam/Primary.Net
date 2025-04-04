using NUnit.Framework;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Primary.Examples
{
    internal static class GetInstrumentsAndDataExample
    {
        public static async Task Run()
        {
            Console.WriteLine("Connecting to ReMarkets...");

            var api = new Api(Api.DemoEndpoint);
            await api.Login(Api.DemoUsername, Api.DemoPassword);

            // Get instruments
            Console.WriteLine("Getting available instruments...");

            var allIInstruments = await api.GetAllInstruments();
            foreach (var instrument in allIInstruments)
            {
                Console.WriteLine($"{instrument.Market},{instrument.Symbol},{instrument.Currency},{instrument.Description},{instrument.PriceConversionFactor}");
            }

            
            // Get instruments with file cache
            Console.WriteLine("Getting available instruments with file cache...");
            string cacheFile = ".\\dummy.tmp";
            // Remove cache to test it.
            if(System.IO.File.Exists(cacheFile)) System.IO.File.Delete(cacheFile);
            
            // 1st call -> Lengthy operation
            var allIInstrumentsFileCache = await api.GetAllInstrumentsFileCache(cacheFile);
            Trace.Assert(allIInstrumentsFileCache != null);
            Trace.Assert(allIInstrumentsFileCache.Count() == allIInstruments.Count());

            // 2do call -> Fast operation
            var allInstrumentsFileCache2 = await api.GetAllInstrumentsFileCache(cacheFile);            
            Trace.Assert(allIInstrumentsFileCache != null);
            Trace.Assert(allIInstrumentsFileCache.Count() == allInstrumentsFileCache2.Count());



            // Get all the dollar futures and their trades
            var fiveDaysAgo = DateTime.Today.AddDays(-5);

            var dollarFutures = allIInstruments.Where(s => s.Symbol.StartsWith("DLR/"));
            foreach (var dollarFuture in dollarFutures)
            {
                var symbol = dollarFuture.Symbol;

                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Getting trades for {symbol}");

                foreach (var trade in await api.GetHistoricalTrades(dollarFuture, fiveDaysAgo, DateTime.Today))
                {
                    Console.WriteLine($"{trade.DateTime.ToLocalTime()}: ${trade.Price} ({trade.Size})");
                }
            }

            // Logout from server
            await api.Logout();
        }
    }
}
