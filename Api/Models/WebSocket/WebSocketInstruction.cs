namespace Api.Models.WebSocket
{
    public enum WebSocketInstruction
    {
        OK,
        CONNECT,
        STARTING_CONNECTION,
        DISPLAY,
        ROW,
        SUBMIT_FIELDS,
        ERROR
    }
}
