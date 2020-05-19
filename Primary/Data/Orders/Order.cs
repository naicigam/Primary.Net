using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Primary.Data.Orders;

namespace Primary.Data
{
    public class Order
    {
        /// <summary>Cancel previous order to the instrument.</summary>
        /// <remarks>
        ///     Only works when submitting an order. Will always have default value when retrieving
        ///     an order information.
        /// </remarks>
        public bool CancelPrevious { get; set; }

        /// <summary>Is this an iceberg order?</summary>
        [JsonProperty("iceberg")]
        public bool Iceberg { get; set; }

        /// <summary>How much of the order quantity is shown.</summary>
        [JsonProperty("displayQty")]
        public uint DisplayQuantity { get; set; }

        /// <summary>Quantity affected in the last operation.</summary>
        [JsonProperty("lastQty")]
        public uint LastQuantity { get; set; }

        /// <summary>Total quantity affected on the order.</summary>
        [JsonProperty("cumQty")]
        public uint CumulativeQuantity { get; set; }

        /// <summary>How much quantity is left on the order.</summary>
        [JsonProperty("leavesQty")]
        public uint LeavesQuantity { get; set; }

        /// <summary>Client id.</summary>
        [JsonProperty("clientId")]
        public ulong ClientId { get; set; }
        
        /// <summary>Client proprietary.</summary>
        [JsonProperty("proprietary")]
        public string Proprietary { get; set; }

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
