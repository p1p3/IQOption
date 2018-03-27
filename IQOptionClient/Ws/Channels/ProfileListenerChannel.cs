using System;
using System.Collections.Generic;
using System.Text;
using IQOptionClient.Ws.Channels.Bases;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Ws.Channels
{
    public class ProfileListenerChannel : IqOptionGenericChannelListener<Profile>
    {
        private const string ChannelName = "profile";
        public ProfileListenerChannel(IWsIQClient wsIqClient) : base(wsIqClient, ChannelName)
        {
        }
    }
}
