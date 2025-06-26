using Services.Models;

namespace Api.Models.GraphQL
{
    public class FieldSubmission
    {
        public Constants.AidKeyValue Aid { get; init; }

        public int CursorRow { get; init; }

        public int CursorCol { get; init; }

        public IEnumerable<FieldInput> FieldData { get; init; } = new List<FieldInput>();
    }
}
