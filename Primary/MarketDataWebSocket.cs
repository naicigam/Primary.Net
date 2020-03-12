using System;
using System.Threading;
using Primary.Data;
using Primary.Net;

namespace Primary
{
    public class MarketDataWebSocket : WebSocket<MarketDataInfo, MarketData>
    {
        internal MarketDataWebSocket(MarketDataInfo marketDataToRequest, Uri url, string accessToken,
                                     CancellationToken cancelToken)
        : 
        base(marketDataToRequest, url, accessToken, cancelToken)
        {}
    }
}
