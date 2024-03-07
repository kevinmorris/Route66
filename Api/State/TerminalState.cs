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
        private readonly ITN3270Service<IEnumerable<FieldData>> _tn3270Service;

        public IEnumerable<FieldData>[] FieldData
        {
            get
            {
                NewDataAvailable = false;
                return _fieldData;
            }
        }

        public TerminalState(ITN3270Service<IEnumerable<FieldData>> tn3270Service, string address, int port)
        {
            _tn3270Service = tn3270Service;
            _fieldData = new IEnumerable<FieldData>[Util.Constants.SCREEN_HEIGHT];
            for (var i = 0; i < _tn3270Service.Handlers.Length; i++)
            {
                _tn3270Service.Handlers[i].RowUpdated += RowUpdatedFunc(i);
            }

            _tn3270Service.Connect(address, port);
        }

        public async Task SendFields(FieldSubmission submission)
        {
            await _tn3270Service.SendFieldsAsync(
                (byte)submission.Aid, 
                submission.CursorRow,
                submission.CursorCol, 
                submission.FieldData);
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
