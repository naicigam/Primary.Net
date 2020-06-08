using System;
using System.Threading;
using Newtonsoft.Json;
using Primary.Data.Orders;
using Primary.Net;

namespace Primary
{
    public struct Request
    {
        [JsonProperty("type", Order=-2)]
        public string Type => "os";

        [JsonProperty("accounts")]
        public OrderStatus.AccountId[] Accounts;
    }

    public struct Response
    {
        [JsonProperty("orderReport")]
        public OrderStatus OrderReport;
    }

    public class OrderDataWebSocket : WebSocket<Request, Response>
    {
        internal OrderDataWebSocket(Request orderDataToRequest, Uri url, string accessToken,
                                    CancellationToken cancelToken)
        : 
        base(orderDataToRequest, url, accessToken, cancelToken)
        {}
    }
}
