using System;
using System.Collections.Generic;
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
                        //iqConnection.ServerDatetime.Subscribe(serverTime =>
                        //    PrintMessage(serverTime.ToString(CultureInfo.InvariantCulture), ConsoleColor.Green));

                        //when loggged in, make the bid
                        iqConnection.Profile.Subscribe(profile =>
                        {
                            var active = Active.EURUSD;
                            var candleSizeInSeconds = 5;
                            var periodOfMa = 5;
                            var candleSize = TimeSpan.FromSeconds(candleSizeInSeconds);
                            var praticeBalance = profile.Balances.First(balance => balance.Type == (long)BalanceType.Practice);
                            var currentCandle = iqConnection
                                .CreateCandles(active, candleSize);

                            var angleTrigger = 45;

                            currentCandle
                                .SimpleMovingAverage(candleSize, periodOfMa)
                                .Timestamp()
                                .Map(ma => new Point(ma.Timestamp.ToUnixTimeMilliseconds(), ma.Value))
                                .Slope()
                                .Scale(1000000000) // Milliseconds : 1000 * Variation point : 1000000 
                                .SlopeDegree()
                                .RadiansToDeg()
                                .Spy("Angle", ConsoleColor.Yellow)
                                .CombineLatest(iqConnection.ServerDatetime,
                                    (angle, servertime) =>
                                    {
                                        var information = new
                                        {
                                            Direction = angle > 0 ? Direction.Call : Direction.Put,
                                            ShouldInvest = Math.Abs(angle) > angleTrigger && servertime.Second == 25,
                                            ServerTime = servertime
                                        };

                                        return information;
                                    })
                                .Filter(information => information.ShouldInvest)
                                .DistinctUntilChanged(information => information.ServerTime)
                                .Spy("information", ConsoleColor.Green)
                                .FlatMap(information => iqConnection
                                    .PlaceBid(1.00, active, information.Direction, praticeBalance, candleSize))
                                .Print("Bid placed", ConsoleColor.Gray);




                            // make a bid every minute
                            //iqConnection.ServerDatetime.Where(serverTime => serverTime.Second == 20)
                            //.Subscribe(serverTime =>
                            //{
                            //    var praticeBalance = profile.Balances.First(balance => balance.Type == (long)BalanceType.Practice);

                            //    iqConnection.PlaceBid(1.00, active, Direction.Call, praticeBalance, candleSize)
                            //                .Subscribe(bid =>
                            //                PrintMessage($"Bid placed {JsonConvert.SerializeObject(bid)}",
                            //                   ConsoleColor.Blue));
                            //});
                        });

                        var results = iqConnection.Bidsresults
                               .Filter(result => result.RateFinished);


                        var aggregateResults = results.Scan(new List<InfoData>(), (acumulator, newInfoDataValue) =>
                             {
                                 acumulator.Add(newInfoDataValue);
                                 return acumulator;
                             })
                              .Subscribe(result =>
                            {
                                var lost = result.Where(infoData => infoData.Win == "loose");
                                var wins = result.Where(infoData => infoData.Win == "win");
                                var total = result.Count();

                                PrintMessage($"Wins : {wins.Count()}", ConsoleColor.Green);
                                PrintMessage($"Lost {lost.Count()}", ConsoleColor.Red);
                                PrintMessage($"Total {total}", ConsoleColor.White);
                            });



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
