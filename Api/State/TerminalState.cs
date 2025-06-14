using Services.Models;
using Services;
using Services.Translators;
using Util;

namespace Api.State
{
    public class TerminalState
    {
        public bool NewDataAvailable { get; private set; }

        private readonly IEnumerable<FieldData>[] _fieldData;
        private readonly ITN3270Service<IEnumerable<FieldData>> _tn3270Service;

        public event EventHandler<FieldsChangedEventArgs>? FieldsChanged;

        public IEnumerable<FieldData>[] FieldData
        {
            get
            {
                NewDataAvailable = false;
                return _fieldData;
            }
        }

        public TerminalState(
            ITN3270Service<IEnumerable<FieldData>> tn3270Service,
            string address,
            int port,
            EventHandler<FieldsChangedEventArgs>? customHandler)
        {
            _tn3270Service = tn3270Service;
            _fieldData = new IEnumerable<FieldData>[Util.Constants.SCREEN_HEIGHT];
            if (customHandler != null)
            {
                FieldsChanged += customHandler;
            }

            //_tn3270Service.Handler.GridUpdated += RowUpdatedFunc;
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

        private EventHandler<GridUpdateEventArgs<IEnumerable<FieldData>>> RowUpdatedFunc(int row)
        {
            return (sender, args) =>
            {
                FieldData[row] = args.Data;
                FieldsChanged?.Invoke(this, new FieldsChangedEventArgs(row, args.Data));
                NewDataAvailable = true;
            };
        }
    }
}
