using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;

namespace IQOptionClient.Utilities
{
    public static class ObservableMathExtensions
    {
        public static IObservable<decimal> Slope(this IObservable<Point> points)
        {
            return points
                 .Buffer(2, 1)
                 .Map(point =>
                 {
                     var oldValue = point.First();
                     var newestValue = point.Last();

                     var changeInYAxis = newestValue.Y - oldValue.Y;
                     var changeInXAxis = newestValue.X - oldValue.X;

                     var slope = (changeInYAxis) / (changeInXAxis);

                     return slope;
                 });
        }

        public static IObservable<decimal> Scale(this IObservable<decimal> values, long scale)
        {
            return values
                .Map(value => value * scale);
        }

        public static IObservable<double> RadiansToDeg(this IObservable<double> radians)
        {
            return radians.Map(radian =>
            {
                var degrees = radian * (180 / Math.PI);

                return degrees;
            });
        }

        public static IObservable<double> SlopeDegree(this IObservable<decimal> slopes)
        {
            return slopes.Map(slope => Math.Tanh((double)slope));
        }


    }

    public struct Point
    {
        public Point(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public decimal X { get; }
        public decimal Y { get; }
    }
}
