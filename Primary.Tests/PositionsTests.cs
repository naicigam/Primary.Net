using Newtonsoft.Json;
using NUnit.Framework;
using Primary.Data.Orders;
using System.Threading.Tasks;
using static Primary.Api;

namespace Primary.Tests;

[TestFixture]
internal class PositionsTests : TestWithApi
{
    [Test]
    public async Task PositionsCanBeRetrieved()
    {
        var positions = await Api.GetPositions(Api.DemoAccount);

        Assert.That(positions, Is.Not.Null);
        // Demo Account has no positions
        Assert.That(positions.Length, Is.Zero);
        
    }

    [Test]
    public void PositionsJsonCanBeDeserialized()
    {
        var jsonResponse = $$"""
            {
                "status":"OK",
                "positions":[
                      {
                         "instrument":{
                            "symbolReference":"KO",
                            "settlType":0
                         },
                         "symbol":"MERV - XMEV - KO - 48hs",
                         "buySize": 123.0,
                         "buyPrice": 123.0,
                         "sellSize":0.0,
                         "sellPrice":0.0,
                         "totalDailyDiff":12.5,
                         "totalDiff":10.2,
                         "tradingSymbol":"MERV - XMEV - KO - CI",
                         "originalBuyPrice":123.0,
                         "originalSellPrice":0.0,
                         "originalBuySize":0,
                         "originalSellSize":0
                      },
                      {
                         "instrument":{
                            "symbolReference":"KO",
                            "settlType":1
                         },
                         "symbol":"MERV - XMEV - KO - 48hs",
                         "buySize": 123.0,
                         "buyPrice": 123.0,
                         "sellSize":0.0,
                         "sellPrice":0.0,
                         "totalDailyDiff":12.5,
                         "totalDiff":10.2,
                         "tradingSymbol":"MERV - XMEV - KO - 24hs",
                         "originalBuyPrice":123.0,
                         "originalSellPrice":0.0,
                         "originalBuySize":0,
                         "originalSellSize":0
                      },
                      {
                         "instrument":{
                            "symbolReference":"KO",
                            "settlType":2
                         },
                         "symbol":"MERV - XMEV - KO - 48hs",
                         "buySize": 123.0,
                         "buyPrice": 123.0,
                         "sellSize":0.0,
                         "sellPrice":0.0,
                         "totalDailyDiff":12.5,
                         "totalDiff":10.2,
                         "tradingSymbol":"MERV - XMEV - KO - 48hs",
                         "originalBuyPrice":123.0,
                         "originalSellPrice":0.0,
                         "originalBuySize":0,
                         "originalSellSize":0
                      }

                 ]
            }
            """;
        var positionResponse = JsonConvert.DeserializeObject<PositionsResponse>(jsonResponse);

        Assert.AreEqual(positionResponse.Positions[0].Instrument.SettlementType, SettlementType.CI);
        Assert.AreEqual(positionResponse.Positions[1].Instrument.SettlementType, SettlementType.T24H);
        Assert.AreEqual(positionResponse.Positions[2].Instrument.SettlementType, SettlementType.T48H);
    }
}