using Newtonsoft.Json;

namespace Api.Models.WebSocket
{
    public record StartingConnectionResponse(
        [JsonProperty("sessionKey")] string SessionKey,
        [JsonProperty("address")] string Address,
        [JsonProperty("port")] int Port) : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.STARTING_CONNECTION;
    }
}
