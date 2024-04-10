using System.Net.WebSockets;
using System.Text;
using Api.Models.WebSocket;
using Api.State;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
                    var outboundMessage = await CreateMessage(buffer, webSocket);
                    if (outboundMessage is StartingConnectionResponse sc)
                    {
                        Start(sc, webSocket);
                    }
                    await SendMessage(webSocket, outboundMessage);

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

        private async Task<IWebSocketMessage> CreateMessage(byte[] input, WebSocket webSocket)
        {
            var inJson = Encoding.UTF8.GetString(input);
            dynamic inboundMessage = JObject.Parse(inJson);
            var instruction = (WebSocketInstruction)int.Parse(inboundMessage.instruction.ToString());

            IWebSocketMessage outboundMessage;
            switch(instruction) {
                case WebSocketInstruction.CONNECT:
                    var connectRequest = (ConnectRequest)inboundMessage.ToObject<ConnectRequest>();
                    outboundMessage = new StartingConnectionResponse(
                        Guid.NewGuid().ToString(),
                        connectRequest.Address,
                        connectRequest.Port);
                    break;

                case WebSocketInstruction.DISPLAY:
                    var displayRequest = (DisplayRequest)inboundMessage.ToObject<DisplayRequest>();
                    outboundMessage = GetFieldData(displayRequest.SessionKey);
                    break;

                case WebSocketInstruction.SUBMIT_FIELDS:
                    var submitRequest = (SubmitFieldsRequest)inboundMessage.ToObject<SubmitFieldsRequest>();
                    outboundMessage = await SubmitFields(submitRequest);
                    break;

                case WebSocketInstruction.OK:
                    outboundMessage = new OkResponse();
                    break;

                default:
                    outboundMessage = new ErrorResponse("Invalid web socket instruction");
                    break;
            }

            return outboundMessage;
        }

        private void Start(StartingConnectionResponse connect, WebSocket webSocket)
        {
            pool.Start(connect.SessionKey, connect.Address, connect.Port, async (sender, args) =>
            {
                var outboundMessage = new RowResponse(args.Row, args.FieldData);
                await SendMessage(webSocket, outboundMessage);
            });
        }

        private IWebSocketMessage GetFieldData(string sessionKey)
        {
            var terminalState = pool[sessionKey];
            return terminalState != null
                ? new DisplayResponse(terminalState.FieldData)
                : new ErrorResponse($"{sessionKey} is invalid");
        }

        private async Task<IWebSocketMessage> SubmitFields(SubmitFieldsRequest request)
        {
            var terminalState = pool[request.SessionKey];
            if (terminalState == null)
            {
                return new ErrorResponse($"{request.SessionKey} is invalid");
            }

            await terminalState.SendFields(request.Submission);
            return new OkResponse();
        }

        private async Task SendMessage(WebSocket webSocket, IWebSocketMessage message)
        {
            var outJson = JsonConvert.SerializeObject(message);
            var output = Encoding.UTF8.GetBytes(outJson);

            await webSocket.SendAsync(
                new ArraySegment<byte>(output, 0, output.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
