using System;
using IQOptionClient.Http;
using IQOptionClient.Http.Resources.V1;
using IQOptionClient.Http.ResthSharpHelpers;
using IQOptionClient.Ws;

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

            //Act
            var response = loginOperation.Login(username, password).GetAwaiter().GetResult();

            using (var client = new WsClient())
            {
                client.Connect(response.Ssid).GetAwaiter().GetResult();
                Console.ReadKey();
            }
 
        }

        public class HttpConfiguration : IHttpConfiguration
        {
            public Uri BaseUrl => new Uri("https://auth.iqoption.com/api/");
        }
    }
}
