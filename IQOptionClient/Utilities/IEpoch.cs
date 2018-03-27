using System;

namespace IQOptionClient.Utilities
{
    public interface IEpoch
    {
        DateTime EpochTime { get; }
        long EpochSeconds { get; }
        long EpochMilliSeconds { get; }
        DateTime FromUnixTimeToDateTime(long unixTime);
        long SecondsUnixTimeFromDateTime(DateTime dateTime);
        long MilliSecondsUnixTimeFromDateTime(DateTime dateTime);
    }
}
