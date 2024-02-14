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
        protected NetworkService<XElement>? NetworkService { get; init;  }

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

        private async void Clear(MouseEventArgs args)
        {
            //await NetworkService.SendKeyAsync(AID.CLEAR);
        }

        private async Task SendUserData(byte aid)
        {
            var result = await JS.InvokeAsync<UserData>("assembleInputFields");
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
                NetworkService?.Connect("127.0.0.1", 3270);
            }
        }

        public void Dispose() => _tdObjRef?.Dispose();
    }
}
