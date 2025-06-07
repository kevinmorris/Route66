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
using Newtonsoft.Json.Serialization;
using Services.Models;

namespace Api.Controllers
{
    [ApiController]
    public class WebSocketController(TerminalStatePool pool) : ControllerBase
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        [Route("/ws")]
        public async Task Connect()
        {
            var webSocketManager = HttpContext.WebSockets;
            if (!webSocketManager.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using var webSocket = await webSocketManager.AcceptWebSocketAsync();
            var buffer = new byte[1024 * 4];
            var cancellationToken = CancellationToken.None;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult? result = null;
                    try
                    {
                        result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                    }
                    catch (WebSocketException wse)
                    {
                        Console.WriteLine($"WebSocket error: {wse.Message}");
                    }

                    if (result is { MessageType: WebSocketMessageType.Close })
                    {
                        Console.WriteLine("Client initiated close");
                        break;
                    }

                    try
                    {
                        var outboundMessage = await CreateMessage(buffer, webSocket);
                        if (outboundMessage is StartingConnectionResponse sc)
                        {
                            Start(sc, webSocket);
                        }

                        await SendMessage(webSocket, outboundMessage);

                    }
                    catch (JsonException jse)
                    {
                        var errorStr = $"JSON parse error: {jse.Message}";
                        Console.WriteLine(errorStr);
                        await SendMessage(webSocket, new ErrorResponse(errorStr));
                    }

                    Array.Clear(buffer);
                }
            }
            finally
            {
                if (webSocket.State != WebSocketState.Closed)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
                }
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
            var outJson = JsonConvert.SerializeObject(message, _jsonSerializerSettings);
            var output = Encoding.UTF8.GetBytes(outJson);

            await webSocket.SendAsync(
                new ArraySegment<byte>(output, 0, output.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
