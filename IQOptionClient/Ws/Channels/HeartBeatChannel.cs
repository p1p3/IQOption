using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using System.Reactive.Threading.Tasks;
using System.Text;
using IQOptionClient.Time;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOptionClient.Ws.Channels
{
    public class HeartBeatChannel : IQOptionChannel<HeartBeatInputMessage, HeartBeatInputMessage>
    {
        private readonly IEpoch _epoch;
        private readonly IWsIQClient _wsIqClient;
        private readonly IDisposable _wsMessagesSubscription;

        public override string ChannelName => "heartbeat";

        public sealed override IObservable<HeartBeatInputMessage> ChannelFeed { get; }
        
        public HeartBeatChannel(IEpoch epoch, IWsIQClient wsIqClient)
        {
            _epoch = epoch;
            _wsIqClient = wsIqClient;

            ChannelFeed = wsIqClient.MessagesFeed
                .Where(this.CanProcessIncommingMessage)
                .Map(message => new HeartBeatInputMessage(long.Parse(message.Message.ToString())));

            _wsMessagesSubscription = ChannelFeed
                .Map(this.SendMessage)
                .Subscribe();
        }


        public override IObservable<IQOptionMessage> SendMessage(HeartBeatInputMessage message)
        {
            var heartbeatResponseMsg = new
            {
                userTime = _epoch.EpochMilliSeconds,
                heartbeatTime = message.HeartbeatTime
            };

            return _wsIqClient.SendMessage(ChannelName, heartbeatResponseMsg)
                .ToObservable();
        }

 

        public override void Dispose()
        {
            base.Dispose();
            //_wsIqClient?.Dispose();
            _wsMessagesSubscription?.Dispose();
        }
    }
}
