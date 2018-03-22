using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IQOptionClient.Ws
{
    public interface IWsClient
    {
    }

    public class WsClient : IWsClient
    {

        public async Task asd(string ssid)
        {
            var cookies = new CookieContainer();

            cookies.Add(new Uri("https://iqoption.com"),
                           new Cookie("platform", "9"));

            cookies.Add(new Uri("https://iqoption.com"),
                new Cookie("platform_version", "1009.13.5397.release"));


            Uri serverUri = new Uri("wss://iqoption.com/echo/websocket");
            using (ClientWebSocket ws = new ClientWebSocket()
            {
                Options = { Cookies = cookies }
            })
            {
                await ws.ConnectAsync(serverUri, CancellationToken.None);
                //Send
                var sendTask = SendSsid(ssid, ws);
                //Recieve 
                var recieveTask = Listen(ws);

                await Task.WhenAll(recieveTask, sendTask);
            }

        }


        private async Task Listen(ClientWebSocket ws)
        {
            var buffer = new byte[1024];

            while (ws.State == WebSocketState.Open)
            {
                var stringResult = new StringBuilder();


                WebSocketReceiveResult result;
                do
                {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await
                            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    else
                    {
                        var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        stringResult.Append(str);
                    }

                } while (!result.EndOfMessage);

                CallOnMessage(stringResult, ws);

            }
        }

        private async Task SendMessageAsync(string message, ClientWebSocket ws)
        {
            if (ws.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var SendChunkSize = 1024;
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (SendChunkSize * i);
                var count = SendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                await ws.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, CancellationToken.None);
            }
        }

        private async void CallOnMessage(StringBuilder stringResult, ClientWebSocket ws)
        {
            var resultString = stringResult.ToString();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Recieve Message : {resultString}");
            Console.ForegroundColor = ConsoleColor.White;

            if (resultString.Contains("profile"))
            {
                Console.WriteLine("oohooo!");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }

            if (resultString.Contains("heartbeat"))
            {
                var deserializedResult = JsonConvert.DeserializeObject<dynamic>(resultString);

                var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                var secondsSinceEpoch = (long)t.TotalSeconds;
                var epochSeconds = secondsSinceEpoch.ToString();

                var miliSecondsSinceEpoch = (long)t.TotalMilliseconds;
                var epochMilis = miliSecondsSinceEpoch.ToString();


                var heartbeatResponse = new
                {
                    name = "heartbeat",
                    request_id = epochSeconds + "_635264212",
                    msg = new { userTime = epochMilis, heartbeatTime = deserializedResult.msg.ToString() }
                };

                var serializedResponse = JsonConvert.SerializeObject(heartbeatResponse);

                await Sendtext(serializedResponse, ws);
            }
        }

        public Task SendSsid(string ssid, ClientWebSocket ws)
        {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var secondsSinceEpoch = (long)t.TotalSeconds;
            var epochSeconds = secondsSinceEpoch.ToString();
            var message = new { name = "ssid", msg = ssid, request_id = epochSeconds + "_1772932922", };

            var serializedMessage = JsonConvert.SerializeObject(message);
            return Sendtext(serializedMessage, ws);
        }

        public Task Sendtext(string text, ClientWebSocket ws)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Send Message : {text}");
            Console.ForegroundColor = ConsoleColor.White;

            return SendMessageAsync(text, ws);
        }
    }

    //public class WsClientWebSocketSharp : IWsClient
    //{
    //    public async Task asd(string ssid)
    //    {




    //        //cookies.Add(new Uri("https://iqoption.com"),
    //        //    new Cookie("platform", "9"));

    //        using (var ws = new WebSocketSharp.WebSocket("wss://iqoption.com/echo/websocket"))
    //        {


    //            ws.Origin = "https://iqoption.com";
    //            var platformCookie = new WebSocketSharp.Net.Cookie("platform", "9");
    //            ws.SetCookie(platformCookie);

    //            //var platformVersionCookie = new WebSocketSharp.Net.Cookie("platform_version", "1009.13.5397.release");
    //            //ws.SetCookie(platformVersionCookie);

    //            //cookies.Add(new Uri("https://iqoption.com"),
    //            //    new Cookie("ssid", ssid));




    //            ws.OnMessage += (sender, e) =>
    //            {
    //                Console.WriteLine("Laputa says: " + e.Data);
    //            };

    //            ws.OnOpen += (sender, e) =>
    //            {
    //                Console.WriteLine("Laputa says: " + e);
    //            };

    //            ws.OnError += (sender, e) =>
    //            {
    //                Console.WriteLine("Laputa says: " + e.Message);
    //            };

    //            //var serializedMessage = "[\"{\"name\":\"ssid\",\"msg\":\"" +
    //            //    ssid + "\"}\"]";

    //            //var serializedMessage =
    //            //   "{\"msg\": \"f3589abe3841d2f71cd36f6b1a395310\", \"name\": \"ssid\"}";


    //            var message = new { name = "ssid", msg = ssid };
    //            var serializedMessage = JsonConvert.SerializeObject(message);


    //            //var bytesToSend = Encoding.ASCII.GetBytes(serializedMessage);

    //            ws.Connect();

    //            while (ws.ReadyState == WebSocketSharp.WebSocketState.Connecting)
    //            {
    //                Thread.Sleep(1000);
    //            }


    //            ws.Send(serializedMessage);

    //            //Thread.Sleep(Timeout.Infinite);
    //        }




    //    }

    //    public async Task Recieve(ClientWebSocket ws)
    //    {
    //        //Recieve
    //        var times = 0;
    //        while (ws.State == WebSocketState.Open)
    //        {
    //            var bytesReceived = new ArraySegment<byte>(new byte[1024]);
    //            var result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);

    //            var resultString = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

    //            times++;
    //            if (times > 10)
    //            {
    //                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    //            }

    //            Thread.Sleep(1000);

    //        }
    //    }

    //    public Task SendSsid(string ssid, ClientWebSocket ws)
    //    {
    //        //var serializedMessage =
    //        //    "{\"msg\": \"f3589abe3841d2f71cd36f6b1a395310\", \"name\": \"ssid\"}";
    //        var jsonObject = new JObject();
    //        jsonObject.Add("msg", ssid);
    //        jsonObject.Add("name", ssid);

    //        //jsonObject.Name = "mkyong.com";
    //        //jsonObject.Age = 100;
    //        var json = jsonObject.ToString();
    //        var message = new List<dynamic>() { new { name = "ssid", msg = ssid } };


    //        var serializedMessage = JsonConvert.SerializeObject(message);

    //        var bytesToSend = Encoding.ASCII.GetBytes(json);
    //        return ws.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Binary, false, CancellationToken.None);
    //    }
    //}



    public class IqOptionWsMessage
    {
        public string name { get; }
        public string msg { get; }
    }
}
