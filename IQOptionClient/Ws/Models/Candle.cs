using System;
using IQOptionClient.Time;
using Newtonsoft.Json;

namespace IQOptionClient.Ws.Models
{
    public class Candle
    {
        [JsonProperty("open")]
        public decimal Open { get; set; }

        [JsonProperty("close")]
        public decimal Close { get; set; }

        [JsonProperty("max")]
        public decimal Max { get; set; }

        [JsonProperty("min")]
        public decimal Min { get; set; }

        [JsonProperty("ask")]
        public decimal Ask { get; set; }

        [JsonProperty("bid")]
        public decimal Bid { get; set; }

        [JsonProperty("volume")]
        public decimal Volume { get; set; }

        [JsonProperty("size")]
        public int SizeInSeconds { get; set; }

        [JsonProperty("at")]
        public long AtTimeStamp { get; set; }


        public DateTime AtUtcDateTime(IEpoch epoch)
        {
            return epoch.FromUnixTimeToDateTime(this.AtTimeStamp);
        }



    }
}