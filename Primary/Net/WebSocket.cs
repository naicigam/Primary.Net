using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Primary.Net
{
    public class WebSocket<TRequest, TResponse> : IDisposable
    {
        internal WebSocket(TRequest request, Uri url, string accessToken, CancellationToken cancelToken)
        {
            _request = request;
            _url = url;
            _accessToken = accessToken;
            CancelToken = cancelToken;
        }
        
        public async Task<Task> Start()
        {
            _client.Options.SetRequestHeader("X-Auth-Token", _accessToken);

            await _client.ConnectAsync(_url, CancelToken);

            // Send data to request
            var jsonRequest = JsonConvert.SerializeObject(_request);

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

        public Action<TResponse> OnData { get; set; }
        
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

        private async Task ProcessSocketData()
        {
            IsRunning = true;
            
            // Buffers for received data
            var receivedMessage = new List<byte>();
            var buffer = new byte[4096];

            while (true)
            {
                try
                {
                    // Get data until the complete message is received
                    WebSocketReceiveResult response;
                    do
                    {
                        response = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancelToken);
                        receivedMessage.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));

                    } while (!response.EndOfMessage);

                    // Decode the message
                    var messageJson = (new ASCIIEncoding()).GetString(buffer).Substring(0, receivedMessage.Count);
                    var data = JsonConvert.DeserializeObject<TResponse>(messageJson);
                    receivedMessage.Clear();

                    // Notify subscriber
                    OnData(data);
                }
                catch (OperationCanceledException) { }

                if (CancelToken.IsCancellationRequested)
                {
                    IsRunning = false;
                    CancelToken.ThrowIfCancellationRequested();
                }
            }
        }

        private readonly ClientWebSocket _client = new ClientWebSocket();

        private readonly TRequest _request;
        private readonly Uri _url;
        private readonly string _accessToken;
    }
}
