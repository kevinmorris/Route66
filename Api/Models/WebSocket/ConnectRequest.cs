using Newtonsoft.Json;

namespace Api.Models.WebSocket
{
    public record ConnectRequest : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.CONNECT;

        public required string Address { get; init; }
        public required int Port { get; init; }
    }
}
