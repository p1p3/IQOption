using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using IQOptionClient.Ws.Channels.Abstractions;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOptionClient.Ws.Channels.Bases
{
    public class IqOptionGenericChannelListener<TInMessage> : IChannelListener<TInMessage>
    {
        protected IObservable<IQOptionMessage> ChannelMessagesFeed;

        private readonly string _channelName;

        public IObservable<TInMessage> ChannelFeed { get; }

        public IqOptionGenericChannelListener(IWsIQClient wsIqClient, string channelName, Func<IQOptionMessage, TInMessage> customDeserializeFunction)
        {
            _channelName = channelName;

            ChannelFeed = wsIqClient.MessagesFeed
                .Where(CanProcessIncommingMessage)
                .Map(customDeserializeFunction);
        }

        public IqOptionGenericChannelListener(IWsIQClient wsIqClient, string channelName) :
            this(wsIqClient, channelName, message => JsonConvert.DeserializeObject<TInMessage>(message.Message.ToString()))
        {

        }

        private bool CanProcessIncommingMessage(IQOptionMessage message)
        {
            var canProcess = message.ChannelName.Equals(_channelName, StringComparison.OrdinalIgnoreCase);
            return canProcess;
        }


        public void Dispose()
        {

        }
    }
}
