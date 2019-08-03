using System;
using Newtonsoft.Json;

namespace Primary.Data.Orders
{
    internal interface IOrderStatus : IOrderId
    {
        [JsonProperty("status")] 
        OrderStatus Status { get; set; }

        [JsonProperty("text")]
        string StatusText { get; set; }

        [JsonProperty("instrumentId")]
        Instrument Instrument { get; set; }

        [JsonProperty("price")]
        float Price { get; set; }
        
        [JsonProperty("orderQty")]
        int Quantity { get; set; }
        
        [JsonProperty("ordType")]
        OrderType Type { get; set; }
        
        [JsonProperty("side")]
        OrderSide Side { get; set; }

        [JsonProperty("timeInForce")]
        OrderExpiration Expiration { get; set; }
        
        [JsonProperty("expireDate")]
        DateTime ExpirationDate { get; set; }
    }
}
