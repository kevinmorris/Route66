using Newtonsoft.Json;
using Services.Models;

namespace Api.Models.WebSocket
{
    public record DisplayResponse([JsonProperty("fieldData")] IEnumerable<FieldData>[] FieldData) : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.DISPLAY;
    }
}
