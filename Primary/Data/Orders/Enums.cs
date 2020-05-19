using Newtonsoft.Json;
using Primary.Serialization;

namespace Primary.Data.Orders
{
    [JsonConverter(typeof(OrderTypeJsonSerializer))]
    public enum OrderType
    {
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
        /// <summary>Order valid during the day. It will expires when the market closes.</summary>
        Day,

        /// <summary>Immediate or cancel.</summary>
        ImmediateOrCancel,

        /// <summary>Fill or kill.</summary>
        FillOrKill,

        /// <summary>Good Till Date (Must send ExpirationDate field in the Order).</summary>
        GoodTillDate
    }

    [JsonConverter(typeof(OrderStatusJsonSerializer))]
    public enum OrderStatus
    {
        /// <summary>The order was successfully submitted.</summary>
        New,
        
        /// <summary>The order was submitted, but it is still being processed.</summary>
        PendingNew,
        
        /// <summary>The order was rejected.</summary>
        Rejected,
        
        /// <summary>The order was cancelled.</summary>
        Cancelled,
        
        /// <summary>The order was cancelled, but it is still being processed.</summary>
        PendingCancel,

        /// <summary>The order was partially filled.</summary>
        PartiallyFilled,

        /// <summary>The order was filled.</summary>
        Filled
    }

    #region String serialization

    internal static class EnumsToApiStrings
    {
        #region OrderType

        public static string ToApiString(this OrderType value)
        {
            return value switch
            {
                OrderType.Market => "Market",
                OrderType.Limit => "Limit",
                _ => throw new InvalidEnumStringException(value.ToString()),
            };
        }

        public static OrderType TypeFromApiString(string value)
        {
            return (value.ToUpper()) switch
            {
                "MARKET" => OrderType.Market,
                "LIMIT" => OrderType.Limit,
                _ => throw new InvalidEnumStringException(value),
            };
        }

        #endregion
        
        #region OrderSide

        public static string ToApiString(this OrderSide value)
        {
            return value switch
            {
                OrderSide.Buy => "Sell",
                OrderSide.Sell => "Buy",
                _ => throw new InvalidEnumStringException(value.ToString()),
            };
        }

        public static OrderSide SideFromApiString(string value)
        {
            return (value.ToUpper()) switch
            {
                "SELL" => OrderSide.Sell,
                "BUY" => OrderSide.Buy,
                _ => throw new InvalidEnumStringException(value),
            };
        }

        #endregion

        #region OrderExpiration

        public static string ToApiString(this OrderExpiration value)
        {
            return value switch
            {
                OrderExpiration.Day => "DAY",
                OrderExpiration.FillOrKill => "FOK",
                OrderExpiration.GoodTillDate => "GTD",
                OrderExpiration.ImmediateOrCancel => "IOC",
                _ => throw new InvalidEnumStringException(value.ToString()),
            };
        }

        public static OrderExpiration ExpirationFromApiString(string value)
        {
            return value switch
            {
                "DAY" => OrderExpiration.Day,
                "FOK" => OrderExpiration.FillOrKill,
                "GTD" => OrderExpiration.GoodTillDate,
                "IOC" => OrderExpiration.ImmediateOrCancel,
                _ => throw new InvalidEnumStringException(value),
            };
        }

        #endregion

        #region OrderStatus

        public static string ToApiString(this OrderStatus value)
        {
            return value switch
            {
                OrderStatus.New => "NEW",
                OrderStatus.PendingNew => "PENDING_NEW",
                OrderStatus.Rejected => "REJECTED",
                OrderStatus.Cancelled => "CANCELLED",
                OrderStatus.PendingCancel => "PENDING_CANCEL",
                OrderStatus.PartiallyFilled => "PARTIALLY_FILLED",
                OrderStatus.Filled => "FILLED",
                _ => throw new InvalidEnumStringException(value.ToString()),
            };
        }

        public static OrderStatus StatusFromApiString(string value)
        {
            return value switch
            {
                "NEW" => OrderStatus.New,
                "PENDING_NEW" => OrderStatus.PendingNew,
                "REJECTED" => OrderStatus.Rejected,
                "CANCELLED" => OrderStatus.Cancelled,
                "PENDING_CANCEL" => OrderStatus.PendingCancel,
                "PARTIALLY_FILLED" => OrderStatus.PartiallyFilled,
                "FILLED" => OrderStatus.Filled,
                _ => throw new InvalidEnumStringException(value),
            };
        }

        #endregion
    }

    #endregion

    #region JSON serialization

    internal class OrderTypeJsonSerializer : EnumJsonSerializer<OrderType>
    {
        protected override string ToString(OrderType enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override OrderType FromString(string enumString)
        {
            return EnumsToApiStrings.TypeFromApiString(enumString);
        }
    }

    internal class OrderSideJsonSerializer : EnumJsonSerializer<OrderSide>
    {
        protected override string ToString(OrderSide enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override OrderSide FromString(string enumString)
        {
            return EnumsToApiStrings.SideFromApiString(enumString);
        }
    }

    internal class OrderExpirationJsonSerializer : EnumJsonSerializer<OrderExpiration>
    {
        protected override string ToString(OrderExpiration enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override OrderExpiration FromString(string enumString)
        {
            return EnumsToApiStrings.ExpirationFromApiString(enumString);
        }
    }

    internal class OrderStatusJsonSerializer : EnumJsonSerializer<OrderStatus>
    {
        protected override string ToString(OrderStatus enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override OrderStatus FromString(string enumString)
        {
            return EnumsToApiStrings.StatusFromApiString(enumString);
        }
    }

    #endregion
}
