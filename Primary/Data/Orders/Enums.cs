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
        Day,
        ImmediateOrCancel,
        FillOrKill,
        GoodTillDate
    }

    [JsonConverter(typeof(OrderStatusJsonSerializer))]
    public enum OrderStatus
    {
        New,
        Rejected,
        Cancelled,
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
                default: throw new InvalidEnumStringException();
            }
        }

        public static OrderType TypeFromApiString(string value)
        {
            switch (value.ToUpper())
            {
                case "MARKET": return OrderType.Market;
                case "LIMIT": return OrderType.Limit;
                default: throw new InvalidEnumStringException();
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
                default: throw new InvalidEnumStringException();
            }
        }

        public static OrderSide SideFromApiString(string value)
        {
            switch (value.ToUpper())
            {
                case "SELL": return OrderSide.Sell;
                case "BUY": return OrderSide.Buy;
                default: throw new InvalidEnumStringException();
            }
        }

        #endregion

        public static string ToApiString(this OrderExpiration value)
        {
            switch (value)
            {
                case OrderExpiration.Day: return "Day";
                case OrderExpiration.FillOrKill: return "FOK";
                case OrderExpiration.GoodTillDate: return "GTD";
                case OrderExpiration.ImmediateOrCancel: return "IOC";
                default: throw new InvalidEnumStringException();
            }
        }

        public static OrderExpiration ExpirationFromApiString(string value)
        {
            switch (value)
            {
                case "Day": return OrderExpiration.Day;
                case "FOK": return OrderExpiration.FillOrKill;
                case "GTD": return OrderExpiration.GoodTillDate;
                case "IOC": return OrderExpiration.ImmediateOrCancel;
                default: throw new InvalidEnumStringException();
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
                case OrderStatus.Filled: return "FILLED";
                default: throw new InvalidEnumStringException();
            }
        }

        public static OrderStatus StatusFromApiString(string value)
        {
            switch (value.ToUpper())
            {
                case "NEW": return OrderStatus.New;
                case "REJECTED": return OrderStatus.Rejected;
                case "CANCELLED": return OrderStatus.Cancelled;
                case "FILLED": return OrderStatus.Filled;
                default: throw new InvalidEnumStringException();
            }
        }

        #endregion
    }

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
