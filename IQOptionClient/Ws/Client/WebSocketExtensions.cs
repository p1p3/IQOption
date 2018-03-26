using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IQOptionClient.Ws.Client
{
    public static class WebSocketExtensions
    {
        public static async Task SendMessageAsync(this ClientWebSocket ws, string message, int sendChunkSize, CancellationToken cancellationToken)
        {
            if (ws.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / sendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (sendChunkSize * i);
                var count = sendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                await ws.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, cancellationToken);
            }
        }
        public static Task SendMessageAsync(this ClientWebSocket ws, string message, CancellationToken cancellationToken)
        {
            return SendMessageAsync(ws, message, 1024, cancellationToken);
        }

        public static Task SendMessageAsync(this ClientWebSocket ws, string message)
        {
            return SendMessageAsync(ws, message, 1024, CancellationToken.None);
        }


    }
}