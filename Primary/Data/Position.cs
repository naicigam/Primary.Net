using Newtonsoft.Json;
using Primary.Data.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primary.Data;

public class PositionInstrument

{
    [JsonProperty("symbolReference")]
    public string SymbolReference { get; set; }

    [JsonProperty("settlType")]
    public SettlementType SettlementType { get; set; }
}

public class Position
{
    [JsonProperty("instrument")]
    public PositionInstrument Instrument { get; set; }

    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("buySize")]
    public decimal BuySize { get; set; }

    [JsonProperty("buyPrice")]
    public decimal BuyPrice { get; set; }

    [JsonProperty("sellSize")]
    public decimal SellSize { get; set; }

    [JsonProperty("sellPrice")]
    public decimal SellPrice { get; set; }

    [JsonProperty("totalDailyDiff")]
    public decimal TotalDailyDiff { get; set; }

    [JsonProperty("totalDiff")]
    public decimal TotalDiff { get; set; }

    [JsonProperty("tradingSymbol")]
    public string TradingSymbol { get; set; }

    [JsonProperty("originalBuyPrice")]
    public decimal OriginalBuyPrice { get; set; }

    [JsonProperty("originalSellPrice")]
    public decimal OriginalSellPrice { get; set; }

    [JsonProperty("originalBuySize")]
    public decimal OriginalBuySize { get; set; }

    [JsonProperty("originalSellSize")]
    public decimal OriginalSellSize { get; set; }
}
