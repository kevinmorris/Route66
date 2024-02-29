using Services.Models;
using Services;
using Util;

namespace Api.Models
{
    public class TerminalState
    {
        private readonly IEnumerable<FieldData>[] _fieldData;

        public TerminalState(TN3270Service<IEnumerable<FieldData>> tn3270Service, string address, int port)
        {
            _fieldData = new IEnumerable<FieldData>[Constants.SCREEN_HEIGHT];

            for (var i = 0; i < tn3270Service.Handlers.Length; i++)
            {
                tn3270Service.Handlers[i].RowUpdated += RowUpdatedFunc(i);
            }

            tn3270Service.Connect(address, port);
        }

        private EventHandler<RowUpdateEventArgs<IEnumerable<FieldData>>> RowUpdatedFunc(int row)
        {
            return (sender, args) =>
            {
                _fieldData[row] = args.Data;
                foreach (var field in args.Data)
                {
                    Console.WriteLine($"XXXXXA128 {row}: {field}");
                }
            };
        }
    }
}
