using Api.Models.GraphQL;
using Api.State;
using Services.Models;

namespace Api.GraphQL
{
    public class Query(TerminalStatePool pool)
    {
        public Display GetDisplay(string sessionKey)
        {
            var terminalState = pool[sessionKey];
            return terminalState != null
                ? new Display(terminalState.FieldData)
                : throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"{sessionKey} not found")
                    .Build());
        }
    }
}
