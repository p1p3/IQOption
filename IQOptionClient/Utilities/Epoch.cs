using System;

namespace IQOptionClient.Time
{
    public class Epoch : IEpoch
    {
        public DateTime EpochTime => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public TimeSpan EpochTimeSpan => DateTime.UtcNow - EpochTime;
        public long EpochSeconds => (long)(EpochTimeSpan).TotalSeconds;
        public long EpochMilliSeconds => (long)(EpochTimeSpan).TotalMilliseconds;
        public DateTime FromUnixTimeToDateTime(long unixTime)
        {
            const int expectedDigits = 13;
            var digits = Math.Floor(Math.Log10(unixTime) + 1);

            var difference = (int)(expectedDigits - digits);
            var factor = long.Parse("1".PadRight(Math.Abs(difference) + 1, '0'));

            if (difference < 0)
            {
                unixTime = unixTime / factor;
            }
            else if (difference > 0)
            {
                unixTime = unixTime * factor;
                ;
            }

            return EpochTime.AddMilliseconds(unixTime);
        }
    }
}