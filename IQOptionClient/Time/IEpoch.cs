using System;

namespace IQOptionClient.Time
{
    public interface IEpoch
    {
        DateTime EpochTime { get; }
        TimeSpan EpochTimeSpan { get; }
        long EpochSeconds { get; }
        long EpochMilliSeconds { get; }
        DateTime FromUnixTimeToDateTime(long unixTime);
    }
}
