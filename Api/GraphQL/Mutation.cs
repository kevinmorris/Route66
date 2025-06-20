using Api.Models.GraphQL;
using Services.Models;

namespace Api.GraphQL
{
    public class Mutation
    {
        public async Task<Connection> Connect(ConnectParams connectParams)
        {
            return await Task.FromResult(new Connection(Guid.NewGuid().ToString(), "127.0.0.1", 3271));
        }

        public async Task<Display> SubmitFields(Submission submission)
        {
            return await Task.FromResult(new Display(Enumerable.Repeat(Array.Empty<FieldData>(), 24).ToArray()));
        }
    }
}
