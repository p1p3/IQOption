using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using IQOptionClient.Http;
using IQOptionClient.Http.Resources.V1;
using IQOptionClient.Http.ResthSharpHelpers;
using IQOptionClient.Ws;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOption
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpConfiguration = new HttpConfiguration();
            var restClient = new ReliableRestClientRestSharPolly(httpConfiguration);

            var loginOperation = new LoginRestSharp(restClient);

            const string username = "wechallp1p3@gmail.com";
            const string password = "t3st!ng";

            var response = loginOperation.Login(username, password).GetAwaiter().GetResult();
            var candleSize = 1;
            using (var client = new WsIQClientRx())
            {
                client.ConnectAsync(response.Ssid).GetAwaiter().GetResult();

                client.CreateCandles(Active.EURUSD, candleSize)
                   .Sample(TimeSpan.FromSeconds(candleSize))
                   .Subscribe(candle =>
                   {
                       PrintMessage(JsonConvert.SerializeObject(candle), ConsoleColor.Blue);
                   });

                client.ServerDatetime.Subscribe(serverTime => PrintMessage(serverTime.ToString(CultureInfo.InvariantCulture), ConsoleColor.Green));
                Console.ReadKey();
            }

            Console.ReadKey();

        }

        public class HttpConfiguration : IHttpConfiguration
        {
            public Uri BaseUrl => new Uri("https://auth.iqoption.com/api/");
        }

        private static void PrintMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
