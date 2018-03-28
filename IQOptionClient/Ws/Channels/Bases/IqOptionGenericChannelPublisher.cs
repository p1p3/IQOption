using System;
using IQOptionClient.Ws.Channels.Abstractions;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels.Bases
{
    public class IqOptionGenericChannelPublisher<TOutMessage> : IChannelPublisher<TOutMessage, IQOptionMessage>
    {
        private readonly IWsIQClient _wsIqClient;
        private readonly string _channelName;

        public IqOptionGenericChannelPublisher(IWsIQClient wsIqClient, string channelName)
        {
            _wsIqClient = wsIqClient;
            _channelName = channelName;
        }

        public IObservable<IQOptionMessage> SendMessage(TOutMessage message)
        {
            return _wsIqClient.SendMessage(_channelName, message);
        }

        public void Dispose()
        {

        }
    }
}
