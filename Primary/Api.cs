using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Primary.Data;
using Primary.Data.Orders;
using ServiceStack;

namespace Primary
{
    public class Api
    {
        /// <summary>This is the default production endpoint.</summary>
        public static Uri ProductionEndpoint => new Uri("https://api.primary.com.ar");

        /// <summary>This is the default demo endpoint.</summary>
        /// <remarks>You can get a demo username at https://remarkets.primary.ventures.</remarks>
        public static Uri DemoEndpoint => new Uri("https://api.remarkets.primary.com.ar");
        
        /// <summary>
        /// Build a new API object.
        /// </summary>
        public Api(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        private readonly Uri _baseUri;

        #region Login

        public string AccessToken { get; private set; }

        /// <summary>
        /// Initialize the specified environment.
        /// </summary>
        /// <param name="username">User used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
        /// <returns></returns>
        public async Task Login(string username, string password)
        {
            var uri = new Uri(_baseUri, "/auth/getToken");
            
            await uri.ToString().PostToUrlAsync(null, "*/*", 
                                                request =>
                                                {
                                                    request.Headers.Add("X-Username", username);
                                                    request.Headers.Add("X-Password", password);
                                                },
                                                response =>
                                                {
                                                    AccessToken = response.Headers["X-Auth-Token"];
                                                }
            );
        }

        public const string DemoUsername = "naicigam2046";
        public const string DemoPassword = "nczhmL9@";
        public const string DemoAccount = "REM2046";
        
        #endregion

        #region Instruments information

        /// <summary>
        /// Get all instruments currently traded on the exchange.
        /// </summary>
        /// <returns>Instruments information.</returns>
        public async Task< IEnumerable<Instrument> > GetAllInstruments()
        {
            var uri = new Uri(_baseUri, "/rest/instruments/all");
            var response = await uri.ToString().GetJsonFromUrlAsync( request =>
            {
                request.Headers.Add("X-Auth-Token", AccessToken);
            });
            
            var data = JsonConvert.DeserializeObject<GetAllInstrumentsResponse>(response);
            return data.Instruments.Select(i => i.InstrumentId);
        }

        private class GetAllInstrumentsResponse
        {
            public class InstrumentEntry
            {
                [JsonProperty("instrumentId")]
                public Instrument InstrumentId { get; set; }
            }

            [JsonProperty("instruments")]
            public List<InstrumentEntry> Instruments { get; set; }
        }

        #endregion

        #region Historical data
        
        /// <summary>
        /// Get historical trades for a specific instrument.
        /// </summary>
        /// <param name="instrument">Instrument to get information for.</param>
        /// <param name="dateFrom">First date of trading information.</param>
        /// <param name="dateTo">Last date of trading information.</param>
        /// <returns>Trade information for the instrument in the specified period.</returns>
        public async Task< IEnumerable<Trade> > GetHistoricalTrades(Instrument instrument, 
                                                                    DateTime dateFrom, 
                                                                    DateTime dateTo)
        {
            var uri = new Uri(_baseUri, "/rest/data/getTrades");

            var response = await uri.ToString()
                                    .AddQueryParam("marketId", instrument.Market)
                                    .AddQueryParam("symbol", instrument.Symbol)
                                    .AddQueryParam("dateFrom", dateFrom.ToString("yyyy-MM-dd"))
                                    .AddQueryParam("dateTo", dateTo.ToString("yyyy-MM-dd"))
                                    .GetJsonFromUrlAsync( request =>
                                    {
                                        request.Headers.Add("X-Auth-Token", AccessToken);
                                    });
            
            var data = JsonConvert.DeserializeObject<GetTradesResponse>(response);
            return data.Trades;
        }

        private class GetTradesResponse
        {
            [JsonProperty("trades")]
            public List<Trade> Trades { get; set; }
        }

        #endregion
        
        #region Market data sockets

        /// <summary>
        /// Create a Market Data web socket to receive real-time market data.
        /// </summary>
        /// <param name="instruments">Instruments to watch.</param>
        /// <param name="entries">Market data entries to watch.</param>
        /// <param name="level"></param>
        /// <param name="depth">Depth of the book.</param>
        /// <returns>The market data web socket.</returns>
        public MarketDataWebSocket CreateMarketDataSocket(IEnumerable<Instrument> instruments, 
                                                          IEnumerable<Entry> entries,
                                                          uint level, uint depth
        )
        {
            return CreateMarketDataSocket( instruments, entries, level, depth, new CancellationToken() );
        }

        /// <summary>
        /// Create a Market Data web socket to receive real-time market data.
        /// </summary>
        /// <param name="instruments">Instruments to watch.</param>
        /// <param name="entries">Market data entries to watch.</param>
        /// <param name="level"></param>
        /// <param name="depth">Depth of the book.</param>
        /// <param name="cancellationToken">Custom cancellation token to end the socket task.</param>
        /// <returns>The market data web socket.</returns>
        public MarketDataWebSocket CreateMarketDataSocket(IEnumerable<Instrument> instruments, 
                                                          IEnumerable<Entry> entries,
                                                          uint level, uint depth,
                                                          CancellationToken cancellationToken
        )
        {
            var wsScheme = (_baseUri.Scheme == "https" ? "wss" : "ws");
            var url = new UriBuilder(_baseUri) { Scheme = wsScheme };

            var marketDataToRequest = new MarketDataInfo()
            {
                Depth = depth,
                Entries = entries.ToArray(),
                Level = level,
                Products = instruments.ToArray()
            };

            return new MarketDataWebSocket(marketDataToRequest, url.Uri, AccessToken, cancellationToken);
        }

        #endregion

        #region Order data sockets

        /// <summary>
        /// Create a Order Data web socket to receive real-time orders data.
        /// </summary>
        /// <param name="accounts">Accounts to get order events from.</param>
        /// <returns>The order data web socket.</returns>
        public OrderDataWebSocket CreateOrderDataSocket(IEnumerable<string> accounts)
        {
            return CreateOrderDataSocket( accounts, new CancellationToken() );
        }

        /// <summary>
        /// Create a Market Data web socket to receive real-time market data.
        /// </summary>
        /// <param name="accounts">Accounts to get order events from.</param>
        /// <param name="cancellationToken">Custom cancellation token to end the socket task.</param>
        /// <returns>The order data web socket.</returns>
        public OrderDataWebSocket CreateOrderDataSocket(IEnumerable<string> accounts,
                                                        CancellationToken cancellationToken
        )
        {
            var wsScheme = (_baseUri.Scheme == "https" ? "wss" : "ws");
            var url = new UriBuilder(_baseUri) { Scheme = wsScheme };

            var request = new Request
            {
                Accounts = accounts.Select(a => new OrderStatus.AccountId() { Id = a } ).ToArray()
            };

            return new OrderDataWebSocket(request, url.Uri, AccessToken, cancellationToken);
        }

        #endregion

        #region Orders

        /// <summary>
        /// Send an order to the specific account.
        /// </summary>
        /// <param name="account">Account to send the order to.</param>
        /// <param name="order">Order to send.</param>
        /// <returns>Order identifier.</returns>
        public async Task<OrderId> SubmitOrder(string account, Order order)
        {
            var uri = new Uri(_baseUri, "/rest/order/newSingleOrder").ToString();

            uri = uri.AddQueryParam("marketId", "ROFX")
                     .AddQueryParam("symbol", order.Instrument.Symbol)
                     .AddQueryParam("price", order.Price)
                     .AddQueryParam("orderQty", order.Quantity)
                     .AddQueryParam("ordType", order.Type.ToApiString())
                     .AddQueryParam("side", order.Side.ToApiString())
                     .AddQueryParam("timeInForce", order.Expiration.ToApiString())
                     .AddQueryParam("account", account)
                     .AddQueryParam("cancelPrevious", order.CancelPrevious)
                     .AddQueryParam("iceberg", order.Iceberg)
                     .AddQueryParam("expireDate", order.ExpirationDate.ToString("yyyyMMdd"));

            if (order.Iceberg)
            {
                uri = uri.AddQueryParam("displayQty", order.DisplayQuantity);
            }
            
            var jsonResponse = await uri.GetJsonFromUrlAsync(
                                                    request =>
                                                    {
                                                        request.Headers.Add("X-Auth-Token", AccessToken);
                                                    }
            );
            
            var response = JsonConvert.DeserializeObject<OrderIdResponse>(jsonResponse);
            if (response.Status == Status.Error)
            {
                throw new Exception($"{response.Message} ({response.Description})");
            }
            
            return new OrderId()
            { 
                ClientOrderId = response.Order.ClientId,
                Proprietary = response.Order.Proprietary
            };
        }
        
        /// <summary>
        /// Get order information from identifier.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        /// <returns>Order information.</returns>
        public async Task<OrderStatus> GetOrderStatus(OrderId orderId)
        {
            var uri = new Uri(_baseUri, "/rest/order/id").ToString();
            uri = uri.AddQueryParam("clOrdId", orderId.ClientOrderId)
                     .AddQueryParam("proprietary", orderId.Proprietary);

            var jsonResponse = await uri.GetJsonFromUrlAsync(
                                                    request =>
                                                    {
                                                        request.Headers.Add("X-Auth-Token", AccessToken);
                                                    }
            );

            var response = JsonConvert.DeserializeObject<GetOrderResponse>(jsonResponse);
            if (response.Status == Status.Error)
            {
                throw new Exception($"{response.Message} ({response.Description})");
            }

            return response.Order;
        }

        /// <summary>
        /// Cancel an order.
        /// </summary>
        /// <param name="orderId">Order identifier to cancel.</param>
        public async Task CancelOrder(OrderId orderId)
        {
            var uri = new Uri(_baseUri, "/rest/order/cancelById").ToString();
            uri = uri.AddQueryParam("clOrdId", orderId.ClientOrderId)
                     .AddQueryParam("proprietary", orderId.Proprietary);

            var jsonResponse = await uri.GetJsonFromUrlAsync(
                                                    request =>
                                                    {
                                                        request.Headers.Add("X-Auth-Token", AccessToken);
                                                    }
            );
            var response = JsonConvert.DeserializeObject<OrderIdResponse>(jsonResponse);
            //if (response.Status == Status.Error)
            //{
            //    throw new Exception($"{response.Message} ({response.Description})");
            //}
        }

        private struct OrderIdResponse
        {
            [JsonProperty("status")]
            public string Status;
            
            [JsonProperty("message")]
            public string Message;

            [JsonProperty("description")]
            public string Description;
            
            public struct Id
            { 
                [JsonProperty("clientId")]
                public ulong ClientId { get; set; }

                [JsonProperty("proprietary")]
                public string Proprietary { get; set; }
            }

            [JsonProperty("order")]
            public Id Order;
        }

        private struct GetOrderResponse
        {
            [JsonProperty("status")]
            public string Status;
            
            [JsonProperty("message")]
            public string Message;

            [JsonProperty("description")]
            public string Description;

            [JsonProperty("order")]
            public OrderStatus Order;
        }

        #endregion

        #region Constants

        private static class Status
        {
            public const string Error = "ERROR";
        }

        #endregion
    }
}
