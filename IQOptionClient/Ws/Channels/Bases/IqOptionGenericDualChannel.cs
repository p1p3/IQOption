using System;
using System.Reactive.Linq;
using IQOptionClient.Ws.Channels.Abstractions;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels.Bases
{
    public abstract class IqOptionGenericDualChannel<TInMessage, TOutMessage> : IDualChannel<TInMessage, TOutMessage>
    {
        private readonly IChannelPublisher<TOutMessage> _channelPublisher;
        private readonly IChannelListener<TInMessage> _channelListener;

        protected IqOptionGenericDualChannel(IChannelListener<TInMessage> channelListener, IChannelPublisher<TOutMessage> channelPublisher)
        {
            _channelPublisher = channelPublisher;
            _channelListener = channelListener;
        }

        public IObservable<TInMessage> ChannelFeed => _channelListener.ChannelFeed;


        public void Dispose() { }


        public IObservable<IQOptionMessage> SendMessage(TOutMessage message)
        {
            return _channelPublisher.SendMessage(message);
        }
    }
}
