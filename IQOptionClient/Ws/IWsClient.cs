using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IQOptionClient.Ws
{
    public interface IWsClient
    {

    }


    public class WsClient : IWsClient, IDisposable
    {
        private ISubject<string> _messageSubject;
        private IConnectableObservable<string> _messsagObservable;

        private IDisposable _temporalActionSubsc;
        private readonly ClientWebSocket _webSocketClient;

        public WsClient()
        {
            var cookies = new CookieContainer();

            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform", "9"));

            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform_version", "1009.13.5397.release"));

            _webSocketClient = new ClientWebSocket()
            {
                Options = { Cookies = cookies }
            };

        }

        public async Task Connect(string ssid)
        {
            _messageSubject = new Subject<string>();
            // TODO check cold
            _messsagObservable = _messageSubject.Publish();
            _temporalActionSubsc = _messageSubject.Subscribe(async message => await TemporalAction(message));

            var serverUri = new Uri("wss://iqoption.com/echo/websocket");
            await _webSocketClient.ConnectAsync(serverUri, CancellationToken.None);

            //TODO retornar observable
            Listen();

            await SendSsid(ssid, _webSocketClient);

            //await Task.Delay(Timeout.Infinite);
        }


        private async Task Listen()
        {
            var buffer = new byte[1024];

            while (_webSocketClient.State == WebSocketState.Open)
            {
                var stringResult = new StringBuilder();


                WebSocketReceiveResult result;
                do
                {
                    result = await _webSocketClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await
                            _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    else
                    {
                        var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        stringResult.Append(str);
                    }

                } while (!result.EndOfMessage);

                CallOnMessage(stringResult.ToString());

            }
        }

        private async Task SendMessageAsync(string message, ClientWebSocket ws)
        {
            if (ws.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var SendChunkSize = 1024;
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (SendChunkSize * i);
                var count = SendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                await ws.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, CancellationToken.None);
            }
        }

        private void CallOnMessage(string stringResult)
        {
            _messageSubject.OnNext(stringResult);
        }

        private async Task TemporalAction(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Recieve Message : {message}");
            Console.ForegroundColor = ConsoleColor.White;

            if (message.Contains("profile"))
            {
                Console.WriteLine("oohooo!");
                //await _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }

            if (message.Contains("heartbeat"))
            {
                var deserializedResult = JsonConvert.DeserializeObject<dynamic>(message);

                var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                var secondsSinceEpoch = (long)t.TotalSeconds;
                var epochSeconds = secondsSinceEpoch.ToString();

                var miliSecondsSinceEpoch = (long)t.TotalMilliseconds;
                var epochMilis = miliSecondsSinceEpoch.ToString();


                var heartbeatResponse = new
                {
                    name = "heartbeat",
                    request_id = epochSeconds + "_635264212",
                    msg = new { userTime = epochMilis, heartbeatTime = deserializedResult.msg.ToString() }
                };

                var serializedResponse = JsonConvert.SerializeObject(heartbeatResponse);

                await Sendtext(serializedResponse, _webSocketClient);
            }
        }

        public Task SendSsid(string ssid, ClientWebSocket ws)
        {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var secondsSinceEpoch = (long)t.TotalSeconds;
            var epochSeconds = secondsSinceEpoch.ToString();
            var message = new { name = "ssid", msg = ssid, request_id = epochSeconds + "_1772932922", };

            var serializedMessage = JsonConvert.SerializeObject(message);
            return Sendtext(serializedMessage, ws);
        }

        public Task Sendtext(string text, ClientWebSocket ws)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Send Message : {text}");
            Console.ForegroundColor = ConsoleColor.White;

            return SendMessageAsync(text, ws);
        }

        public void Dispose()
        {
            _temporalActionSubsc?.Dispose();

            if (_webSocketClient == null) return;

            if (_webSocketClient.State == WebSocketState.Open)
                _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).GetAwaiter().GetResult();

            _webSocketClient.Dispose();

        }
    }



    public class IqOptionWsMessage
    {
        public IqOptionWsMessage()
        {
            Counter c = new Counter();
            c.ThresholdReached += c_ThresholdReached;

        }

        static void c_ThresholdReached(object sender, EventArgs e)
        {
            Console.WriteLine("The threshold was reached.");
        }

        public string name { get; }
        public string msg { get; }
    }


    public class Counter
    {
        public event EventHandler ThresholdReached;

        public delegate void ThresholdReachedEventHandler(object sender, ThresholdReachedEventArgs e);

        protected virtual void OnThresholdReached(EventArgs e)
        {
            EventHandler handler = ThresholdReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        public class ThresholdReachedEventArgs : EventArgs
        {
            public int Threshold { get; set; }
            public DateTime TimeReached { get; set; }
        }

        // provide remaining implementation for the class
    }
}
