using EnsureThat;
using MediatR;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using WebSockets.Handlers;

namespace WebSockets
{
    public interface IServer
    {
        Task Accept(Guid id, WebSocket socket);

        Task Broadcast(string message);
    }

    public class Server : IServer
    {
        private static Dictionary<Guid, WebSocket> Sockets = new Dictionary<Guid, WebSocket>();

        private IMediator Mediator { get; }

        public Server(IMediator mediator)
        {
            Mediator = EnsureArg.IsNotNull(mediator);
        }

        public async Task Accept(Guid id, WebSocket socket)
        {
            Sockets.Add(id, socket);

            var buffer = new byte[1024 * 4];

            var received = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!received.CloseStatus.HasValue)
            {
                // Console.WriteLine($"{Encoding.ASCII.GetString(buffer)}");

                var schema = await Mediator.Send(new ValidateRequest { Schema = "Person", Payload = Encoding.ASCII.GetString(buffer) });

                await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new { schema = schema.Valid }))), WebSocketMessageType.Text, true, CancellationToken.None);

                // Console.WriteLine($"Schema: {schema.Valid}");

                received = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await socket.CloseAsync(received.CloseStatus.Value, received.CloseStatusDescription, CancellationToken.None);
        }

        public async Task Broadcast(string message)
        {
            foreach (var socket in Sockets.Values)
            {
                await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new { message }))), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}