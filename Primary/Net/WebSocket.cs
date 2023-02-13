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

        public async Task<Task> Start()
        {
            if (OnData == null)
            {
                throw new Exception(ErrorMessages.CallbackNotSet);
            }

            _client.Options.SetRequestHeader("X-Auth-Token", _api.AccessToken);

            await _client.ConnectAsync(_uri, CancelToken);

            // Send data to request
            var jsonRequest = JsonConvert.SerializeObject(_request, _jsonSerializerSettings);

            var outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonRequest));
            await _client.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancelToken);

            // Receive
            var socketTask = Task.Factory.StartNew(ProcessSocketData, CancelToken,
                                                   TaskCreationOptions.LongRunning,
                                                   TaskScheduler.Default
            );

            return socketTask.Unwrap();
        }

        public bool IsRunning { get; private set; }
        public CancellationToken CancelToken { get; }

        public Action<Api, TResponse> OnData { get; set; }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client.Dispose();
            }
        }

        #endregion

        protected async Task SendJsonData(string jsonData)
        {
            var encoded = Encoding.UTF8.GetBytes(jsonData);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            await _client.SendAsync(buffer, WebSocketMessageType.Text, true, CancelToken);
        }

        private async Task ProcessSocketData()
        {
            IsRunning = true;

            // Buffers for received data
            var receivedMessage = new List<byte>();
            var buffer = new byte[9192];

            while (true)
            {
                try
                {
                    // Get data until the complete message is received
                    WebSocketReceiveResult response;
                    do
                    {
                        response = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancelToken);
                        if (response.CloseStatus != null)
                        {
                            throw new Exception(response.CloseStatusDescription);
                        }
                        else if (response.MessageType == WebSocketMessageType.Close)
                        {
                            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        }

                        var segment = new ArraySegment<byte>(buffer, 0, response.Count);
                        receivedMessage.AddRange(segment);

                    } while (!response.EndOfMessage);

                    // Decode the message
                    var messageJson = Encoding.ASCII.GetString(receivedMessage.ToArray());
                    receivedMessage.Clear();

                    // Parse and notify subscriber
                    var responseJson = JObject.Parse(messageJson);
                    if (responseJson.ContainsKey("status") && responseJson.ContainsKey("message") &&
                        responseJson.ContainsKey("description"))
                    {
                        if (responseJson["status"].ToString() == "ERROR")
                        {
                            throw new Exception($"{responseJson["description"]} + [{responseJson["message"]}]");
                        }
                    }
                    else
                    {
                        var data = responseJson.ToObject<TResponse>();
                        OnData(_api, data);
                    }
                }
                catch (OperationCanceledException) { }

                if (CancelToken.IsCancellationRequested)
                {
                    IsRunning = false;
                    CancelToken.ThrowIfCancellationRequested();
                }
            }
        }

        private readonly ClientWebSocket _client = new()
        {
            Options = { KeepAliveInterval = TimeSpan.FromSeconds(30) }
        };

        private readonly TRequest _request;
        private readonly Api _api;
        private readonly Uri _uri;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
    }
}
