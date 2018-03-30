using System;
using IQOptionClient.Ws.Channels.Abstractions;
using IQOptionClient.Ws.Channels.Bases;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels
{
    public class CandleGeneratedDualChannel : IDualChannel<Candle, IQOptionMessage, CandleSubscription>
    {
        private const string ListenerChannelName = "candle-generated";
        private const string PublisherChannelName = "subscribeMessage";

        private readonly IChannelListener<Candle> _channelListener;
        private readonly IChannelPublisher<dynamic, IQOptionMessage> _channelPublisher;

        public CandleGeneratedDualChannel(IWsIQClient wsIqClient)
        {
            _channelListener = new IqOptionGenericChannelListener<Candle>(wsIqClient, ListenerChannelName);
            _channelPublisher = new IqOptionGenericChannelPublisher<dynamic>(wsIqClient, PublisherChannelName);
        }

        public IObservable<Candle> ChannelFeed => _channelListener.ChannelFeed;

        public IObservable<IQOptionMessage> SendMessage(CandleSubscription message)
        {
            //TODO SUBSCRIBE IS VERY ESPECIFIC the mssage create object and method to subscribe to, maybe subscriber helper           
            var msg = new
            {
                name = ListenerChannelName,
                @params = new

                {
                    routingFilters = new
                    {
                        active_id = (int)message.Active,
                        size = (long)message.SizeInSeconds
                    }
                }
            };

            return _channelPublisher.SendMessage(msg);
        }

        public void Dispose()
        {

        }


    }
}
