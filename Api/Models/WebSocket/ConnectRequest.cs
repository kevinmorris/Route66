using Newtonsoft.Json;

namespace Api.Models.WebSocket
{
    public record ConnectRequest(string Address, int Port) : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.CONNECT;
    }
}
