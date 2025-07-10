using System.Text.Json.Serialization;
using Services.Models;

namespace Api.Models
{
    public class FieldSubmission()
    {

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Constants.AidKeyValue Aid { get; init; }

        public int CursorRow { get; init; }

        public int CursorCol { get; init; }

        private readonly IEnumerable<FieldData> _fieldData = [];

        public IEnumerable<FieldData> FieldData
        {
            get => _fieldData;
            init => _fieldData = value ?? [];
        }
    }
}
