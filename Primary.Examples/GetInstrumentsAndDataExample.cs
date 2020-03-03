using System;
using System.Linq;
using System.Threading.Tasks;

namespace Primary.Examples
{
    internal static class GetInstrumentsAndDataExample
    {
        private static async Task RunExample()
        {
            // Login
            var api = new Api(Api.DemoEndpoint);
            await api.Login(Api.DemoUsername, Api.DemoPassword);

            // Obtener instrumento a operar
            var allIInstruments = await api.GetAllInstruments();
            foreach (var instrument in allIInstruments)
            {
                Console.WriteLine($"{instrument.Market} / {instrument.Symbol}");
            }

            // Otengo los futuros de dolar
            var dollarFutures = allIInstruments.Where( s => s.Symbol.StartsWith("DO") &&
                                                            !s.Symbol.StartsWith("DOP ")
            );
            foreach (var dollarFuture in dollarFutures)
            {
                Console.WriteLine($"{dollarFuture.Symbol}");
            }

            // Obtengo los trades 
            var sampleInstrument = allIInstruments.First(s => s.Symbol == "DONov19");
            var threeDaysAgo = DateTime.Today.AddDays(-5);

            foreach (var trade in await api.GetHistoricalTrades(sampleInstrument, threeDaysAgo, DateTime.Today))
            {
                Console.WriteLine($"{trade.DateTime.ToLocalTime()}: ${trade.Price} ({trade.Size})");
            }
        }
    }
}
