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
    public class HeartBeatChannel : IQOptionChannel<HeartBeatInputMessage, HeartBeatOutputMessage>
    {
        public override string ChannelName => "heartbeat";

        public sealed override IObservable<HeartBeatInputMessage> ChannelFeed { get; }

        public HeartBeatChannel(IWsIQClient wsIqClient) : base(wsIqClient)
        {
            ChannelFeed = base.ChannelMessagesFeed
                .Map(message => new HeartBeatInputMessage(long.Parse(message.Message.ToString())));
        }

        public override IObservable<IQOptionMessage> SendMessage(HeartBeatOutputMessage message)
        {
            return base.WsIqClient.SendMessage(ChannelName, message);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
