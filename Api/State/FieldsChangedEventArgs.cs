using Services.Models;

namespace Api.State
{
    public record FieldsChangedEventArgs(IEnumerable<FieldData>[] FieldData)
    {
    }
}
