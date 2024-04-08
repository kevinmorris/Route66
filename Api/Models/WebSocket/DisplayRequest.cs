using Newtonsoft.Json;

namespace Api.Models.WebSocket
{
    public record DisplayRequest : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.DISPLAY;

        public required string SessionKey { get; init; }
    }
}
