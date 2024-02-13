using System.Text.Json.Serialization;
using Services.Models;

namespace Route66Blazor.Models
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public record InputData(int CursorRow, int CursorCol, IEnumerable<FieldData> Fields)
    {
    }
}
