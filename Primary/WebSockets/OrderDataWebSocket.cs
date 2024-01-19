using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Primary.Data.Orders;
using Primary.Net;
using System.Globalization;
using System.Threading;

namespace Primary.WebSockets
{
    public struct OrderDataRequest
    {
        [JsonProperty("type", Order = -2)]
        public static string Type => "os";

        [JsonProperty("accounts")]
        public OrderStatus.AccountId[] Accounts;
    }

    public struct OrderData
    {
        [JsonProperty("orderReport")]
        public OrderStatus OrderReport;
    }

    public class OrderDataWebSocket : WebSocket<OrderDataRequest, OrderData>
    {
        internal OrderDataWebSocket(Api api, OrderDataRequest orderDataToRequest, CancellationToken cancelToken,
            JsonSerializerSettings jsonSerializerSettings = null, ILoggerFactory loggerFactory = null)
        :
        base(api, orderDataToRequest, cancelToken, jsonSerializerSettings, loggerFactory)
        { }

        public void SubmitOrder(string account, Order order)
        {
            var jsonOrder = new JObject()
            {
                ["type"] = "no",
                ["product"] = new JObject(
                        new JProperty("marketId", order.InstrumentId.Market),
                        new JProperty("symbol", order.InstrumentId.Symbol)
                    ),
                ["account"] = account,
                ["quantity"] = order.Quantity.ToString(CultureInfo.InvariantCulture),
                ["side"] = order.Side.ToApiString(),
                ["iceberg"] = order.Iceberg.ToString(CultureInfo.InvariantCulture),
                ["price"] = order.Price?.ToString(CultureInfo.InvariantCulture),
                ["ordType"] = order.Type.ToApiString(),
                ["wsClOrdId"] = order.WebSocketClientOrderId,
                ["timeInForce"] = order.Expiration.ToApiString(),
                ["cancelPrevious"] = order.CancelPrevious.ToString(CultureInfo.InvariantCulture)
            };

            if (order.Expiration == Expiration.GoodTillDate)
            {
                jsonOrder["expireDate"] = order.ExpirationDate.ToString("yyyyMMdd");
            }

            if (order.Iceberg)
            {
                jsonOrder["displayQuantity"] = order.DisplayQuantity;
            }

            var jsonString = JsonConvert.SerializeObject(jsonOrder);
            SendJsonData(jsonString);
        }

        public void CancelOrder(Order order)
        {
            var jsonCancelOrder = new JObject()
            {
                ["type"] = "co",
                ["clientId"] = order.ClientOrderId,
                ["proprietary"] = order.Proprietary
            };

            var jsonString = JsonConvert.SerializeObject(jsonCancelOrder);
            SendJsonData(jsonString);
        }

    }
}
