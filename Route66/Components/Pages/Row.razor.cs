using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.AspNetCore.Components;
using Services;

namespace Route66Blazor.Components.Pages
{
    public partial class Row
    {
        [Inject]
        protected ILogger<Row> Logger { get; set; }
        
        internal RowHandler<XElement> Handler { get; set; } = default;

        private MarkupString _content;
        private XslCompiledTransform _xslt = new XslCompiledTransform(true);

        public Row()
        {
            _xslt.Load("xml-tools/html-transform.xsl");
        }


        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                Handler.RowUpdated += OnRowUpdated;
            }
        }

        private void OnRowUpdated(object sender, RowUpdateEventArgs<XElement> e)
        {
            var xmlReader = new XmlTextReader(new StringReader(e.Data.ToString()));

            var htmlSb = new StringBuilder();
            _xslt.Transform(xmlReader, XmlWriter.Create(new StringWriter(htmlSb)));
            var html = htmlSb.ToString();

            InvokeAsync(() =>
            {
                _content = new MarkupString(html);
                StateHasChanged();
            });
        }
    }
}
