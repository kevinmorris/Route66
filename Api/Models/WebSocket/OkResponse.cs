namespace Api.Models.WebSocket
{
    public record OkResponse : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.OK;
    }
}
