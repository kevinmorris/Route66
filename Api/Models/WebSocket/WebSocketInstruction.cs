namespace Api.Models.WebSocket
{
    public enum WebSocketInstruction
    {
        OK,
        CONNECT,
        STARTING_CONNECTION,
        DISPLAY,
        SUBMIT_FIELDS,
        ERROR
    }
}
