using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IQOptionClient.Ws.Client
{
    public interface IWebSocketClient : IDisposable
    {
        event OnMessageEventHandler OnMessage;
        event OnErrorEventHandler OnError;
        event OnConnectedEventHandler OnConnected;
        event OnDisconnectedEventHandler OnDisconnected;

        Task ConnectAsync(string hostUrl, CookieContainer cookies, CancellationToken cancellationToken);

        Task SendMessage(string message);
    }

    public delegate void OnMessageEventHandler(IWebSocketClient wsClient, WsRecievemessageEventArgs e);
    public delegate void OnErrorEventHandler(IWebSocketClient wsClient, WsErrorEventArgs e);
    public delegate void OnConnectedEventHandler(IWebSocketClient wsClient);
    public delegate void OnDisconnectedEventHandler(IWebSocketClient wsClient);
}