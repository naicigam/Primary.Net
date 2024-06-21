using Newtonsoft.Json;
using Primary.Serialization;

namespace Primary.Data.Orders
{
    [JsonConverter(typeof(SettlementTypeJsonSerializer))]
    public enum SettlementType
    {
        CI,
        T24H,
        T48H
    }

    [JsonConverter(typeof(TypeJsonSerializer))]
    public enum Type
    {
        Market,
        Limit
    }

    [JsonConverter(typeof(SideJsonSerializer))]
    public enum Side
    {
        Buy,
        Sell
    }

    [JsonConverter(typeof(ExpirationJsonSerializer))]
    public enum Expiration
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

    [JsonConverter(typeof(StatusJsonSerializer))]
    public enum Status
    {
        /// <summary>The order does not have a status yet.</summary>
        NotSet,

        /// <summary>The order was successfully submitted.</summary>
        New,

        /// <summary>The order was submitted, but it is still being processed.</summary>
        PendingNew,

        /// <summary>The order was rejected.</summary>
        Rejected,

        /// <summary>The order was cancelled.</summary>
        Cancelled,

        /// <summary>The order was replaced, but it is still being processed.</summary>
        PendingReplace,

        /// <summary>The order was cancelled, but it is still being processed.</summary>
        PendingCancel,

        /// <summary>The order was partially filled.</summary>
        PartiallyFilled,

        /// <summary>The order was filled.</summary>
        Filled,

        /// <summary>The order expired.</summary>
        Expired
    }

    #region String serialization

    internal static class EnumsToApiStrings
    {
        #region SettlementType

        public static string ToApiString(this SettlementType value)
        {
            return value switch
            {
                SettlementType.CI => "0",
                SettlementType.T24H => "1",
                SettlementType.T48H => "2",
                _ => throw new InvalidEnumStringException(value.ToString()),
            };
        }

        public static SettlementType SettlementTypeFromApiString(string value)
        {
            return (value.ToUpper()) switch
            {
                "0" => SettlementType.CI,
                "1" => SettlementType.T24H,
                "2" => SettlementType.T48H,
                _ => throw new InvalidEnumStringException(value),
            };
        }

        #endregion

        #region Type

        public static string ToApiString(this Type value)
        {
            return value switch
            {
                Type.Market => "MARKET",
                Type.Limit => "LIMIT",
                _ => throw new InvalidEnumStringException(value.ToString()),
            };
        }

        public static Type TypeFromApiString(string value)
        {
            return (value.ToUpper()) switch
            {
                "MARKET" => Type.Market,
                "LIMIT" => Type.Limit,
                _ => throw new InvalidEnumStringException(value),
            };
        }

        #endregion

        #region Side

        public static string ToApiString(this Side value)
        {
            return value switch
            {
                Side.Buy => "BUY",
                Side.Sell => "SELL",
                _ => throw new InvalidEnumStringException(value.ToString()),
            };
        }

        public static Side SideFromApiString(string value)
        {
            return (value.ToUpper()) switch
            {
                "SELL" => Side.Sell,
                "BUY" => Side.Buy,
                _ => throw new InvalidEnumStringException(value),
            };
        }

        #endregion

        #region Expiration

        public static string ToApiString(this Expiration value)
        {
            return value switch
            {
                Expiration.Day => "DAY",
                Expiration.FillOrKill => "FOK",
                Expiration.GoodTillDate => "GTD",
                Expiration.ImmediateOrCancel => "IOC",
                _ => throw new InvalidEnumStringException(value.ToString()),
            };
        }

        public static Expiration ExpirationFromApiString(string value)
        {
            return value switch
            {
                "DAY" => Expiration.Day,
                "FOK" => Expiration.FillOrKill,
                "GTD" => Expiration.GoodTillDate,
                "IOC" => Expiration.ImmediateOrCancel,
                _ => throw new InvalidEnumStringException(value),
            };
        }

        #endregion

        #region Status

        public static string ToApiString(this Status value)
        {
            return value switch
            {
                Status.New => "NEW",
                Status.PendingNew => "PENDING_NEW",
                Status.Rejected => "REJECTED",
                Status.Cancelled => "CANCELLED",
                Status.PendingCancel => "PENDING_CANCEL",
                Status.PendingReplace => "PENDING_REPLACE",
                Status.PartiallyFilled => "PARTIALLY_FILLED",
                Status.Filled => "FILLED",
                Status.Expired => "EXPIRED",
                _ => throw new InvalidEnumStringException(value.ToString()),
            };
        }

        public static Status StatusFromApiString(string value)
        {
            return value switch
            {
                "NEW" => Status.New,
                "PENDING_NEW" => Status.PendingNew,
                "REJECTED" => Status.Rejected,
                "CANCELLED" => Status.Cancelled,
                "PENDING_CANCEL" => Status.PendingCancel,
                "PENDING_REPLACE" => Status.PendingReplace,
                "PARTIALLY_FILLED" => Status.PartiallyFilled,
                "FILLED" => Status.Filled,
                "EXPIRED" => Status.Expired,
                _ => throw new InvalidEnumStringException(value),
            };
        }

        #endregion
    }

    #endregion

    #region JSON serialization

    internal class SettlementTypeJsonSerializer : EnumJsonSerializer<SettlementType>
    {
        protected override string ToString(SettlementType enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override SettlementType FromString(string enumString)
        {
            return EnumsToApiStrings.SettlementTypeFromApiString(enumString);
        }
    }

    internal class TypeJsonSerializer : EnumJsonSerializer<Type>
    {
        protected override string ToString(Type enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override Type FromString(string enumString)
        {
            return EnumsToApiStrings.TypeFromApiString(enumString);
        }
    }

    internal class SideJsonSerializer : EnumJsonSerializer<Side>
    {
        protected override string ToString(Side enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override Side FromString(string enumString)
        {
            return EnumsToApiStrings.SideFromApiString(enumString);
        }
    }

    internal class ExpirationJsonSerializer : EnumJsonSerializer<Expiration>
    {
        protected override string ToString(Expiration enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override Expiration FromString(string enumString)
        {
            return EnumsToApiStrings.ExpirationFromApiString(enumString);
        }
    }

    internal class StatusJsonSerializer : EnumJsonSerializer<Status>
    {
        protected override string ToString(Status enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override Status FromString(string enumString)
        {
            return EnumsToApiStrings.StatusFromApiString(enumString);
        }
    }

    #endregion
}
