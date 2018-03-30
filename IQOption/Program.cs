using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using System.Threading;
using System.Threading.Tasks;
using IQOptionClient.Http;
using IQOptionClient.Http.Resources.V1;
using IQOptionClient.Http.ResthSharpHelpers;
using IQOptionClient.Utilities;
using IQOptionClient.Ws;
using IQOptionClient.Ws.Models;
using Newtonsoft.Json;

namespace IQOption
{
    class Program
    {

        //TODO THINGS :
        // RECHARGE ACCOUNT
        // LOAD PREVIOUS DATA, FIRST CANDLE I THINK
        // SECURE CONNECTION TO RETURN

        static void Main(string[] args)
        {
            var httpConfiguration = new HttpConfiguration();
            var restClient = new ReliableRestClientRestSharPolly(httpConfiguration);

            var loginOperation = new LoginRestSharp(restClient);

            const string username = "wechallp1p3@gmail.com";
            const string password = "t3st!ng";

            var response = loginOperation.Login(username, password).GetAwaiter().GetResult();


            using (var client = new WsIQClientRx())
            {
                //TODO :This client should be different when you connect returns the wsConnection ( connection should be the variable name)
                client.Connect()
                    .Subscribe(iqConnection =>
                    {
                        // TODO THis is not secure for example as well heartbeat
                        iqConnection.ServerDatetime.Subscribe(serverTime =>
                            PrintMessage(serverTime.ToString(CultureInfo.InvariantCulture), ConsoleColor.Green));

                        //when loggged in, make the bid
                        iqConnection.Profile.Subscribe(profile =>
                        {
                            var active = Active.EURUSD;
                            var candleSizeInSeconds = 5;
                            var periodOfMa = TimeSpan.FromMinutes(60);
                            var candleSize = TimeSpan.FromSeconds(candleSizeInSeconds);

                            var currentCandle = iqConnection
                                .CreateCandles(active, candleSize);

                            var candlesEachTime = currentCandle
                                .Throttle(candleSize)
                                .Print("Candle each 5");


                            var movingAverage5Seconds = candlesEachTime
                                .Sum(candle => candle.Bid)
                                .Map(sum => sum / candleSizeInSeconds)
                                .Print("ma")
                                .Sample(periodOfMa)
                                .Print("After period")
                                .Subscribe(mas =>
                                {
                                    //restar
                                });

                            // make a bid every minute
                            iqConnection.ServerDatetime.Where(serverTime => serverTime.Second == 20)
                                .Subscribe(serverTime =>
                                {
                                    var praticeBalance = profile.Balances.First(balance => balance.Type == (long)BalanceType.Practice);

                                    iqConnection.PlaceBid(5.00, Active.EURUSD, Direction.Call, praticeBalance, candleSize)
                                                .Subscribe(bid =>
                                                PrintMessage($"Bid placed {JsonConvert.SerializeObject(bid)}",
                                                   ConsoleColor.Blue));
                                });
                        });

                        iqConnection.Bidsresults
                            .Filter(result => result.RateFinished)
                            .Subscribe(result =>
                        {
                            PrintMessage($"Bid Result {JsonConvert.SerializeObject(result)}",
                                result.Win == "loose" ? ConsoleColor.Red : ConsoleColor.Green);
                        });


                        //subscribe to canndles
                        //iqConnection.Profile.Subscribe(profile =>
                        //{
                        //    iqConnection.CreateCandles(Active.EURUSD, candleSize)
                        //        .Sample(TimeSpan.FromSeconds(1))
                        //        .Subscribe(candle =>
                        //        {
                        //            PrintMessage(JsonConvert.SerializeObject(candle), ConsoleColor.White);
                        //        });
                        //});




                        //TODO Return a different "secured connection" only things you can do when loged in.
                        iqConnection.Login(ssid: response.Ssid)
                            .Subscribe();


                    });
                Console.ReadKey();
            }

            Console.WriteLine("Not listening");
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
