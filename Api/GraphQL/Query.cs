using Api.Models.GraphQL;
using Services.Models;

namespace Api.GraphQL
{
    public class Query
    {
        public Display GetDisplay()
        {
            return new Display(Enumerable.Repeat(Array.Empty<FieldData>(), 24).ToArray());
        }
    }
}
