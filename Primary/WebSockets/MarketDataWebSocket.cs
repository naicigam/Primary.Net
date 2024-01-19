﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Primary.Data;
using Primary.Net;
using System.Threading;

namespace Primary.WebSockets
{
    public class MarketDataWebSocket : WebSocket<MarketDataInfo, MarketData>
    {
        internal MarketDataWebSocket(Api api, MarketDataInfo marketDataToRequest, CancellationToken cancelToken,
            JsonSerializerSettings jsonSerializerSettings = null, ILoggerFactory loggerFactory = null)
        :
        base(api, marketDataToRequest, cancelToken, jsonSerializerSettings, loggerFactory)
        { }
    }
}
