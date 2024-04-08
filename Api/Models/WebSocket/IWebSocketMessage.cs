using Newtonsoft.Json;

namespace Api.Models.WebSocket
{
    public interface IWebSocketMessage
    {
        [JsonProperty("instruction")]
        public WebSocketInstruction Instruction { get; }
    }
}
