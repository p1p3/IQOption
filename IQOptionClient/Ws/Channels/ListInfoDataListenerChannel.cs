using System;
using System.Collections.Generic;
using IQOptionClient.Ws.Channels.Bases;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels
{
    public class ListInfoDataListenerChannel : IqOptionGenericChannelListener<IList<InfoData>>
    {
        //TODO RETURN EACH INFO DATA AND NOT A LIST
        private const string ChannelName = "listInfoData";

        public ListInfoDataListenerChannel(IWsIQClient wsIqClient) : base(wsIqClient, ChannelName)
        {

        }
    }
}
