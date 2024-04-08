using Newtonsoft.Json;

namespace Api.Models.WebSocket
{
    public record SetSessionKeyResponse : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.SET_SESSION_KEY;

        [JsonProperty("sessionKey")] public required string SessionKey { get; init; }
    }
}
