using Api.Models.GraphQL;
using Api.State;
using HotChocolate.Subscriptions;
using Services.Models;
using System.Net.WebSockets;

namespace Api.GraphQL
{
    public class Mutation(TerminalStatePool pool)
    {
        public Connection Connect(ConnectRequest connectRequest, [Service] ITopicEventSender eventSender)
        {
            var sessionKey = Guid.NewGuid().ToString();
            pool.Start(sessionKey, connectRequest.Address, connectRequest.Port, async (sender, args) =>
            {
                var display = new Display(args.FieldData);
                await eventSender.SendAsync("DisplayUpdated", display);
            });

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
