using Newtonsoft.Json;
using Primary.Serialization;

namespace Primary.Data.Orders
{
    [JsonConverter(typeof(OrderTypeJsonSerializer))]
    public enum OrderType
    {
        /// <summary>Market order.</summary>
        Market, 

        /// <summary>Limit order.</summary>
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
        
        /// <summary>The order was submitted but it is still being processed.</summary>
        PendingNew,
        
        /// <summary>The order was rejected.</summary>
        Rejected,
        
        /// <summary>The order was cancelled.</summary>
        Cancelled,
        
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
            switch (value)
            {
                case OrderType.Market: return "Market";
                case OrderType.Limit: return "Limit";
                default: throw new InvalidEnumStringException( value.ToString() );
            }
        }

        public static OrderType TypeFromApiString(string value)
        {
            switch (value.ToUpper())
            {
                case "MARKET": return OrderType.Market;
                case "LIMIT": return OrderType.Limit;
                default: throw new InvalidEnumStringException(value);
            }
        }

        #endregion
        
        #region OrderSide

        public static string ToApiString(this OrderSide value)
        {
            switch (value)
            {
                case OrderSide.Buy: return "Sell";
                case OrderSide.Sell: return "Buy";
                default: throw new InvalidEnumStringException( value.ToString() );
            }
        }

        public static OrderSide SideFromApiString(string value)
        {
            switch (value.ToUpper())
            {
                case "SELL": return OrderSide.Sell;
                case "BUY": return OrderSide.Buy;
                default: throw new InvalidEnumStringException(value);
            }
        }

        #endregion

        #region OrderExpiration

        public static string ToApiString(this OrderExpiration value)
        {
            switch (value)
            {
                case OrderExpiration.Day: return "DAY";
                case OrderExpiration.FillOrKill: return "FOK";
                case OrderExpiration.GoodTillDate: return "GTD";
                case OrderExpiration.ImmediateOrCancel: return "IOC";
                default: throw new InvalidEnumStringException( value.ToString() );
            }
        }

        public static OrderExpiration ExpirationFromApiString(string value)
        {
            switch (value)
            {
                case "DAY": return OrderExpiration.Day;
                case "FOK": return OrderExpiration.FillOrKill;
                case "GTD": return OrderExpiration.GoodTillDate;
                case "IOC": return OrderExpiration.ImmediateOrCancel;
                default: throw new InvalidEnumStringException(value);
            }
        }

        #endregion

        #region OrderStatus

        public static string ToApiString(this OrderStatus value)
        {
            switch (value)
            {
                case OrderStatus.New: return "NEW";
                case OrderStatus.Rejected: return "REJECTED";
                case OrderStatus.Cancelled: return "CANCELLED";
                case OrderStatus.PartiallyFilled: return "PARTIALLY_FILLED";
                case OrderStatus.Filled: return "FILLED";
                default: throw new InvalidEnumStringException( value.ToString() );
            }
        }

        public static OrderStatus StatusFromApiString(string value)
        {
            switch (value)
            {
                case "NEW": return OrderStatus.New;
                case "PENDING_NEW": return OrderStatus.PendingNew;
                case "REJECTED": return OrderStatus.Rejected;
                case "CANCELLED": return OrderStatus.Cancelled;
                case "PARTIALLY_FILLED": return OrderStatus.PartiallyFilled;
                case "FILLED": return OrderStatus.Filled;
                default: throw new InvalidEnumStringException(value);
            }
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
