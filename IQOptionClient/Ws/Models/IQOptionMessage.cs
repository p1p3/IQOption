using Newtonsoft.Json;

namespace IQOptionClient.Ws.Models
{
    public class IQOptionMessage
    {
        [JsonProperty("name")]
        public string ChannelName { get; set; }

        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("msg")]
        public dynamic Message { get; set; }
    }
}