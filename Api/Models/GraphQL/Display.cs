using Services.Models;

namespace Api.Models.GraphQL
{
    public record Display(IEnumerable<IEnumerable<FieldData>> FieldData)
    {
    }
}
