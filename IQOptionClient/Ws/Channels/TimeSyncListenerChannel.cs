using IQOptionClient.Ws.Channels.Bases;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels
{
    public class TimeSyncListenerChannel : IqOptionGenericChannelListener<TimeSync>
    {

        private const string ChannelName = "timeSync";

        public TimeSyncListenerChannel(IWsIQClient wsIqClient) : base(wsIqClient,
            ChannelName,
            message => new TimeSync(long.Parse(message.Message.ToString())))
        {

        }

    }
}
