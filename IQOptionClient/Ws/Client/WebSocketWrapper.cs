using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IQOptionClient.Ws.Client
{

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

        protected virtual void OnConnectedDispatcher(EventArgs e)
        {
            RunInTask(() => OnConnected?.Invoke(this, e));
        }

        protected virtual void OnDisconnectedDispatcher(EventArgs e)
        {
            RunInTask(() => OnDisconnected?.Invoke(this, e));
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

            OnConnectedDispatcher(new EventArgs());
            await StartListen();
        }

        public Task SendMessage(string message)
        {
            return _ws.SendMessageAsync(message, _cancellationToken);
        }

        private Task StartListen()
        {
            RunInTask(async () =>
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
                                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _cancellationToken);
                                OnDisconnectedDispatcher(new EventArgs());
                            }
                            else
                            {
                                var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                                stringResult.Append(str);
                            }

                            _cancellationToken.ThrowIfCancellationRequested();
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
            });

            return Task.CompletedTask;
        }

        private void RunInTask(Action action)
        {
            Task.Factory.StartNew(action, _cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
        }

        public void Dispose()
        {
            _ws?.Dispose();
            OnDisconnectedDispatcher(new EventArgs());
        }
    }


}