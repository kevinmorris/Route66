using Api.Models;
using Services;

namespace Api.Services
{
    public class TerminalStatePool(string address, int port)
    {
        private readonly IDictionary<string, TerminalState> _pool = new Dictionary<string, TerminalState>();

        public TerminalState? this[string key] => _pool.TryGetValue(key, out var item) ? item : null;

        public void Start(string key)
        {
            var terminalState = new TerminalState(address, port);
            _pool[key] = terminalState;
        }
    }
}
