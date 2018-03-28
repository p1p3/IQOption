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
using IQOptionClient.Utilities;
using IQOptionClient.Ws.Channels;
using IQOptionClient.Ws.Channels.Abstractions;
using IQOptionClient.Ws.Channels.Bases;
using IQOptionClient.Ws.Client;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOptionClient.Ws
{

    public interface IWsIQClient : IDisposable
    {
        IObservable<IQOptionMessage> MessagesFeed { get; }
        IObservable<DateTime> ServerDatetime { get; }
        Task ConnectAsync(string ssid);
        Task ConnectAsync(string ssid, CancellationToken cancellationToken);
        IObservable<IQOptionMessage> SendMessage(string channel, dynamic message);
        IObservable<InfoData> PlaceBid(double price, Active active, Direction direction);

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
                .Select((e) =>
                {
                    var serializedMessage = e.EventArgs.Message;
                    var iQmessage = JsonConvert.DeserializeObject<IQOptionMessage>(serializedMessage);
                    return iQmessage;
                });

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

        public async Task ConnectAsync(string ssid, CancellationToken cancellationToken)
        {
            var cookies = new CookieContainer();
            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform", "9"));
            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform_version", "1009.13.5397.release"));

            await _ws.ConnectAsync("wss://iqoption.com/echo/websocket", cookies, cancellationToken);

            await _ssidDualChannel.SendMessage(ssid);
        }

        public Task ConnectAsync(string ssid)
        {
            return this.ConnectAsync(ssid, CancellationToken.None);
        }

        public IObservable<Candle> CreateCandles(Active active, int sizeInSeconds)
        {
            return _candleGeneratedDualChannel
                .SendMessage(new CandleSubscription(active, sizeInSeconds))
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

        public IObservable<InfoData> PlaceBid(double price, Active active, Direction direction)
        {
            var inputModel = new BuyInputModel(price, active, direction);
            return _buyV2ChannelPublisher
                 .SendMessage(inputModel)
                 .FlatMap(messageSent =>
                     {
                         return Bidsresults.Where(infoData => infoData.Created == messageSent.CurrentUserTime && infoData.RateFinished);
                     });
        }

        public IObservable<DateTime> ServerDatetime => _serverTimeSync.ChannelFeed.Map(timeSync => _epoch.FromUnixTimeToDateTime(timeSync.ServerTimeStamp));

        public IObservable<Profile> Profile => _profileChannel.ChannelFeed.StartWith();

        private IObservable<InfoData> Bidsresults => _listInfoDataChannelListener.ChannelFeed.SelectMany(infoData => infoData);

        public void Dispose()
        {
            _ws?.Dispose();
            _wsMessagesSubscription?.Dispose();
            _heartBeatDualChannel?.Dispose();
            _candleGeneratedDualChannel?.Dispose();
        }
    }
}
