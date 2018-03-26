using System;
using System.Collections.Generic;
using System.Reactive.Observable.Aliases;
using System.Text;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOptionClient.Ws.Channels
{
    public class SsidChannel : IQOptionChannel<string, string>
    {
        public override string ChannelName => "ssid";

        public override IObservable<string> ChannelFeed { get; }

        public SsidChannel(IWsIQClient wsIqClient) : base(wsIqClient)
        {
            ChannelFeed = base.ChannelMessagesFeed
                .Map(serializedMessage =>
                {
                    string message =  serializedMessage.Message.ToString();
                    return message;
                });
        }



        public override IObservable<IQOptionMessage> SendMessage(string message)
        {
            return base.WsIqClient.SendMessage(ChannelName, message);
        }
    }
}
