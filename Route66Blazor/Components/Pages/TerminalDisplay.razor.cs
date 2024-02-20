using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Route66Blazor.Models;
using Services;
using Services.Models;
using Util;

namespace Route66Blazor.Components.Pages
{
    public partial class TerminalDisplay : IDisposable
    {
        [Inject]
        protected TN3270Service<XElement>? NetworkService { get; init;  }

        [SupplyParameterFromQuery]
        public string Address { get; set; }

        [SupplyParameterFromQuery]
        public int Port { get; set; }

        private ElementReference _container;
        private readonly Row[] _rows = new Row[Constants.SCREEN_HEIGHT];
        private (int, int) _cursor = (-1, -1);
        private DotNetObjectReference<TerminalDisplay>? _tdObjRef;
        private readonly Action<(int, int)> _cursorAction;

        public TerminalDisplay()
        {
            _cursorAction = OnFocusChanged;
        }

        protected override void OnInitialized()
        {
            _tdObjRef = DotNetObjectReference.Create(this);
        }

        private void OnFocusChanged((int, int) coords)
        {
            _cursor = coords;
        }

        private async Task KeyDown(KeyboardEventArgs args)
        {
            if (args.Code == "Enter")
            {
                await SendUserData(AID.ENTER);
            }
        }

        private async Task Reset(MouseEventArgs args)
        {
            await Task.CompletedTask;
        }

        private async Task FunctionKey(byte key)
        {
            if (NetworkService != null)
            {
                await NetworkService.SendKeyAsync(key);
            }
        }

        private async Task SendUserData(byte aid)
        {
            if (NetworkService != null)
            {
                var fields = _rows.SelectMany(row => row.FieldData);
                var inputFields = fields.Where(f => f is { IsProtected: false, Dirty: true });

                var cursorField = inputFields.FirstOrDefault(f => f.Row == _cursor.Item1 && f.Col == _cursor.Item2) ??
                                  inputFields.LastOrDefault() ??
                                  fields.Last();

                _cursor = (_cursor.Item1, _cursor.Item2 + cursorField.Value.Length);

                await NetworkService.SendFieldsAsync(
                    aid,
                    _cursor.Item1,
                    _cursor.Item2,
                    inputFields);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                for (var i = 0; i < _rows.Length; i++)
                {
                    _rows[i].Handler = NetworkService?.Handlers[i];
                    _rows[i].Index = i;
                }

                await _container.FocusAsync(true);
                await JS.InvokeAsync<string>("setDotNetObjRef", _tdObjRef);
                NetworkService?.Connect(Address, Port);
            }
        }

        public void Dispose() => _tdObjRef?.Dispose();
    }
}
