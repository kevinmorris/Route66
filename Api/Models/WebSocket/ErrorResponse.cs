using Newtonsoft.Json;

namespace Api.Models.WebSocket
{
    public record ErrorResponse([JsonProperty("message")] string Message) : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.ERROR;
    }
}
