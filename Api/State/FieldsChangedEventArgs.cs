using Services.Models;

namespace Api.State
{
    public record FieldsChangedEventArgs(int Row, IEnumerable<FieldData> FieldData)
    {
    }
}
