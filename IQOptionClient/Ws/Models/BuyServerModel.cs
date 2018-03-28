using IQOptionClient.Utilities;
using Newtonsoft.Json;

namespace IQOptionClient.Ws.Models
{
    public class BuyServerModel
    {
        public BuyServerModel(double price, Active active, Direction direction, long bidExpirationTime, long currentUserTime, string type)
        {
            Price = price;
            BidExpirationTime = bidExpirationTime;
            CurrentUserTime = currentUserTime;
            Active = (int)active;
            Direction = direction.GetDescription();
            Type = type;

        }

        [JsonProperty("price")]
        public double Price { get; }

        [JsonProperty("act")]
        public int Active { get; }

        [JsonProperty("direction")]
        public string Direction { get; }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("exp")]
        public long BidExpirationTime { get; }

        [JsonProperty("time")]
        public long CurrentUserTime { get; }
    }
}
