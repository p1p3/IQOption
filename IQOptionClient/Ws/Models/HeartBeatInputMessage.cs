namespace IQOptionClient.Ws.Models
{
    public class HeartBeatInputMessage
    {
        public HeartBeatInputMessage(long heartbeatTime)
        {
            HeartbeatTime = heartbeatTime;
        }

        public long HeartbeatTime { get; }
    }
}
