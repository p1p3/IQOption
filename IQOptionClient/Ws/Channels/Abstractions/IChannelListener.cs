using System;

namespace IQOptionClient.Ws.Channels.Abstractions
{
    public interface IChannelListener<out TInMessage> : IDisposable
    {
        IObservable<TInMessage> ChannelFeed { get; }
    }
}
