namespace IQOptionClient.Ws.Models
{
    public class TimeSync
    {
        public TimeSync(long serverTimeStamp)
        {
            ServerTimeStamp = serverTimeStamp;
        }

        public long ServerTimeStamp { get; }
    }
}
