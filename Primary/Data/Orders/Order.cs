using System;
using Newtonsoft.Json;
using Primary.Data.Orders;

namespace Primary.Data
{
    public class Order
    {
        public string Symbol => Instrument.Symbol;

        public bool CancelPrevious { get; set; }

        public bool Iceberg { get; set; }
        public uint DisplayQuantity { get; set; }

        [JsonProperty("clientId")]
        public ulong ClientId { get; set; }
        
        [JsonProperty("proprietary")]
        public string Proprietary { get; set; }

        [JsonProperty("status")] 
        public OrderStatus Status { get; set; }

        [JsonProperty("text")]
        public string StatusText { get; set; }

        [JsonProperty("instrumentId")]
        public Instrument Instrument { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
        
        [JsonProperty("orderQty")]
        public int Quantity { get; set; }
        
        [JsonProperty("ordType")]
        public OrderType Type { get; set; }
        
        [JsonProperty("side")]
        public OrderSide Side { get; set; }

        [JsonProperty("timeInForce")]
        public OrderExpiration Expiration { get; set; }
        
        [JsonProperty("expireDate")]
        public DateTime ExpirationDate { get; set; }
    }
}
