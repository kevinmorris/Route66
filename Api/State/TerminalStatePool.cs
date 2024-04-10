using System.Collections;
using Api.State;
using Services;
using Services.Models;
using System.Collections.Generic;

namespace Api.State
{
    public class TerminalStatePool(IServiceProvider serviceProvider)
    {
        private readonly IDictionary<string, TerminalState> _pool = new Dictionary<string, TerminalState>();

        public TerminalState? this[string key] => _pool.TryGetValue(key, out var item) ? item : null;

        public void Start(string key, string address, int port, EventHandler<FieldsChangedEventArgs>? customHandler)
        {
            var terminalState = new TerminalState(
                serviceProvider.GetService<TN3270Service<IEnumerable<FieldData>>>(),
                address,
                port,
                customHandler);

            _pool[key] = terminalState;
        }
    }
}
