using NUnit.Framework;
using Primary.Data;
using Primary.Data.Orders;
using System.Linq;
using System.Threading.Tasks;

namespace Primary.Tests
{
    [TestFixture]
    internal class AccountsTests : TestWithApi
    {
        [Test]
        public async Task AccountStatementCanBeRetrieved()
        {
            // Submit an order
            var accountStatement = await Api.GetAccountStatement(Api.DemoAccount);

            Assert.That(accountStatement, Is.Not.Null);
            Assert.That(accountStatement.Name, Is.Not.Null.And.Not.Empty);
            Assert.That(accountStatement.MarketMember, Is.Not.Null.And.Not.Empty);
            Assert.That(accountStatement.MarketMemberIdentity, Is.Not.Null.And.Not.Empty);

            Assert.That(accountStatement.DetailedAccountReports, Is.Not.Empty);

            var detailedAccountReport = accountStatement.DetailedAccountReports.Values.First();
            Assert.That(detailedAccountReport.SettlementDate, Is.Not.EqualTo(default));
            Assert.That(detailedAccountReport.CurrencyBalance, Is.Not.Null.And.Not.Empty);
            Assert.That(detailedAccountReport.AvailableToOperate.Cash.DetailedCash, Is.Not.Empty);
        }

        [Test]
        public async Task AccountsCanBeRetrieved()
        {
            var accounts = await Api.GetAccounts();
            Assert.That(accounts, Is.Not.Empty);

            var account = accounts.First();
            Assert.That(account.BrokerId, Is.Not.EqualTo(default));
            Assert.That(account.Id, Is.Not.EqualTo(default));
            Assert.That(account.Name, Is.EqualTo(Api.DemoAccount));
            Assert.That(account.Status, Is.True);
        }

        [Test]
        [Timeout(20000)]
        public async Task PositionsCanBeRetrieved()
        {
            var instrument = await RandomInstrument(AnotherApi);
            var symbol = instrument.Symbol;

            // Generate liquidity
            var buyOrder = new Order()
            {
                InstrumentId = instrument,
                Type = Type.Limit,
                Price = instrument.MinimumTradePrice + 10,
                Side = Side.Sell,
                Quantity = instrument.MinimumTradeVolume
            };

            var orderId = await AnotherApi.SubmitOrder(AnotherApiAccount, buyOrder);
            await WaitForOrderToComplete(AnotherApi, orderId);

            // Take the opposite side
            var order = new Order()
            {
                InstrumentId = instrument,
                Type = Type.Market,
                Side = Side.Buy,
                Quantity = instrument.MinimumTradeVolume
            };

            orderId = await Api.SubmitOrder(ApiAccount, order);
            await WaitForOrderToComplete(Api, orderId);

            var positions = await Api.GetAccountPositions(ApiAccount);
            Assert.That(positions, Is.Not.Null);

            var position = positions.FirstOrDefault(p => p.Symbol == symbol);
            Assert.That(position, Is.Not.EqualTo(default));
            Assert.That(position.Symbol, Is.EqualTo(symbol));

            if (order.Side == Side.Buy)
            {
                Assert.That(position.BuySize, Is.GreaterThanOrEqualTo(order.Quantity));
                Assert.That(position.OriginalBuyPrice, Is.Not.EqualTo(0));
            }
            else
            {
                Assert.That(position.SellSize, Is.GreaterThanOrEqualTo(order.Quantity));
                Assert.That(position.OriginalSellPrice, Is.Not.EqualTo(0));
            }
        }

        private static async Task<Instrument> RandomInstrument(Api api)
        {
            var instruments = (await api.GetAllInstruments()).Where(i => i.Type == InstrumentType.Equity);
            return instruments.ElementAt(_random.Next(0, instruments.Count()));
        }
        private static readonly System.Random _random = new();
    }
}