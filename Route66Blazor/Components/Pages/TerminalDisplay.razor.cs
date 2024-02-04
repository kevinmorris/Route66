using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Services;

namespace Route66Blazor.Components.Pages
{
    public partial class TerminalDisplay
    {
        [Inject]
        protected NetworkService<XElement> NetworkService { get; private set; }

        private readonly Row[] _rows = new Row[24];

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
                    if (_rows[i] != null)
                    {
                        _rows[i].Handler = NetworkService.Handlers[i];
                    }
                }

                await NetworkService.Connect("127.0.0.1", 3270);
            }
        }
    }
}
