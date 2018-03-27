using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using System.Reactive.Threading.Tasks;
using System.Text;
using IQOptionClient.Ws.Channels.Bases;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOptionClient.Ws.Channels
{
    public class HeartBeatDualChannel : IqOptionGenericDualChannel<HeartBeatInputMessage, HeartBeatOutputMessage>
    {
        private const string DualChannelName = "heartbeat";

        public HeartBeatDualChannel(IWsIQClient wsIqClient)
            : base(new IqOptionGenericChannelListener<HeartBeatInputMessage>(wsIqClient, DualChannelName,
                    message => new HeartBeatInputMessage(long.Parse(message.Message.ToString()))),
                new IqOptionGenericChannelPublisher<HeartBeatOutputMessage>(wsIqClient, DualChannelName))
        {
        }

    }
}
