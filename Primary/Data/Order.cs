using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Primary.Serialization;

namespace Primary.Data
{
    [TypeConverter(typeof(OrderTypeToStringConverter))]
    public enum OrderType
    {
        Market,
        Limit
    }

    [TypeConverter(typeof(OrderSideToStringConverter))]
    public enum OrderSide
    {
        Buy,
        Sell
    }

    [TypeConverter(typeof(OrderExpirationToStringConverter))]
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

    public class OrderTypeToStringConverter : EnumToStringConverter<OrderType>
    {
        public OrderTypeToStringConverter()
        {
            _enumToString = OrderTypeToString;
        }

        private static readonly Dictionary<OrderType, string> OrderTypeToString = new Dictionary<OrderType, string>()
        {
            {OrderType.Market, "Market"},
            {OrderType.Limit, "Limit"}
        };
    }

    public class OrderSideToStringConverter : EnumToStringConverter<OrderSide>
    {
        public OrderSideToStringConverter()
        {
            _enumToString = OrderSideToString;
        }

        private static readonly Dictionary<OrderSide, string> OrderSideToString = new Dictionary<OrderSide, string>()
        {
            {OrderSide.Sell, "Sell"},
            {OrderSide.Buy, "Buy"}
        };
    }

    public class OrderExpirationToStringConverter : EnumToStringConverter<OrderExpiration>
    {
        public OrderExpirationToStringConverter()
        {
            _enumToString = OrderExpirationToString;
        }

        private static readonly Dictionary<OrderExpiration, string> OrderExpirationToString = new Dictionary<OrderExpiration, string>()
        {
            {OrderExpiration.Day, "Day"},
            {OrderExpiration.FillOrKill, "FOK"},
            {OrderExpiration.GoodTillDate, "GTD"},
            {OrderExpiration.ImmediateOrCancel, "IOC"}
        };
    }

    #endregion
}
