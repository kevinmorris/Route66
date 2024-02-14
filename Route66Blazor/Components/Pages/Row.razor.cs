using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.AspNetCore.Components;
using Route66Blazor.Components.Fields;
using Route66Blazor.Models;
using Services;
using Services.Models;

namespace Route66Blazor.Components.Pages
{
    public partial class Row
    {
        [Inject]
        protected ILogger<Row> Logger { get; set; }

        internal int Index { get; set; }
        internal RowHandler<XElement>? Handler { get; set; } = default;
        private FieldData[] _fieldData = [];
        public IEnumerable<FieldData> FieldData => _fieldData;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender && Handler != null)
            {
                Handler.RowUpdated += OnRowUpdated;
            }
        }

        private void OnRowUpdated(object sender, RowUpdateEventArgs<XElement> e)
        {
            _fieldData = (from element in e.Data.Elements()
                select new FieldData
                {
                    Row = int.Parse(element.Attribute("row").Value),
                    Col = int.Parse(element.Attribute("col").Value),
                    Value = element.Value,
                    IsProtected = element.Name.ToString() != "input"
                }).ToArray();

            InvokeAsync(StateHasChanged);
        }
    }
}
