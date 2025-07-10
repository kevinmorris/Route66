using Api.Models.GraphQL;
using Services.Models;

namespace Api.GraphQL
{
    public class Subscription
    {
        [Subscribe]
        [Topic($"display_{{{nameof(sessionKey)}}}")]
        public Display Display(string sessionKey, [EventMessage] Display display)
        {
            return display;
        }
    }
}
