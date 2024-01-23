using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Services;

namespace Route66.Components.Pages
{
    public partial class Row
    {
        [Inject]
        protected ILogger<Row> Logger { get; set; }
        
        internal RowTranslator Translator { get; set; } = default;

        private string _content;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                Translator.RowUpdated += OnRowUpdated;
            }
        }

        private void OnRowUpdated(object sender, RowUpdateEventArgs<XElement> e)
        {
            InvokeAsync(() =>
            {
                _content = e.Data.ToString();
                StateHasChanged();
            });
        }
    }
}
