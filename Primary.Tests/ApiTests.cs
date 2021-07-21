using System;
using System.Linq;
using NUnit.Framework;
using System.Threading.Tasks;
using Primary.Data;

namespace Primary.Tests
{
    [TestFixture]
    internal class ApiTests : TestWithApi
    {
        [Test]
        public async Task AllAvailableInstrumentsCanBeRetrieved()
        {
            var instruments = await Api.GetAllInstruments();
            Assert.That(instruments, Is.Not.Empty);
            
            foreach (var instrument in instruments)
            {
                Assert.That(instrument.Market, Is.Not.Null.And.Not.Empty);
                Assert.That(instrument.Symbol, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public async Task HistoricalTradesCanBeRetrievedForAnInstrument()
        {
            var instrument = new Instrument() 
            { 
                Market = "ROFX", 
                Symbol = Build.DollarFutureSymbol() 
            };

            var dateFrom = DateTime.Today.AddDays(-20);
            var dateTo = DateTime.Today;

            var trades = await Api.GetHistoricalTrades(instrument, dateFrom, dateTo);
            Assert.That(trades, Is.Not.Empty);

            foreach (var trade in trades)
            {
                Assert.That(trade.Price, Is.Not.EqualTo(default(float)));
                Assert.That(trade.Size, Is.Not.EqualTo(default(float)));
                Assert.That(trade.DateTime, Is.Not.EqualTo(default(DateTime)));
                Assert.That(trade.ServerTime, Is.Not.EqualTo(default(long)));

                Assert.That(trade.DateTime.Date, Is.GreaterThanOrEqualTo(dateFrom).And.LessThanOrEqualTo(dateTo));
            }
        }

        // Test: No data (ex. old instrument).
    }
}
