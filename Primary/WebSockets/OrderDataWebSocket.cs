using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Primary.Data.Orders;
using Primary.Net;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

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
        internal OrderDataWebSocket(Api api, OrderDataRequest orderDataToRequest,
                                    CancellationToken cancelToken)
        :
        base(api, orderDataToRequest, cancelToken)
        { }

        public async Task SubmitOrder(string account, Order order)
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
            await SendJsonData(jsonString);
        }

    }
}
