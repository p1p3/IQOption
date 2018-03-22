using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IQOptionClient.Ws
{
    public class WebSocketWrapper : IDisposable
    {
        private readonly ClientWebSocket _ws;
        private readonly CancellationToken _cancellationToken;
        private const int ReceiveChunkSize = 1024;

        #region Events 
        public event OnMessageEventHandler OnMessage;
        public delegate void OnMessageEventHandler(WebSocketWrapper wsClient, WsRecievemessageEventArgs e);

        public event OnErrorEventHandler OnError;
        public delegate void OnErrorEventHandler(WebSocketWrapper wsClient, WsErrorEventArgs e);

        public event OnConnectedEventHandler OnConnected;
        public delegate void OnConnectedEventHandler(WebSocketWrapper wsClient);

        public event OnDisconnectedEventHandler OnDisconnected;
        public delegate void OnDisconnectedEventHandler(WebSocketWrapper wsClient);

        protected virtual void OnMessageDispatcher(WsRecievemessageEventArgs e)
        {
            RunInTask(() => OnMessage?.Invoke(this, e));
        }

        protected virtual void OnErrorDispatcher(WsErrorEventArgs e)
        {
            RunInTask(() => OnError?.Invoke(this, e));
        }

        protected virtual void OnConnectedDispatcher()
        {
            RunInTask(() => OnConnected?.Invoke(this));
        }

        protected virtual void OnDisconnectedDispatcher()
        {
            RunInTask(() => OnDisconnected?.Invoke(this));
        }
        #endregion

        public WebSocketWrapper(CancellationToken cancellationToken)
        {
            _ws = new ClientWebSocket();
            _ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
            _cancellationToken = cancellationToken;
        }

        public async Task ConnectAsync(string hostUrl, CookieContainer cookies)
        {
            _ws.Options.Cookies = cookies;
            var hostUri = new Uri(hostUrl);
            await _ws.ConnectAsync(hostUri, _cancellationToken);
            OnConnectedDispatcher();
            StartListen();
        }

        private async void StartListen()
        {
            var buffer = new byte[ReceiveChunkSize];

            try
            {
                while (_ws.State == WebSocketState.Open)
                {
                    var stringResult = new StringBuilder();

                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            OnDisconnectedDispatcher();
                        }
                        else
                        {
                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            stringResult.Append(str);
                        }

                    } while (!result.EndOfMessage);

                    OnMessageDispatcher(new WsRecievemessageEventArgs(result, stringResult.ToString()));

                }
            }
            catch (Exception e)
            {
                OnErrorDispatcher(new WsErrorEventArgs(e, $"Error reading a message"));
            }
            finally
            {
                Dispose();
            }
        }

        private static void RunInTask(Action action)
        {
            Task.Factory.StartNew(action);
        }

        public void Dispose()
        {
            _ws?.Dispose();
            OnDisconnectedDispatcher();
        }
    }

    public class WsRecievemessageEventArgs : EventArgs
    {
        public WsRecievemessageEventArgs(WebSocketReceiveResult result, string message)
        {
            Result = result;
            Message = message;
        }

        public WebSocketReceiveResult Result { get; set; }
        public string Message { get; set; }
    }

    public class WsErrorEventArgs : EventArgs
    {
        public WsErrorEventArgs(Exception exception, string error)
        {
            Exception = exception;
            Error = error;
        }

        public Exception Exception { get; set; }
        public string Error { get; set; }
    }
}


