using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using System.Threading;
using Newtonsoft.Json;

namespace IQOptionClient.Utilities
{
    public static class ObservableHelperExtensions
    {
        public static IObservable<TType> FlatMapToSelf<TType>(this IObservable<IObservable<TType>> observable)
        {
            return observable.FlatMap<IObservable<TType>, TType>(i => i);
        }

        public static IObservable<TSource> Print<TSource>(this IObservable<TSource> source, string eventName = "")
        {
            return source.Do(message =>
             {
                 var serializedMessage = JsonConvert.SerializeObject(message);
                 var thread = Thread.CurrentThread.ManagedThreadId;
                 var messageToPrint = $"New event {eventName} {serializedMessage} on thread {thread}";

                 Console.WriteLine(messageToPrint);
             });
        }
    }
}
