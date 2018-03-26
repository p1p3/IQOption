using Newtonsoft.Json;

namespace IQOptionClient.Ws.Models
{
    public class HeartBeatOutputMessage
    {
        public HeartBeatOutputMessage(long userTime, long heartbeatTime)
        {
            UserTime = userTime;
            HeartbeatTime = heartbeatTime;
        }

        [JsonProperty("userTime")]
        public long UserTime { get; }

        [JsonProperty("heartbeatTime")]
        public long HeartbeatTime { get; }
    }
}
