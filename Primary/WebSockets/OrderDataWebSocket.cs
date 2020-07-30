using System;
using System.Threading;
using Newtonsoft.Json;
using Primary.Data.Orders;
using Primary.Net;

namespace Primary.WebSockets
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
        internal OrderDataWebSocket(Api api, Request orderDataToRequest,
                                    CancellationToken cancelToken)
        : 
        base(api, orderDataToRequest, cancelToken)
        {}
    }
}
