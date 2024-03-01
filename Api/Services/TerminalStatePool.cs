using System.Collections;
using Api.Models;
using Services;
using Services.Models;
using System.Collections.Generic;

namespace Api.Services
{
    public class TerminalStatePool(IServiceProvider serviceProvider)
    {
        private readonly IDictionary<string, TerminalState> _pool = new Dictionary<string, TerminalState>();

        public TerminalState? this[string key] => _pool.TryGetValue(key, out var item) ? item : null;

        public void Start(string key)
        {
            var terminalState = new TerminalState(
                serviceProvider.GetService<TN3270Service<IEnumerable<FieldData>>>(),
                "127.0.0.1",
                3270);

            _pool[key] = terminalState;
        }
    }
}
