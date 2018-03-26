using System;

namespace IQOptionClient.Ws.Client
{
    public class WsErrorEventArgs : EventArgs
    {
        public WsErrorEventArgs(Exception exception, string error)
        {
            Exception = exception;
            Error = error;
        }

        public Exception Exception { get; set; }
        public string Error { get; set; }
    }
}