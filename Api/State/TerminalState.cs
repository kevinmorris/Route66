using Services.Models;
using Services;
using Services.Translators;
using Util;

namespace Api.State
{
    public class TerminalState
    {
        public bool NewDataAvailable { get; private set; }

        private IEnumerable<FieldData>[] _fieldData;
        private readonly ITN3270Service<IEnumerable<FieldData>> _tn3270Service;

        public event EventHandler<FieldsChangedEventArgs>? FieldsChanged;

        public IEnumerable<FieldData>[] FieldData
        {
            get
            {
                NewDataAvailable = false;
                return _fieldData;
            }

            set
            {
                NewDataAvailable = true;
                _fieldData = value;
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

            _tn3270Service.Handler.GridUpdated += GridUpdatedFunc();
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

        private EventHandler<GridUpdateEventArgs<IEnumerable<FieldData>>> GridUpdatedFunc()
        {
            return (sender, args) =>
            {
                FieldData = args.Data.GroupBy(
                    field => field.Row,
                    field => field,
                    (k, v) => v).ToArray();
                FieldsChanged?.Invoke(this, new FieldsChangedEventArgs(FieldData));
                NewDataAvailable = true;
            };
        }
    }
}
