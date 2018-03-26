using IQOptionClient.Ws.Channels.Bases;

namespace IQOptionClient.Ws.Channels
{
    public class SsidPublisherChannel : IqOptionGenericChannelPublisher<string>
    {
        private const string ChannelName = "ssid";

        public SsidPublisherChannel(IWsIQClient wsIqClient) : base(wsIqClient, ChannelName)
        {
        }

    }
}
