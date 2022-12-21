using NUnit.Framework;
using Primary.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                Assert.That(instrument.Description, Is.Not.Null.And.Not.Empty);
                Assert.That(instrument.Currency, Is.Not.Null.And.Not.Empty);
                Assert.That(instrument.PriceConversionFactor, Is.Not.EqualTo(default));
            }

            Assert.That(instruments.Where(i => i.MaturityDate != default), Is.Not.Empty);
        }

        [Test]
        public async Task HistoricalTradesCanBeRetrievedForAnInstrument()
        {
            var instrumentId = new InstrumentId()
            {
                Market = "ROFX",
                Symbol = Build.DollarFutureSymbol()
            };

            var dateFrom = DateTime.Today.AddDays(-20);
            var dateTo = DateTime.Today;

            var trades = await Api.GetHistoricalTrades(instrumentId, dateFrom, dateTo);
            Assert.That(trades, Is.Not.Empty);

            foreach (var trade in trades)
            {
                Assert.That(trade.Price, Is.Not.EqualTo(default));
                Assert.That(trade.Size, Is.Not.EqualTo(default));
                Assert.That(trade.DateTime, Is.Not.EqualTo(default));
                Assert.That(trade.ServerTime, Is.Not.EqualTo(default));

                Assert.That(trade.DateTime.Date, Is.GreaterThanOrEqualTo(dateFrom).And.LessThanOrEqualTo(dateTo));
            }
        }

        // Test: No data (ex. old instrument).
    }
}
