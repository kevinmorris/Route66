using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Services;

namespace Route66.Components.Pages
{
    public partial class TerminalDisplay
    {
        [Inject]
        protected NetworkService<XElement> NetworkService { get; private set; }

        private readonly Row[] _rows = new Row[24];

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                for (var i = 0; i < _rows.Length; i++)
                {
                    if (_rows[i] != null)
                    {
                        _rows[i].UpdateHandler = NetworkService.Handlers[i];
                    }
                }
            }
        }
    }
}
