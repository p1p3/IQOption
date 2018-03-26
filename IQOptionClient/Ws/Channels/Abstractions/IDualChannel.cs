namespace IQOptionClient.Ws.Channels.Abstractions
{
    public interface IDualChannel<out TInMessage, in TOutMessage> : IChannelListener<TInMessage>, IChannelPublisher<TOutMessage>
    {

    }
}
