using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.AspNetCore.Components;
using Route66Blazor.Components.Fields;
using Route66Blazor.Models;
using Services;

namespace Route66Blazor.Components.Pages
{
    public partial class Row
    {
        [Inject]
        protected ILogger<Row> Logger { get; set; }

        internal int Index { get; set; }
        internal RowHandler<XElement>? Handler { get; set; } = default;

        private IEnumerable<FieldData> _fieldData = new List<FieldData>();

        private XslCompiledTransform _xslt = new(true);

        public Row()
        {
            _xslt.Load("xml-tools/html-transform.xsl");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && Handler != null)
            {
                Handler.RowUpdated += OnRowUpdated;
            }
        }

        private void OnRowUpdated(object sender, RowUpdateEventArgs<XElement> e)
        {
            _fieldData = from element in e.Data.Elements()
                select new FieldData(
                    int.Parse(element.Attribute("row").Value),
                    int.Parse(element.Attribute("col").Value),
                    element.Value,
                    element.Name.ToString() != "input");

            InvokeAsync(StateHasChanged);
        }
    }
}
