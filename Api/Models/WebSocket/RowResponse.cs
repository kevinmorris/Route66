using Services.Models;

namespace Api.Models.WebSocket
{
    public record RowResponse(int Row, IEnumerable<FieldData> FieldData) : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.ROW;
    }
}
