using Newtonsoft.Json;
using Primary.Serialization;
using System;
using System.Collections.Generic;

namespace Primary.Data
{
    public class AccountStatement
    {
        [JsonProperty("accountName")]
        public string Name { get; set; }

        [JsonProperty("marketMember")]
        public string MarketMember { get; set; }

        [JsonProperty("marketMemberIdentity")]
        public string MarketMemberIdentity { get; set; }

        [JsonProperty("collateral")]
        public decimal Collateral { get; set; }

        [JsonProperty("margin")]
        public decimal Margin { get; set; }

        [JsonProperty("availableToCollateral")]
        public decimal AvailableToCollateral { get; set; }

        [JsonProperty("detailedAccountReports")]
        public Dictionary<string, AccountReport> DetailedAccountReports { get; set; }

        [JsonProperty("hasError")]
        public bool HasError { get; set; }

        [JsonProperty("errorDetail")]
        public string ErrorDetail { get; set; }

        [JsonProperty("lastCalculation")]
        public decimal LastCalculation { get; set; }

        [JsonProperty("portfolio")]
        public decimal Portfolio { get; set; }

        [JsonProperty("ordersMargin")]
        public decimal OrdersMargin { get; set; }

        [JsonProperty("currentCash")]
        public decimal CurrentCash { get; set; }

        [JsonProperty("dailyDiff")]
        public decimal DailyDiff { get; set; }

        [JsonProperty("uncoveredMargin")]
        public decimal UncoveredMargin { get; set; }

        public class CurrencyBalance
        {
            [JsonProperty("consumed")]
            public decimal Consumed { get; set; }

            [JsonProperty("available")]
            public decimal Available { get; set; }
        }

        public class Cash
        {
            [JsonProperty("totalCash")]
            public decimal TotalCash { get; set; }

            [JsonProperty("detailedCash")]
            public Dictionary<string, decimal> DetailedCash { get; set; }
        }

        public class AvailableToOperate
        {
            [JsonProperty("cash")]
            public Cash Cash { get; set; }

            [JsonProperty("movements")]
            public decimal Movements { get; set; }

            [JsonProperty("total")]
            public decimal Total { get; set; }

            [JsonProperty("pendingMovements")]
            public decimal PendingMovements { get; set; }
        }

        [JsonConverter(typeof(JsonPathDeserializer))]
        public class AccountReport
        {
            [JsonProperty("currencyBalance.detailedCurrencyBalance")]
            public Dictionary<string, CurrencyBalance> CurrencyBalance { get; set; }

            [JsonProperty("availableToOperate")]
            public AvailableToOperate AvailableToOperate { get; set; }

            [JsonProperty("settlementDate")]
            [JsonConverter(typeof(UnixMillisecondsConverter))]
            public DateTime SettlementDate { get; set; }
        }
    }
}