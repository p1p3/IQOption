using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using IQOptionClient.Time;
using IQOptionClient.Ws.Client;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOptionClient.Ws.Channels
{
    public abstract class IQOptionChannel<TInMessage, TOutMessage> : IChannel<TInMessage, TOutMessage>
    {
        protected readonly IWsIQClient WsIqClient;

        protected IQOptionChannel(IWsIQClient wsIqClient)
        {
            WsIqClient = wsIqClient;

            ChannelMessagesFeed = WsIqClient.MessagesFeed
                .Where(CanProcessIncommingMessage);
        }

        protected IObservable<IQOptionMessage> ChannelMessagesFeed;

        public abstract IObservable<TInMessage> ChannelFeed { get; }
        public abstract string ChannelName { get; }

        public virtual void Dispose() { }

        public virtual bool CanProcessIncommingMessage(IQOptionMessage message)
        {
            var canProcess = message.ChannelName.Equals(ChannelName, StringComparison.OrdinalIgnoreCase);
            return canProcess;
        }

        public abstract IObservable<IQOptionMessage> SendMessage(TOutMessage message);
    }
}
