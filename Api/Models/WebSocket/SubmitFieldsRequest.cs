using Api.State;

namespace Api.Models.WebSocket
{
    public record SubmitFieldsRequest(string SessionKey, FieldSubmission Submission) : IWebSocketMessage
    {
        public WebSocketInstruction Instruction => WebSocketInstruction.SUBMIT_FIELDS;
    }
}
