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

        public static IObservable<TSource> Spy<TSource>(this IObservable<TSource> source, string eventName = "", ConsoleColor color = ConsoleColor.White)
        {
            return source.Do(message => { PrintMessage(message, eventName, color); });
        }

        public static IDisposable Print<TSource>(this IObservable<TSource> source, string eventName = "", ConsoleColor color = ConsoleColor.White)
        {
            return source.Subscribe(message => { PrintMessage(message, eventName, color); });
        }

        private static void PrintMessage<TSource>(TSource message, string eventName, ConsoleColor color)
        {

            var serializedMessage = JsonConvert.SerializeObject(message);
            var thread = Thread.CurrentThread.ManagedThreadId;
            var messageToPrint = $"New event {eventName} {serializedMessage} on thread {thread}";

            Console.ForegroundColor = color;
            Console.WriteLine(messageToPrint);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
