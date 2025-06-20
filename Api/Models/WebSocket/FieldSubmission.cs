using System.Text.Json.Serialization;
using Services.Models;

namespace Api.Models.WebSocket
{
    public class FieldSubmission()
    {

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Constants.AidKeyValue Aid { get; init; }

        public int CursorRow { get; init; }

        public int CursorCol { get; init; }

        public IEnumerable<FieldData> FieldData { get; init; } = new List<FieldData>();
    }
}
