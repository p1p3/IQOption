using System;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels.Abstractions
{
    public interface IChannelPublisher<in TOutMessage> : IDisposable
    {

        IObservable<IQOptionMessage> SendMessage(TOutMessage message);
    }
}