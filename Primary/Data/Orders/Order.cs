using System;
using Newtonsoft.Json;
using Primary.Data.Orders;

namespace Primary.Data
{
    public class Order : OrderId
    {
        /// <summary>Cancel previous order to the instrument.</summary>
        [JsonProperty("cancelPrevious")]
        public bool CancelPrevious { get; set; }

        /// <summary>Is this an iceberg order?</summary>
        [JsonProperty("iceberg")]
        public bool Iceberg { get; set; }

        /// <summary>How much of the order quantity is shown.</summary>
        [JsonProperty("displayQty")]
        public uint DisplayQuantity { get; set; }

        /// <summary>Order status.</summary>
        [JsonProperty("status")] 
        public OrderStatus Status { get; set; }

        /// <summary>More information about the order status.</summary>
        [JsonProperty("text")]
        public string StatusText { get; set; }

        /// <summary>Which instrument is the order for.</summary>
        [JsonProperty("instrumentId")]
        public Instrument Instrument { get; set; }

        /// <summary>Instrument price.</summary>
        [JsonProperty("price")]
        public decimal? Price { get; set; }
        
        /// <summary>Order size.</summary>
        [JsonProperty("orderQty")]
        public int Quantity { get; set; }
        
        /// <summary>Market or limit.</summary>
        [JsonProperty("ordType")]
        public OrderType Type { get; set; }
        
        /// <summary>Buy or sell.</summary>
        [JsonProperty("side")]
        public OrderSide Side { get; set; }

        /// <summary>How long the order will last if not filled.</summary>
        [JsonProperty("timeInForce")]
        public OrderExpiration Expiration { get; set; }
        
        /// <summary>Order expiration date.</summary>
        /// <remarks>Only valid when `Expiration` is `GoodTillDate`.</remarks>
        [JsonProperty("expireDate")]
        public DateTime ExpirationDate { get; set; }
    }
}
