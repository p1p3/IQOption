using System;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels.Abstractions
{
    public interface IChannelPublisher<in TOutMessage, out TMessageSent> : IDisposable
    {

        IObservable<TMessageSent> SendMessage(TOutMessage message);
    }
}