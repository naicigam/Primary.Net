using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Primary.Data;
using Primary.Net;

namespace Primary
{
    public class MarketDataWebSocket : WebSocket<MarketDataInfo, MarketData>
    {
        internal MarketDataWebSocket(IEnumerable<Instrument> instruments, IEnumerable<Entry> entries,
                                     uint level, uint depth, Uri url, string accessToken,
                                     CancellationToken cancelToken)
        : base(url, accessToken, cancelToken)
        {
            Request.Type = "smd";
            Request.Level = level;
            Request.Depth = depth;
            Request.Entries = entries.ToArray();
            Request.Products = instruments.ToArray();
        }
    }
}
