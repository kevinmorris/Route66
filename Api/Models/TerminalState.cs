using Services.Models;
using Services;
using Services.Translators;
using Util;

namespace Api.Models
{
    public class TerminalState
    {
        public bool NewDataAvailable { get; private set; }

        private readonly IEnumerable<FieldData>[] _fieldData;

        public IEnumerable<FieldData>[] FieldData
        {
            get
            {
                NewDataAvailable = false;
                return _fieldData;
            }
        }

        public TerminalState(string address, int port)
        {
            var tn3270Service = new TN3270Service<IEnumerable<FieldData>>(new Poco3270Translator());
            
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
                FieldData[row] = args.Data;
                NewDataAvailable = true;
            };
        }
    }
}
