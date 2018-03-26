using System;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels
{
    public abstract class IQOptionChannel<TInMessage, TOutMessage> : IChannel<TInMessage, TOutMessage>
    {
        public abstract IObservable<TInMessage> ChannelFeed { get; }
        public abstract string ChannelName { get; }

        public virtual void Dispose(){}

        public virtual bool CanProcessIncommingMessage(IQOptionMessage message)
        {
            var canProcess = message.ChannelName.Equals(ChannelName, StringComparison.OrdinalIgnoreCase);
            return canProcess;
        }

        public abstract IObservable<IQOptionMessage> SendMessage(TOutMessage message);
    }
}
