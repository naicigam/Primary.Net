using NUnit.Framework;
using Primary.Data.Orders;
using System.Linq;
using System.Threading.Tasks;
using static Primary.Api;
using Type = Primary.Data.Orders.Type;

namespace Primary.Tests;

[TestFixture]
internal class PositionsTests : TestWithApi
{
    [Test]
    [Timeout(10000)]
    public async Task PositionsCanBeRetrieved()
    {
        var marketData = await GetSomeMarketData();
        var symbol = marketData.InstrumentId.Symbol;

        // Take the opposite side.
        var order = new Order()
        {
            InstrumentId = marketData.InstrumentId,
            Type = Type.Market,
            Side = marketData.Data.Offers?.Length > 0 ? Side.Buy : Side.Sell,
            Quantity = AllInstrumentsBySymbol[symbol].MinimumTradeVolume,
        };

        var orderId = await Api.SubmitOrder(DemoAccount, order);
        await WaitForOrderToComplete(orderId);

        var positions = await Api.GetPositions(DemoAccount);
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
}