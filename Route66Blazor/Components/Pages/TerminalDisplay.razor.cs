using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Services;

namespace Route66Blazor.Components.Pages
{
    public partial class TerminalDisplay : IDisposable
    {
        [Inject]
        protected NetworkService<XElement> NetworkService { get; private set; }

        private readonly Row[] _rows = new Row[24];
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

        private void Reset(MouseEventArgs args)
        {

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

                await JS.InvokeAsync<string>("setDotNetObjRef", _tdObjRef);
                await NetworkService.Connect("127.0.0.1", 3270);
            }
        }

        public void Dispose() => _tdObjRef?.Dispose();
    }
}
