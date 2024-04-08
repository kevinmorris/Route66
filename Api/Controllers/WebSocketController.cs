using System.Text;
using Api.Models.WebSocket;
using Api.State;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Models;

namespace Api.Controllers
{
    [ApiController]
    public class WebSocketController(TerminalStatePool pool) : ControllerBase
    {
        [Route("/ws")]
        public async Task Connect()
        {
            var webSocketManager = HttpContext.WebSockets;
            if (webSocketManager.IsWebSocketRequest)
            {
                using var webSocket = await webSocketManager.AcceptWebSocketAsync();

                var buffer = new byte[1024 * 4];
                var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    var output = CreateMessage(buffer);
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(output, 0, output.Length),
                        result.MessageType,
                        result.EndOfMessage,
                        CancellationToken.None);

                    result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                }

                await webSocket.CloseAsync(
                    result.CloseStatus.Value,
                    result.CloseStatusDescription,
                    CancellationToken.None);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private byte[] CreateMessage(byte[] input)
        {
            var inJson = Encoding.UTF8.GetString(input);
            dynamic inboundMessage = JObject.Parse(inJson);
            var instruction = (WebSocketInstruction)int.Parse(inboundMessage.instruction.ToString());

            IWebSocketMessage outboundMessage;
            switch(instruction) {
                case WebSocketInstruction.CONNECT:
                    var connectRequest = (ConnectRequest)inboundMessage.ToObject<ConnectRequest>();
                    outboundMessage = new SetSessionKeyResponse
                    {
                        SessionKey = Start(connectRequest)
                    };
                    break;

                case WebSocketInstruction.DISPLAY:
                    var displayRequest = (DisplayRequest)inboundMessage.ToObject<DisplayRequest>();
                    outboundMessage = new DisplayResponse
                    {
                        FieldData = GetFieldData(displayRequest.SessionKey)
                    };
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var outJson = JsonConvert.SerializeObject(outboundMessage);
            return Encoding.UTF8.GetBytes(outJson);
        }

        private string Start(ConnectRequest connectRequest)
        {
            var sessionKey = Guid.NewGuid().ToString();
            pool.Start(sessionKey, connectRequest.Address, connectRequest.Port);

            return sessionKey;
        }

        private IEnumerable<FieldData>[] GetFieldData(string sessionKey)
        {
            var terminalState = pool[sessionKey];
            return terminalState == null
                ? throw new InvalidOperationException($"{sessionKey} is invalid")
                : terminalState.FieldData;
        }
    }
}
