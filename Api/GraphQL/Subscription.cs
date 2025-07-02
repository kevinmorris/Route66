using Api.Models.GraphQL;

namespace Api.GraphQL
{
    public class Subscription
    {
        [Subscribe]
        public Display DisplayUpdated([EventMessage] Display display)
        {
            return display;
        }
    }
}
