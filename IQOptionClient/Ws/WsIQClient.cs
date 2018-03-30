using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using IQOptionClient.Utilities;
using IQOptionClient.Ws.Channels;
using IQOptionClient.Ws.Channels.Abstractions;
using IQOptionClient.Ws.Client;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOptionClient.Ws
{

    public interface IWsIQClient : IDisposable
    {
        IObservable<IQOptionMessage> MessagesFeed { get; }
        IObservable<DateTime> ServerDatetime { get; }
        IObservable<Profile> Profile { get; }
        IObservable<InfoData> Bidsresults { get; }

        IObservable<IWsIQClient> Connect();
        IObservable<IWsIQClient> Connect(CancellationToken cancellationToken);
        IObservable<IQOptionMessage> SendMessage(string channel, dynamic message);
        IObservable<Candle> CreateCandles(Active active, TimeSpan size);
        IObservable<BuyServerModel> PlaceBid(double price, Active active, Direction direction, Balance balance, TimeSpan operationTime);
        IObservable<Profile> Login(string ssid);
    }

    public class WsIQClientRx : IWsIQClient
    {
        private readonly IWebSocketClient _ws;
        private readonly IEpoch _epoch;
        private readonly IRandomNumbers _randomNumbers;


        public IObservable<IQOptionMessage> MessagesFeed { get; }

        private readonly IDisposable _wsMessagesSubscription;
        private readonly IDualChannel<HeartBeatInputMessage, IQOptionMessage, HeartBeatOutputMessage> _heartBeatDualChannel;
        private readonly IDualChannel<Candle, IQOptionMessage, CandleSubscription> _candleGeneratedDualChannel;
        private readonly IChannelPublisher<string, IQOptionMessage> _ssidDualChannel;
        private readonly IChannelListener<TimeSync> _serverTimeSync;
        private readonly IChannelListener<Profile> _profileChannel;
        private readonly IChannelListener<IList<InfoData>> _listInfoDataChannelListener;
        private readonly IChannelPublisher<BuyInputModel, BuyServerModel> _buyV2ChannelPublisher;

        private readonly IConnectableObservable<EventArgs> _onConnection;
        private readonly IDisposable _onConnectionConnection;


        public WsIQClientRx()
        {
            // TODO INJECT
            _ws = new WebSocketWrapper();
            _epoch = new Epoch();
            _randomNumbers = new RandomNumbers();

            MessagesFeed = Observable
                .FromEventPattern<OnMessageEventHandler, WsRecievemessageEventArgs>(
                    h => _ws.OnMessage += h,
                    h => _ws.OnMessage -= h)
                //.ObserveOn(NewThreadScheduler.Default)
                .Map((e) =>
                {
                    var serializedMessage = e.EventArgs.Message;
                    var iQmessage = JsonConvert.DeserializeObject<IQOptionMessage>(serializedMessage);
                    return iQmessage;
                });

            _onConnection = Observable
                .FromEventPattern<OnConnectedEventHandler, EventArgs>(
                    h => _ws.OnConnected += h,
                    h => _ws.OnConnected -= h)
                .Map(e => e.EventArgs)
                .Replay();

            _onConnectionConnection = _onConnection.Connect();

            //TODO DO THIS IN THE IQCLient
            _ssidDualChannel = new SsidPublisherChannel(this);

            _heartBeatDualChannel = new HeartBeatDualChannel(this);

            _wsMessagesSubscription = _heartBeatDualChannel.ChannelFeed
                .Map(heartbeat => _heartBeatDualChannel.SendMessage(new HeartBeatOutputMessage(_epoch.EpochMilliSeconds, heartbeat.HeartbeatTime)))
                .Subscribe();

            _candleGeneratedDualChannel = new CandleGeneratedDualChannel(this);

            _serverTimeSync = new TimeSyncListenerChannel(this);

            _profileChannel = new ProfileListenerChannel(this);

            _listInfoDataChannelListener = new ListInfoDataListenerChannel(this);

            _buyV2ChannelPublisher = new BuyV2Channel(this, _epoch);
        }

        public IObservable<IWsIQClient> Connect(CancellationToken cancellationToken)
        {
            var cookies = new CookieContainer();
            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform", "9"));
            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform_version", "1009.13.5397.release"));

            return _ws.ConnectAsync("wss://iqoption.com/echo/websocket", cookies, cancellationToken)
                   .ToObservable()
                   .FlatMap(Observable.Return(this));
        }

        public IObservable<IWsIQClient> Connect()
        {
            return this.Connect(CancellationToken.None);
        }

        public IObservable<Profile> Login(string ssid)
        {
            //TODO RETURN SECURE WSIWClient instance.
            var profile = this.Profile.Replay();
            using (profile.Connect())
            {
                return _ssidDualChannel.SendMessage(ssid).FlatMap(profile);
            }
        }

        public IObservable<Candle> CreateCandles(Active active, TimeSpan size)
        {
            return _candleGeneratedDualChannel
                .SendMessage(new CandleSubscription(active, size.TotalSeconds))
                .FlatMap(_candleGeneratedDualChannel.ChannelFeed);
        }

        public IObservable<IQOptionMessage> SendMessage(string channel, dynamic message)
        {
            var requestId = $"{_epoch.EpochSeconds}_{_randomNumbers.GenerateValue()}";

            var messageToIq = new IQOptionMessage()
            {
                Message = message,
                ChannelName = channel,
                RequestId = requestId
            };

            var serializedMessage = JsonConvert.SerializeObject(messageToIq);

            return _ws.SendMessage(serializedMessage)
                  .ToObservable()
                  .FlatMap((unit) => Observable.Return(messageToIq));
        }

        public IObservable<BuyServerModel> PlaceBid(double price, Active active, Direction direction, Balance balance, TimeSpan operationTime)
        {
            var inputModel = new BuyInputModel(price, active, direction, balance, operationTime);
            return _buyV2ChannelPublisher
                .SendMessage(inputModel);
        }

        public IObservable<DateTime> ServerDatetime => _serverTimeSync.ChannelFeed.Map(timeSync => _epoch.FromUnixTimeToDateTime(timeSync.ServerTimeStamp));

        public IObservable<Profile> Profile => this._profileChannel.ChannelFeed.Filter(profile => !string.IsNullOrEmpty(profile.Ssid));

        public IObservable<InfoData> Bidsresults => _listInfoDataChannelListener.ChannelFeed.SelectMany(infoData => infoData);

        public void Dispose()
        {
            _ws?.Dispose();
            _wsMessagesSubscription?.Dispose();
            _heartBeatDualChannel?.Dispose();
            _candleGeneratedDualChannel?.Dispose();
            _onConnectionConnection?.Dispose();
        }
    }
}
