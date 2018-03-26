using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using System.Reactive.Threading.Tasks;
using IQOptionClient.Time;
using IQOptionClient.Ws.Client;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOptionClient.Ws.Channels
{
    public class CandleGeneratedChannel : IQOptionChannel<Candle, CandleSubscription>
    {
        public override string ChannelName => "candle-generated";

        public sealed override IObservable<Candle> ChannelFeed { get; }

        public CandleGeneratedChannel(IWsIQClient wsIqClient) : base(wsIqClient)
        {
            ChannelFeed = base.ChannelMessagesFeed
                .Map(serializedMessage =>
                {
                    Candle candle = JsonConvert.DeserializeObject<Candle>(serializedMessage.Message.ToString());
                    return candle;
                });
        }

        public override IObservable<IQOptionMessage> SendMessage(CandleSubscription message)
        {
            //TODO SUBSCRIBE IS VERY ESPECIFIC the mssage create object and method to subscribe to
            const string subscriptionChannelName = "subscribeMessage";
            var msg = new
            {
                name = ChannelName,
                @params = new

                {
                    routingFilters = new
                    {
                        active_id = (int)message.Active,
                        size = message.SizeInSeconds
                    }
                }
            };

            return base.WsIqClient.SendMessage(subscriptionChannelName, msg);
        }


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
