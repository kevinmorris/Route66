using Api.Models.GraphQL;
using Api.State;
using Services.Models;
using System.Net.WebSockets;

namespace Api.GraphQL
{
    public class Mutation(TerminalStatePool pool)
    {
        public Connection Connect(ConnectRequest connectRequest)
        {
            var sessionKey = Guid.NewGuid().ToString();
            pool.Start(sessionKey, connectRequest.Address, connectRequest.Port, null);

            return new Connection(sessionKey, connectRequest.Address, connectRequest.Port);
        }

        public async Task<OkResponse> SubmitFields(Submission submission)
        {
            var terminalState = pool[submission.SessionKey];
            if (terminalState == null)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"{submission.SessionKey} not found")
                    .Build());
            }

            await terminalState.SendFields(submission.FieldSubmission);
            return new OkResponse(0);
        }
    }
}
