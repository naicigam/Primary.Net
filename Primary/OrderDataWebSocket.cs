using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Primary.Data.Orders;
using Primary.Net;

namespace Primary
{
    public struct Request
    {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("accounts")]
        public string[] Accounts;
    }

    public struct Response
    {
        [JsonProperty("orderReport")]
        public OrderData OrderReport;
    }

    public class OrderDataWebSocket : WebSocket<Request, Response>
    {
        internal OrderDataWebSocket(IEnumerable<string> accounts, Uri url, string accessToken,
                                    CancellationToken cancelToken)
        : base(url, accessToken, cancelToken)
        {
            Request.Type = "os";
            Request.Accounts = accounts.ToArray();
        }
    }
}
