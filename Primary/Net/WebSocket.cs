using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Primary.Net
{
    public class WebSocket<TRequest, TResponse> : IDisposable
    {
        internal WebSocket(Api api, TRequest request, CancellationToken cancelToken,
            JsonSerializerSettings jsonSerializerSettings = null)
        {
            _api = api;
            _request = request;
            CancelToken = cancelToken;

            if (jsonSerializerSettings == null)
            {
                jsonSerializerSettings = new JsonSerializerSettings();
            }
            _jsonSerializerSettings = jsonSerializerSettings;

            var wsScheme = (_api.BaseUri.Scheme == "https" ? "wss" : "ws");
            var uriBuilder = new UriBuilder(_api.BaseUri) { Scheme = wsScheme };
            _uri = uriBuilder.Uri;
        }

        public async Task Start()
        {
            if (OnData == null)
            {
                throw new Exception(ErrorMessages.CallbackNotSet);
            }

            var factory = new Func<Uri, CancellationToken, Task<WebSocket>>(async (uri, token) =>
            {
                var client = new ClientWebSocket
                {
                    Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(30)
                    }
                };
                client.Options.SetRequestHeader("X-Auth-Token", _api.AccessToken);

                await client.ConnectAsync(uri, CancelToken).ConfigureAwait(false);
                return client;
            });

            Exception closedByException = null;

            var logger = _loggerFactory.CreateLogger<WebsocketClient>();
            _client = new WebsocketClient(_uri, logger, factory);

            _client.ReconnectionHappened.Subscribe(info =>
            {
            var jsonRequest = JsonConvert.SerializeObject(_request, _jsonSerializerSettings);
                _client.Send(jsonRequest);
                logger.LogInformation("Reconnection happened, type: {type}, url: {url}", info.Type, _client.Url);
            });

            _client.DisconnectionHappened.Subscribe(info =>
            {
                IsRunning = false;
                logger.LogWarning("Disconnection happened, type: {type}, exception: {exception}", info.Type, info.Exception);
                closedByException = info.Exception;
                _exitEvent.Set();
            });

            _client.MessageReceived.Subscribe(msg =>
            {
                logger.LogInformation("Message received: {message}", msg);
                OnMessageReceived(msg);
            });

            CancelToken.Register(() =>
            {
                _client.Stop(WebSocketCloseStatus.NormalClosure, "Closed by handler.");
            });

            await _client.StartOrFail();
            IsRunning = true;
            _exitEvent.WaitOne();

            if (closedByException != null)
            {
                throw closedByException;
        }

            CancelToken.ThrowIfCancellationRequested();
        }

        public bool IsRunning { get; private set; }
        public CancellationToken CancelToken { get; }

        private readonly ManualResetEvent _exitEvent = new(false);

        public Action<Api, TResponse> OnData { get; set; }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_client != null && disposing)
            {
                _client.Dispose();
            }
        }

        #endregion

        protected void SendJsonData(string jsonData)
        {
            _client.Send(jsonData);
        }

        private void OnMessageReceived(ResponseMessage receivedMessage)
                        {
                    // Decode the message
            var messageJson = receivedMessage.Text;

                    // Parse and notify subscriber
                    var responseJson = JObject.Parse(messageJson);
                    if (responseJson.ContainsKey("status") && responseJson.ContainsKey("message") &&
                        responseJson.ContainsKey("description"))
                    {
                        if (responseJson["status"].ToString() == "ERROR")
                        {
                            throw new Exception($"{responseJson["description"]} / [{responseJson["message"]}]");
                        }
                    }
                    else
                    {
                        var data = responseJson.ToObject<TResponse>();
                        OnData(_api, data);
                    }
                }

        private IWebsocketClient _client;
        private readonly ILoggerFactory _loggerFactory;

        private readonly TRequest _request;
        private readonly Api _api;
        private readonly Uri _uri;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
    }
}
