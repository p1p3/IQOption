using System;

namespace IQOptionClient.Ws.Client
{
    public class WsRecievemessageEventArgs : EventArgs
    {
        public WsRecievemessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}