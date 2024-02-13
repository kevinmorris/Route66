using System.Text.Json.Serialization;
using Services.Models;

namespace Route66Blazor.Models
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public record UserData(int CursorRow, int CursorCol, IEnumerable<FieldData> Fields)
    {
    }
}
