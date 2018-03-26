using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IQOptionClient.Time;
using IQOptionClient.Ws.Channels;
using IQOptionClient.Ws.Client;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOptionClient.Ws
{
    //public interface IWsIQClient
    //{

    //}


    //public class WsIQClient : IDisposable
    //{
    //    private ISubject<string> _messageSubject;
    //    private IConnectableObservable<string> _messsagObservable;

    //    private IDisposable _temporalActionSubsc;
    //    private readonly ClientWebSocket _webSocketClient;
    //    private bool _subscribedToCandles = false;

    //    public WsIQClient()
    //    {
    //        var cookies = new CookieContainer();

    //        cookies.Add(new Uri("https://iqoption.com"),
    //            new Cookie("platform", "9"));

    //        cookies.Add(new Uri("https://iqoption.com"),
    //            new Cookie("platform_version", "1009.13.5397.release"));

    //        _webSocketClient = new ClientWebSocket()
    //        {
    //            Options = { Cookies = cookies }
    //        };

    //    }

    //    public async Task Connect(string ssid)
    //    {
    //        _messageSubject = new Subject<string>();
    //        // TODO check cold
    //        _messsagObservable = _messageSubject.Publish();
    //        _temporalActionSubsc = _messageSubject.Subscribe(async message => await TemporalAction(message));

    //        var serverUri = new Uri("wss://iqoption.com/echo/websocket");
    //        await _webSocketClient.ConnectAsync(serverUri, CancellationToken.None);

    //        //TODO retornar observable
    //        Listen();

    //        await SendSsid(ssid, _webSocketClient);



    //        //await Task.Delay(Timeout.Infinite);
    //    }

    //    private async Task Listen()
    //    {
    //        var buffer = new byte[1024];

    //        while (_webSocketClient.State == WebSocketState.Open)
    //        {
    //            var stringResult = new StringBuilder();


    //            WebSocketReceiveResult result;
    //            do
    //            {
    //                result = await _webSocketClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    //                if (result.MessageType == WebSocketMessageType.Close)
    //                {
    //                    await
    //                        _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    //                }
    //                else
    //                {
    //                    var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
    //                    stringResult.Append(str);
    //                }

    //            } while (!result.EndOfMessage);

    //            CallOnMessage(stringResult.ToString());

    //        }
    //    }


    //    private void CallOnMessage(string stringResult)
    //    {
    //        _messageSubject.OnNext(stringResult);
    //    }

    //    private async Task TemporalAction(string message)
    //    {
    //        Console.ForegroundColor = ConsoleColor.Green;
    //        Console.WriteLine($"Recieve Message : {message}");
    //        Console.ForegroundColor = ConsoleColor.White;

    //        if (message.Contains("profile"))
    //        {
    //            Console.WriteLine("oohooo!");
    //            //await _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    //        }

    //        if (message.Contains("heartbeat"))
    //        {
    //            var deserializedResult = JsonConvert.DeserializeObject<dynamic>(message);

    //            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
    //            var secondsSinceEpoch = (long)t.TotalSeconds;
    //            var epochSeconds = secondsSinceEpoch.ToString();

    //            var miliSecondsSinceEpoch = (long)t.TotalMilliseconds;
    //            var epochMilis = miliSecondsSinceEpoch.ToString();


    //            var heartbeatResponse = new
    //            {
    //                name = "heartbeat",
    //                request_id = epochSeconds + "_635264212",
    //                msg = new { userTime = epochMilis, heartbeatTime = deserializedResult.msg.ToString() }
    //            };

    //            var serializedResponse = JsonConvert.SerializeObject(heartbeatResponse);

    //            await Sendtext(serializedResponse, _webSocketClient);
    //        }


    //        if (message.Contains("candle-generated"))
    //        {
    //            _subscribedToCandles = true;
    //        }

    //        if (message.Contains("timeSync"))
    //        {
    //            if (!_subscribedToCandles)
    //                await SubscribeToCandles(_webSocketClient);
    //            // {"name":"sendMessage","request_id":"27","msg":{"name":"get-first-candles","version":"1.0","body":{"active_id":1}}}
    //            // {"name":"subscribeMessage","msg":{"name":"candle-generated","params":{"routingFilters":{"active_id":1,"size":5}}}}
    //            // {"name":"sendMessage","request_id":"53","msg":{"name":"get-candles","version":"2.0","body":{"active_id":1,"size":1,"to":1521683003,"count":1}}}
    //            // {"name":"sendMessage","request_id":"54","msg":{"name":"get-candles","version":"2.0","body":{"active_id":1,"size":5,"from_id":1752072,"to_id":1752524}}}
    //            // {"name":"sendMessage","request_id":"56","msg":{"name":"get-candles","version":"2.0","body":{"active_id":1,"size":1,"from_id":8762397,"to_id":8762596}}}

    //            // R : {"name":"candles","request_id":"53",
    //            // "msg":{"candles":[{"id":8762597,"from":1521682978,"to":1521682979,"open":1.23667,"close":1.23666,"min":1.23666,"max":1.23667,"volume":0}]}
    //            //,"status":2000}


    //            //var heartbeatResponse = new
    //            //{
    //            //    name = "sendMessage",
    //            //    request_id = "100",
    //            //    msg = new
    //            //    {
    //            //        name = "get-candles",
    //            //        version = "2.0",
    //            //        body = new
    //            //        {
    //            //            active_id = 1,
    //            //            size = 1,
    //            //            from_id = 1752072,
    //            //            to_id = 1752524
    //            //        }
    //            //        userTime = epochMilis, heartbeatTime = deserializedResult.msg.ToString()
    //            //    }
    //            //};
    //        }
    //    }

    //    public Task SendSsid(string ssid, ClientWebSocket ws)
    //    {
    //        var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
    //        var secondsSinceEpoch = (long)t.TotalSeconds;
    //        var epochSeconds = secondsSinceEpoch.ToString();
    //        var message = new { name = "ssid", msg = ssid, request_id = epochSeconds + "_1772932922", };

    //        var serializedMessage = JsonConvert.SerializeObject(message);
    //        return Sendtext(serializedMessage, ws);
    //    }

    //    public Task SubscribeToCandles(ClientWebSocket ws)
    //    {

    //        //{"name":"subscribeMessage","msg":{"name":"candle-generated","params":{"routingFilters":{"active_id":1,"size":1}}}}

    //        var message = new
    //        {
    //            name = "subscribeMessage",
    //            msg = new
    //            {
    //                name = "candle-generated",
    //                @params = new

    //                {
    //                    routingFilters = new
    //                    {
    //                        active_id = 1,
    //                        size = 15
    //                    }
    //                }
    //            }
    //        };

    //        var serializedMessage = JsonConvert.SerializeObject(message);
    //        return Sendtext(serializedMessage, ws);
    //    }

    //    public Task Sendtext(string text, ClientWebSocket ws)
    //    {
    //        Console.ForegroundColor = ConsoleColor.Red;
    //        Console.WriteLine($"Send Message : {text}");
    //        Console.ForegroundColor = ConsoleColor.White;

    //        return ws.SendMessageAsync(text);
    //    }

    //    public void Dispose()
    //    {
    //        _temporalActionSubsc?.Dispose();

    //        if (_webSocketClient == null) return;

    //        if (_webSocketClient.State == WebSocketState.Open)
    //            _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).GetAwaiter().GetResult();

    //        _webSocketClient.Dispose();

    //    }
    //}

    public interface IWsIQClient : IDisposable
    {
        IObservable<IQOptionMessage> MessagesFeed { get; }

        Task ConnectAsync(string ssid);
        Task ConnectAsync(string ssid, CancellationToken cancellationToken);
        Task<IQOptionMessage> SendMessage(string channel, dynamic message);
    }

    public class WsIQClientRx : IWsIQClient
    {
        private readonly IWebSocketClient _ws;
        private readonly IEpoch _epoch;

        private readonly Random _rnd = new Random();

        public IObservable<IQOptionMessage> MessagesFeed { get; }

        private IDisposable _wsMessagesSubscription;
        private IDisposable _wsCandlesSubscription;
        private readonly IDisposable _heartBeatChannel;
        private readonly IChannel<Candle, CandleSubscription> _candleGeneratedChannel;

        public WsIQClientRx()
        {
            // TODO INJECT
            _ws = new WebSocketWrapper();
            _epoch = new Epoch();

            MessagesFeed = Observable
                .FromEventPattern<OnMessageEventHandler, WsRecievemessageEventArgs>(
                    h => _ws.OnMessage += h,
                    h => _ws.OnMessage -= h)
                .Select((e) =>
                {
                    var serializedMessage = e.EventArgs.Message;
                    var iQmessage = JsonConvert.DeserializeObject<IQOptionMessage>(serializedMessage);
                    return iQmessage;
                });

            //TODO DO THIS IN THE IQCLient
            _heartBeatChannel = new HeartBeatChannel(_epoch, this);
            _candleGeneratedChannel = new CandleGeneratedChannel(this);

        }

        public async Task ConnectAsync(string ssid, CancellationToken cancellationToken)
        {
            var cookies = new CookieContainer();
            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform", "9"));
            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform_version", "1009.13.5397.release"));

            await _ws.ConnectAsync("wss://iqoption.com/echo/websocket", cookies, cancellationToken);
            await Login(ssid);
        }

        public Task ConnectAsync(string ssid)
        {
            return this.ConnectAsync(ssid, CancellationToken.None);
        }

        #region SSID Channel
        private Task Login(string ssid)
        {
            return this.SendMessage("ssid", ssid);
        }

        #endregion

        public IObservable<Candle> CreateCandles(Active active, int sizeInSeconds)
        {
            return _candleGeneratedChannel
                .SendMessage(new CandleSubscription(active, sizeInSeconds))
                .FlatMap(_candleGeneratedChannel.ChannelFeed);

        }

        public async Task<IQOptionMessage> SendMessage(string channel, dynamic message)
        {
            var requestId = $"{_epoch.EpochSeconds}_{_rnd.Next(1, 99999999)}";

            var messageToIq = new IQOptionMessage()
            {
                Message = message,
                ChannelName = channel,
                RequestId = requestId
            };

            var serializedMessage = JsonConvert.SerializeObject(messageToIq);
            await _ws.SendMessage(serializedMessage);

            return messageToIq;
        }


        public void Dispose()
        {
            _ws?.Dispose();
            _wsCandlesSubscription?.Dispose();
            _wsMessagesSubscription?.Dispose();
            _heartBeatChannel?.Dispose();
            _candleGeneratedChannel?.Dispose();
        }
    }
}
