using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using IQOptionClient.Ws.Models;

namespace IQOptionClient.Utilities
{
    public static class ObservableMovingAverageExtension
    {
        public static IObservable<decimal> SimpleMovingAverage(this IObservable<Candle> candles,TimeSpan candleSize, int period)
        {
            return candles
                 .Sample(candleSize)
                 .Buffer(period, 1)
                 .Map(buffer => buffer.Average(candle => candle.Close));
        }

        [Obsolete]
        public static IObservable<decimal> ExperimentalSimpleMovingAverage(this IObservable<Candle> candles, TimeSpan candleSize, int period)
        {
            return candles
                .Sample(candleSize)
                .Scan(new List<decimal>(),
                    (buffer, candle) =>
                    {
                        buffer.Add(candle.Close);
                        if (buffer.Count > period)
                        {
                            buffer.RemoveAt(0);
                        }

                        return buffer;
                    })
                .Select(buffer => buffer.Average());
        }
    }
}
