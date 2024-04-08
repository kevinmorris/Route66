using Services.Models;

namespace Api.Models.WebSocket
{
    public record DisplayResponse : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.DISPLAY;

        public required IEnumerable<FieldData>[] FieldData { get; init; }
    }
}
