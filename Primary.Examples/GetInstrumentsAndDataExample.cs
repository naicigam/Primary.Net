using System;
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
