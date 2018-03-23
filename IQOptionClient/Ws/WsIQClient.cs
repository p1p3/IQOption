using System;
using System.Net;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IQOptionClient.Ws
{
    public interface IWsIQClient
    {

    }


    public class WsIQClient : IWsIQClient, IDisposable
    {
        private ISubject<string> _messageSubject;
        private IConnectableObservable<string> _messsagObservable;

        private IDisposable _temporalActionSubsc;
        private readonly ClientWebSocket _webSocketClient;
        private bool _subscribedToCandles = false;

        public WsIQClient()
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


            if (message.Contains("candle-generated"))
            {
                _subscribedToCandles = true;
            }

            if (message.Contains("timeSync"))
            {
                if (!_subscribedToCandles)
                    await SubscribeToCandles(_webSocketClient);
                // {"name":"sendMessage","request_id":"27","msg":{"name":"get-first-candles","version":"1.0","body":{"active_id":1}}}
                // {"name":"subscribeMessage","msg":{"name":"candle-generated","params":{"routingFilters":{"active_id":1,"size":5}}}}
                // {"name":"sendMessage","request_id":"53","msg":{"name":"get-candles","version":"2.0","body":{"active_id":1,"size":1,"to":1521683003,"count":1}}}
                // {"name":"sendMessage","request_id":"54","msg":{"name":"get-candles","version":"2.0","body":{"active_id":1,"size":5,"from_id":1752072,"to_id":1752524}}}
                // {"name":"sendMessage","request_id":"56","msg":{"name":"get-candles","version":"2.0","body":{"active_id":1,"size":1,"from_id":8762397,"to_id":8762596}}}

                // R : {"name":"candles","request_id":"53",
                // "msg":{"candles":[{"id":8762597,"from":1521682978,"to":1521682979,"open":1.23667,"close":1.23666,"min":1.23666,"max":1.23667,"volume":0}]}
                //,"status":2000}


                //var heartbeatResponse = new
                //{
                //    name = "sendMessage",
                //    request_id = "100",
                //    msg = new
                //    {
                //        name = "get-candles",
                //        version = "2.0",
                //        body = new
                //        {
                //            active_id = 1,
                //            size = 1,
                //            from_id = 1752072,
                //            to_id = 1752524
                //        }
                //        userTime = epochMilis, heartbeatTime = deserializedResult.msg.ToString()
                //    }
                //};
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

        public Task SubscribeToCandles(ClientWebSocket ws)
        {

            //{"name":"subscribeMessage","msg":{"name":"candle-generated","params":{"routingFilters":{"active_id":1,"size":1}}}}

            var message = new
            {
                name = "subscribeMessage",
                msg = new
                {
                    name = "candle-generated",
                    @params = new

                    {
                        routingFilters = new
                        {
                            active_id = 1,
                            size = 15
                        }
                    }
                }
            };

            var serializedMessage = JsonConvert.SerializeObject(message);
            return Sendtext(serializedMessage, ws);
        }

        public Task Sendtext(string text, ClientWebSocket ws)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Send Message : {text}");
            Console.ForegroundColor = ConsoleColor.White;

            return ws.SendMessageAsync(text);
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


    public class WsIQClientWrapper : IDisposable
    {
        private readonly IWebSocketClient _ws;
        private readonly Random _rnd = new Random();

        private TimeSpan Epoch => DateTime.UtcNow - new DateTime(1970, 1, 1);
        private long EpochSeconds => (long)(Epoch).TotalSeconds;
        private long EpochMilliSeconds => (long)(Epoch).TotalMilliseconds;

        public IObservable<Candle> Candles;
        private IObservable<IQOptionMessage> _wsMessages;

        public IDisposable _wsMessagesSubscription;
        public IDisposable _wsCandlesSubscription;

        public WsIQClientWrapper()
        {
            _ws = new WebSocketWrapper();

            _wsMessages = Observable
                .FromEventPattern<OnMessageEventHandler, WsRecievemessageEventArgs>(
                    h => _ws.OnMessage += h,
                    h => _ws.OnMessage -= h)
                .Select((e) =>
                {
                    var serializedMessage = e.EventArgs.Message;
                    var iQmessage = JsonConvert.DeserializeObject<IQOptionMessage>(serializedMessage);
                    return iQmessage;
                });

            _wsMessagesSubscription = _wsMessages.Subscribe(message => Console.WriteLine(message.ChannelName));

            _wsCandlesSubscription = _wsMessages
                .Where(message => message.ChannelName.Equals("candle-generated", StringComparison.OrdinalIgnoreCase))
                //.Select(message => new Candle(message.Message.))
                .Subscribe(message => Console.WriteLine(message.Message));
        }


        public async Task ConnectAsync(string ssid, CancellationToken cancellationToken)
        {
            var cookies = new CookieContainer();
            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform", "9"));
            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform_version", "1009.13.5397.release"));

            await _ws.ConnectAsync("wss://iqoption.com/echo/websocket", cookies, cancellationToken);

            //TODO  don't do everything here and still missing heartbeat
            await Login(ssid);
            await SubscribeToCandles(1);
        }
        public Task ConnectAsync(string ssid)
        {
            return this.ConnectAsync(ssid, CancellationToken.None);
        }


        private Task Login(string ssid)
        {
            return this.SendMessage("ssid", ssid);
        }

        private Task SendMessage(string channel, dynamic message)
        {
            var requestId = $"{EpochSeconds}_{_rnd.Next(1, 99999999)}";

            var messageToIq = new IQOptionMessage()
            {
                Message = message,
                ChannelName = channel,
                RequestId = requestId
            };

            var serializedMessage = JsonConvert.SerializeObject(messageToIq);
            return _ws.SendMessage(serializedMessage);
        }

        private Task SubscribeToCandles(int activeId)
        {
            //TODO SUBSCRIBE IS VERY ESPECIFIC the mssage create object and method to subscribe to
            var channelName = "subscribeMessage";
            var msg = new
            {
                name = "candle-generated",
                @params = new

                {
                    routingFilters = new
                    {
                        active_id = activeId,
                        size = 15
                    }
                }
            };

            return SendMessage(channelName, msg);
        }


        public void Dispose()
        {
            _ws?.Dispose();
            _wsCandlesSubscription?.Dispose();
            _wsMessagesSubscription?.Dispose();
        }
    }


    public struct Candle
    {
        public Candle(decimal open, decimal close, decimal max, decimal min, decimal ask, decimal bid, decimal volume, int sizeInSeconds)
        {
            Open = open;
            Close = close;
            Max = max;
            Min = min;
            Ask = ask;
            Bid = bid;
            Volume = volume;
            SizeInSeconds = sizeInSeconds;
        }

        public decimal Open { get; }
        public decimal Close { get; }
        public decimal Max { get; }
        public decimal Min { get; }
        public decimal Ask { get; }
        public decimal Bid { get; }
        public decimal Volume { get; }
        public int SizeInSeconds { get; }

    }

    public class IQOptionMessage
    {
        [JsonProperty("name")]
        public string ChannelName { get; set; }

        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("msg")]
        public dynamic Message { get; set; }
    }

}
