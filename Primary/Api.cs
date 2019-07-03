using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Primary.Data;
using ServiceStack;

namespace Primary
{
    public class Api
    {
        public static Uri ProductionEndpoint => new Uri("https://api.primary.com.ar");
        public static Uri DemoEndpoint => new Uri("http://pbcp-remarket.cloud.primary.com.ar");
        
        public Api(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        private readonly Uri _baseUri;

        #region Login

        public string AccessToken { get; set; }

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

        #endregion

        #region Instruments information

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
        
        public MarketDataWebSocket CreateSocket(IEnumerable<Instrument> instruments, 
                                                IEnumerable<Entry> entries,
                                                uint level, uint depth
        )
        {
            var url = new UriBuilder(_baseUri)
            {
                Scheme = "ws"
            };
            return new MarketDataWebSocket(instruments, entries, level, depth, url.Uri, AccessToken);
        }

        #region Orders

        public async Task<uint> SubmitOrder(Order order)
        {
            var uri = new Uri(_baseUri, "/rest/order/newSingleOrder");

            var jsonResponse = await uri.ToString()
                                    .AddQueryParam("marketId", "ROFX")
                                    .AddQueryParam("symbol", order.Symbol)
                                    .AddQueryParam("price", order.Price)
                                    .AddQueryParam("orderQty", order.Quantity)
                                    .AddQueryParam("ordType", order.Type)
                                    .AddQueryParam("side", order.Side)
                                    //.AddQueryParam("timeInForce", order.T)
                                    //.AddQueryParam("account", order.Price)
                                    //.AddQueryParam("cancelPrevious", order.Price)
                                    //.AddQueryParam("iceberg", order.Price)
                                    //.AddQueryParam("expireDate", order.Price)
                                    //.AddQueryParam("displayQty", order.Price)
                                    .GetJsonFromUrlAsync( request =>
                                    {
                                        request.Headers.Add("X-Auth-Token", AccessToken);
                                    });
            
            var response = JsonConvert.DeserializeObject<SubmitOrderResponse>(jsonResponse);

            if (response.Status == Status.Error)
            {
                throw new Exception($"{response.Message} ({response.Description})");
            }
            //return data.Trades;
            return 42;
        }
        
        public async Task<Order> GetOrder(uint orderId)
        {
            return new Order();
        }

        private struct SubmitOrderResponse
        {
            [JsonProperty("status")]
            public string Status;
            
            [JsonProperty("message")]
            public string Message;

            [JsonProperty("description")]
            public string Description;
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
