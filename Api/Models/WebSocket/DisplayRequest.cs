using Newtonsoft.Json;

namespace Api.Models.WebSocket
{
    public record DisplayRequest(string SessionKey) : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.DISPLAY;
    }
}
