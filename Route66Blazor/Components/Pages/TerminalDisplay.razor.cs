using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Services;
using Util;

namespace Route66Blazor.Components.Pages
{
    public partial class TerminalDisplay : IDisposable
    {
        [Inject]
        protected NetworkService<XElement> NetworkService { get; private set; }

        private ElementReference _container;
        private readonly Row[] _rows = new Row[Constants.SCREEN_HEIGHT];
        private DotNetObjectReference<TerminalDisplay>? _tdObjRef;

        private IJSObjectReference? _module;

        protected override void OnInitialized()
        {
            _tdObjRef = DotNetObjectReference.Create(this);
        }

        [JSInvokable]
        public async Task<string> InputChangedAsync(object e)
        {
            return await Task.FromResult(e.ToString());
        }

        private void KeyDown(KeyboardEventArgs args)
        {

        }

        private void Reset(MouseEventArgs args)
        {

        }

        private async void Clear(MouseEventArgs args)
        {
            await NetworkService.SendKeyAsync(AID.CLEAR);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                for (var i = 0; i < _rows.Length; i++)
                {
                    _rows[i].Handler = NetworkService.Handlers[i];
                    _rows[i].Index = i;
                }

                await _container.FocusAsync(true);
                await JS.InvokeAsync<string>("setDotNetObjRef", _tdObjRef);
                NetworkService.Connect("127.0.0.1", 3270);
            }
        }

        public void Dispose() => _tdObjRef?.Dispose();
    }
}
