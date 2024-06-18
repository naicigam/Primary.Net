using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Primary.Data;
using Primary.Data.Orders;
using Primary.Serialization;
using Primary.WebSockets;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Primary
{
    public class Api
    {
        /// <summary>This is the default production endpoint.</summary>
        public static Uri ProductionEndpoint => new("https://api.primary.com.ar");

        /// <summary>This is the default demo endpoint.</summary>
        /// <remarks>You can get a demo username at https://remarkets.primary.ventures.</remarks>
        public static Uri DemoEndpoint => new("https://api.remarkets.primary.com.ar");

        /// <summary>
        /// Build a new API object.
        /// </summary>
        public Api(Uri baseUri, HttpClient httpClient = null, ILoggerFactory loggerFactory = null)
        {
            BaseUri = baseUri;
            HttpClient = httpClient ?? new HttpClient()
            {
                DefaultRequestVersion = new Version(2, 0)
            };

            _loggerFactory = loggerFactory ?? new NullLoggerFactory();
            _logger = _loggerFactory.CreateLogger<Api>();
        }

        public Uri BaseUri { get; private set; }
        public HttpClient HttpClient { get; private set; }

        #region Login

        public string AccessToken { get; private set; }

        /// <summary>
        /// Initialize the specified environment.
        /// </summary>
        /// <param name="username">User used for authentication.</param>
        /// <param name="password">Password used for authentication.</param>
        /// <returns></returns>
        public async Task<bool> Login(string username, string password)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Logging to {BaseUri} with user {Username}", BaseUri, username);
            }

            var uri = new Uri(BaseUri, "/auth/getToken");

            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("X-Username", username);
            HttpClient.DefaultRequestHeaders.Add("X-Password", password);

            var result = await HttpClient.PostAsync(uri, new StringContent(string.Empty));

            if (result.IsSuccessStatusCode)
            {
                AccessToken = result.Headers.GetValues("X-Auth-Token").FirstOrDefault();
                HttpClient.DefaultRequestHeaders.Clear();
                HttpClient.DefaultRequestHeaders.Add("X-Auth-Token", AccessToken);
            }

            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Logout from server
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Logout()
        {
            var uri = new Uri(BaseUri, "/auth/removeToken");
            var result = await HttpClient.GetAsync(uri);

            if (result.IsSuccessStatusCode)
            {
                AccessToken = null;
            }

            return result.IsSuccessStatusCode;
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
        public async Task<IEnumerable<Instrument>> GetAllInstruments()
        {
            var uri = new Uri(BaseUri, "/rest/instruments/details");
            var response = await HttpClient.GetStringAsync(uri);

            var data = JsonConvert.DeserializeObject<GetAllInstrumentsResponse>(response);
            return data.Instruments;
        }

        private class GetAllInstrumentsResponse
        {
            [JsonProperty("instruments")]
            public List<Instrument> Instruments { get; set; }
        }

        #endregion

        #region Historical data

        /// <summary>
        /// Get historical trades for a specific instrument.
        /// </summary>
        /// <param name="instrumentId">Instrument to get information for.</param>
        /// <param name="dateFrom">First date of trading information.</param>
        /// <param name="dateTo">Last date of trading information.</param>
        /// <returns>Trade information for the instrument in the specified period.</returns>
        public async Task<IEnumerable<Trade>> GetHistoricalTrades(InstrumentId instrumentId,
                                                                    DateTime dateFrom,
                                                                    DateTime dateTo)
        {
            var builder = new UriBuilder(BaseUri + "/rest/data/getTrades");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["marketId"] = instrumentId.Market;
            query["symbol"] = instrumentId.Symbol;
            query["dateFrom"] = dateFrom.ToString("yyyy-MM-dd");
            query["dateTo"] = dateTo.ToString("yyyy-MM-dd");
            builder.Query = query.ToString();

            var response = await HttpClient.GetStringAsync(builder.Uri);
            var data = JsonConvert.DeserializeObject<GetTradesResponse>(response);

            if (data.Status == Status.Error)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Error when getting historical trades: {Message}, {Description}", data.Message, data.Description);
                }

                throw new Exception($"{data.Message} ({data.Description})");
            }

            return data.Trades;
        }

        private class GetTradesResponse
        {
            [JsonProperty("status")]
            public string Status;

            [JsonProperty("message")]
            public string Message;

            [JsonProperty("description")]
            public string Description;

            [JsonProperty("trades")]
            public List<Trade> Trades { get; set; }

            public GetTradesResponse()
            {
                Status = null;
                Message = null;
                Description = null;
            }
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
        public MarketDataWebSocket CreateMarketDataSocket(IEnumerable<InstrumentId> instruments,
                                                          IEnumerable<Entry> entries,
                                                          uint level, uint depth
        )
        {
            return CreateMarketDataSocket(instruments, entries, level, depth, new CancellationToken());
        }

        /// <summary>
        /// Create a Market Data web socket to receive real-time market data.
        /// </summary>
        /// <param name="instrumentIds">Instruments to watch.</param>
        /// <param name="entries">Market data entries to watch.</param>
        /// <param name="level">Real-time message update time.
        ///     <list type="table">
        ///         <listheader> <term>Level</term> <description>Update time (ms)</description> </listheader>
        ///         <item> <term>1</term> <description>100</description> </item>
        ///         <item> <term>2</term> <description>500</description> </item>
        ///         <item> <term>3</term> <description>1000</description> </item>
        ///         <item> <term>4</term> <description>3000</description> </item>
        ///         <item> <term>5</term> <description>6000</description> </item>
        ///     </list>
        /// </param>
        /// <param name="depth">Depth of the book.</param>
        /// <param name="cancellationToken">Custom cancellation token to end the socket task.</param>
        /// <returns>The market data web socket.</returns>
        public MarketDataWebSocket CreateMarketDataSocket(IEnumerable<InstrumentId> instrumentIds,
                                                          IEnumerable<Entry> entries,
                                                          uint level, uint depth,
                                                          CancellationToken cancellationToken
        )
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Creating market data socket: @{Instruments}, @{Entries}, {Level}, {Depth}",
                    instrumentIds, entries, level, depth);
            }

            var marketDataToRequest = new MarketDataInfo()
            {
                Depth = depth,
                Entries = entries.ToArray(),
                Level = level,
                Products = instrumentIds.ToArray()
            };

            JsonSerializerSettings instrumentsSerializationSettings = new()
            {
                Culture = CultureInfo.InvariantCulture,
                ContractResolver = new StrictTypeContractResolver(typeof(InstrumentId))
            };

            return new MarketDataWebSocket(this, marketDataToRequest, cancellationToken,
                instrumentsSerializationSettings, _loggerFactory);
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
            return CreateOrderDataSocket(accounts, new CancellationToken());
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
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Creating order data socket: @{Accounts}", accounts);
            }

            var request = new OrderDataRequest
            {
                Accounts = accounts.Select(a => new OrderStatus.AccountId() { Id = a }).ToArray()
            };

            return new OrderDataWebSocket(this, request, cancellationToken, null, _loggerFactory);
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
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Submitting order: @{Order}", order);
            }

            var builder = new UriBuilder(BaseUri + "/rest/order/newSingleOrder");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["marketId"] = "ROFX";
            query["symbol"] = order.InstrumentId.Symbol;
            query["price"] = order.Price?.ToString(CultureInfo.InvariantCulture);
            query["orderQty"] = order.Quantity.ToString();
            query["ordType"] = order.Type.ToApiString();
            query["side"] = order.Side.ToApiString();
            query["timeInForce"] = order.Expiration.ToApiString();
            query["account"] = account;
            query["cancelPrevious"] = JsonConvert.SerializeObject(order.CancelPrevious);
            query["iceberg"] = JsonConvert.SerializeObject(order.Iceberg);

            if (order.Expiration == Expiration.GoodTillDate)
            {
                query["expireDate"] = order.ExpirationDate.ToString("yyyyMMdd");
            }

            if (order.Iceberg)
            {
                query["displayQty"] = order.DisplayQuantity.ToString(CultureInfo.InvariantCulture);
            }
            builder.Query = query.ToString();

            var jsonResponse = await HttpClient.GetStringAsync(builder.Uri);

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

            var builder = new UriBuilder(BaseUri + "/rest/order/id");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["clOrdId"] = orderId.ClientOrderId;
            query["proprietary"] = orderId.Proprietary;
            builder.Query = query.ToString();

            var jsonResponse = await HttpClient.GetStringAsync(builder.Uri);

            var response = JsonConvert.DeserializeObject<GetOrderResponse>(jsonResponse);
            if (response.Status == Status.Error)
            {
                throw new Exception($"{response.Message} ({response.Description})");
            }

            return response.Order;
        }

        /// <summary>
        /// Updates the order quantity and price.
        /// </summary>
        /// <param name="orderId">The id of the order to update.</param>
        /// <param name="newQuantity">The new order quantity.</param>
        /// <param name="newPrice">The new order price.</param>
        /// <returns>Order identifier.</returns>
        public async Task<OrderId> UpdateOrder(OrderId orderId, decimal newQuantity, decimal? newPrice)
        {
            var builder = new UriBuilder(BaseUri + "/rest/order/replaceById");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["clOrdId"] = orderId.ClientOrderId;
            query["proprietary"] = orderId.Proprietary;
            query["orderQty"] = newQuantity.ToString();

            if (newPrice != null)
            {
                query["price"] = newPrice?.ToString(CultureInfo.InvariantCulture);
            }

            builder.Query = query.ToString();

            var jsonResponse = await HttpClient.GetStringAsync(builder.Uri);

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
        /// Cancel an order.
        /// </summary>
        /// <param name="orderId">Order identifier to cancel.</param>
        public async Task CancelOrder(OrderId orderId)
        {
            var builder = new UriBuilder(BaseUri + "/rest/order/cancelById");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["clOrdId"] = orderId.ClientOrderId;
            query["proprietary"] = orderId.Proprietary;
            builder.Query = query.ToString();

            var jsonResponse = await HttpClient.GetStringAsync(builder.Uri);

            var response = JsonConvert.DeserializeObject<StatusResponse>(jsonResponse);
            if (response.Status == Status.Error)
            {
                throw new Exception($"{response.Message} ({response.Description})");
            }
        }

        /// <summary>
        /// Get all the active orders for a specific account.
        /// </summary>
        /// <param name="accountId">Account to get orders from.</param>
        public async Task<IEnumerable<OrderStatus>> GetActiveOrderStatuses(string accountId)
        {
            var builder = new UriBuilder(BaseUri + "/rest/order/actives");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["accountId"] = accountId;
            builder.Query = query.ToString();

            var jsonResponse = await HttpClient.GetStringAsync(builder.Uri);

            var response = JsonConvert.DeserializeObject<OrdersStatusResponse>(jsonResponse);
            if (response.Status == Status.Error)
            {
                throw new Exception($"{response.Message})");
            }
            return response.Orders;
        }

        private struct OrdersStatusResponse
        {
            [JsonProperty("status")]
            public string Status;

            [JsonProperty("message")]
            public string Message;

            [JsonProperty("orders")]
            public IEnumerable<OrderStatus> Orders;

            public OrdersStatusResponse()
            {
                Status = null;
                Orders = null;
            }
        }

        private struct StatusResponse
        {
            [JsonProperty("status")]
            public string Status;

            [JsonProperty("message")]
            public string Message;

            [JsonProperty("description")]
            public string Description;

            public StatusResponse()
            {
                Status = null;
                Message = null;
                Description = null;
            }
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
                public string ClientId { get; set; }

                [JsonProperty("proprietary")]
                public string Proprietary { get; set; }
            }

            [JsonProperty("order")]
            public Id Order;

            public OrderIdResponse()
            {
                Status = null;
                Message = null;
                Description = null;
                Order = new Id();
            }
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
            public OrderStatus Order { get; set; }

            public GetOrderResponse()
            {
                Status = null;
                Message = null;
                Description = null;
            }
        }

        #endregion

        #region Accounts

        public async Task<IEnumerable<Account>> GetAccounts()
        {
            var builder = new UriBuilder(BaseUri + "rest/accounts");

            var jsonResponse = await HttpClient.GetStringAsync(builder.Uri);

            var response = JsonConvert.DeserializeObject<AccountsResponse>(jsonResponse);
            if (response.Status == Status.Error)
            {
                throw new Exception($"{response.Message} ({response.Description})");
            }
            return response.Accounts;
        }

        public class AccountsResponse
        {
            [JsonProperty("status")]
            public string Status;

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("accounts")]
            public List<Account> Accounts { get; set; }
        }

        public async Task<AccountStatement> GetAccountStatement(string accountId)
        {
            var uri = new Uri(BaseUri, "/rest/risk/accountReport/" + accountId);
            var jsonResponse = await HttpClient.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<GetAccountStatementResponse>(jsonResponse);

            //if (response.Status == Status.Error)
            //{
            //throw new Exception($"{response.Message} ({response.Description})");
            //}

            return response.AccountStatement;
        }

        private struct GetAccountStatementResponse
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("accountData")]
            public AccountStatement AccountStatement { get; set; }
        }

        #endregion

        #region Positions

        public async Task<IEnumerable<Position>> GetAccountPositions(string accountName)
        {
            var builder = new UriBuilder(BaseUri + $"rest/risk/position/getPositions/{accountName}");
            var jsonResponse = await HttpClient.GetStringAsync(builder.Uri);

            var response = JsonConvert.DeserializeObject<PositionsResponse>(jsonResponse);
            if (response.Status == Status.Error)
            {
                throw new Exception($"{response.Message} ({response.Description})");
            }
            return response.Positions;
        }

        public class PositionsResponse
        {
            [JsonProperty("status")]
            public string Status;

            [JsonProperty("message")]
            public string Message;

            [JsonProperty("description")]
            public string Description;

            [JsonProperty("positions")]
            public List<Position> Positions { get; set; }
        }

        #endregion

        #region Constants

        private static class Status
        {
            public const string Error = "ERROR";
        }

        #endregion

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<Api> _logger;
    }
}
