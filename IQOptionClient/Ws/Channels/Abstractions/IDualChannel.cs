namespace IQOptionClient.Ws.Channels.Abstractions
{
    public interface IDualChannel<out TInMessage, out TMessageSent, in TOutMessage> : IChannelListener<TInMessage>, IChannelPublisher<TOutMessage, TMessageSent>
    {

    }
}
