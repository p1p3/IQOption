using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IQOptionClient.Ws
{

    public delegate void OnMessageEventHandler(IWebSocketClient wsClient, WsRecievemessageEventArgs e);
    public delegate void OnErrorEventHandler(IWebSocketClient wsClient, WsErrorEventArgs e);
    public delegate void OnConnectedEventHandler(IWebSocketClient wsClient);
    public delegate void OnDisconnectedEventHandler(IWebSocketClient wsClient);

    public class WsRecievemessageEventArgs : EventArgs
    {
        public WsRecievemessageEventArgs(string message)
        {
            Message = message;
        }

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


    public interface IWebSocketClient : IDisposable
    {
        event OnMessageEventHandler OnMessage;
        event OnErrorEventHandler OnError;
        event OnConnectedEventHandler OnConnected;
        event OnDisconnectedEventHandler OnDisconnected;

        Task ConnectAsync(string hostUrl, CookieContainer cookies, CancellationToken cancellationToken);

        Task SendMessage(string message);
    }




    public class WebSocketWrapper : IWebSocketClient
    {
        private readonly ClientWebSocket _ws;
        private const int ReceiveChunkSize = 1024;

        private CancellationToken _cancellationToken;

        #region Events 

        public event OnMessageEventHandler OnMessage;
        public event OnErrorEventHandler OnError;
        public event OnConnectedEventHandler OnConnected;
        public event OnDisconnectedEventHandler OnDisconnected;


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

        public WebSocketWrapper()
        {
            _ws = new ClientWebSocket();
            _ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
        }

        public async Task ConnectAsync(string hostUrl, CookieContainer cookies, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _ws.Options.Cookies = cookies;
            var hostUri = new Uri(hostUrl);
            await _ws.ConnectAsync(hostUri, _cancellationToken);
            OnConnectedDispatcher();
            StartListen();
        }

        public Task SendMessage(string message)
        {
            return _ws.SendMessageAsync(message);
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

                    OnMessageDispatcher(new WsRecievemessageEventArgs(stringResult.ToString()));

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


}