using Api.Models.GraphQL;
using Services.Models;

namespace Api.GraphQL
{
    public class Subscription
    {
        [Subscribe]
        [Topic($"displayUpdated_{{{nameof(sessionKey)}}}")]
        public Display DisplayUpdated(string sessionKey, [EventMessage] Display display)
        {
            return display;
        }
    }
}
