using System;

namespace IQOptionClient.Ws.Exceptions
{
    public class TimeWindowExpiredException : Exception
    {
        public TimeWindowExpiredException(DateTime time) : base($"Server bid time is closed {time}")
        {

        }
    }
}
