using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Primary.Serialization;

namespace Primary.Data
{
    [JsonConverter(typeof(OrderTypeJsonSerializer))]
    public enum OrderType
    {
        None,
        Market,
        Limit
    }

    [JsonConverter(typeof(OrderSideJsonSerializer))]
    public enum OrderSide
    {
        Buy,
        Sell
    }

    [JsonConverter(typeof(OrderExpirationJsonSerializer))]
    public enum OrderExpiration
    {
        Day,
        ImmediateOrCancel,
        FillOrKill,
        GoodTillDate
    }

    public class Order
    {
        public string Symbol { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public OrderType Type { get; set; }
        public OrderSide Side { get; set; }
        public OrderExpiration Expiration { get; set; }
        public bool Iceberg { get; set; }
    }

    #region String serialization

    public class OrderTypeJsonSerializer : EnumJsonSerializer<OrderType>
    {
        protected override Dictionary<OrderType, string> EnumToString =>
            new Dictionary<OrderType, string>
            {
                {OrderType.Market, "Market"},
                {OrderType.Limit, "Limit"}
            };
    }

    public class OrderSideJsonSerializer : EnumJsonSerializer<OrderSide>
    {
        protected override Dictionary<OrderSide, string> EnumToString =>
            new Dictionary<OrderSide, string>
            {
                {OrderSide.Sell, "Sell"},
                {OrderSide.Buy, "Buy"}
            };
    }

    public class OrderExpirationJsonSerializer : EnumJsonSerializer<OrderExpiration>
    {
        protected override Dictionary<OrderExpiration, string> EnumToString =>
            new Dictionary<OrderExpiration, string>
            {
                {OrderExpiration.Day, "Day"},
                {OrderExpiration.FillOrKill, "FOK"},
                {OrderExpiration.GoodTillDate, "GTD"},
                {OrderExpiration.ImmediateOrCancel, "IOC"}
            };
    }

    #endregion
}
