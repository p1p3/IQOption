using System;

namespace IQOptionClient.Utilities
{
    public class Epoch : IEpoch
    {
        public DateTime EpochTime => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public long EpochSeconds => SecondsUnixTimeFromDateTime(DateTime.UtcNow);
        public long EpochMilliSeconds => MilliSecondsUnixTimeFromDateTime(DateTime.UtcNow);
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

        public long SecondsUnixTimeFromDateTime(DateTime dateTime)
        {
            var epochTime = dateTime-  EpochTime;
            return (long)epochTime.TotalSeconds;
        }

        public long MilliSecondsUnixTimeFromDateTime(DateTime dateTime)
        {
            var epochTime = dateTime - EpochTime;
            return (long)epochTime.TotalMilliseconds;
        }
    }
}