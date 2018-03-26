using System;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels
{
    public interface IChannel<out TInMessage, in TOutMessage> : IDisposable
    {
        IObservable<TInMessage> ChannelFeed { get; }

        string ChannelName { get; }

        bool CanProcessIncommingMessage(IQOptionMessage message);

        IObservable<IQOptionMessage> SendMessage(TOutMessage message);

    }
}
