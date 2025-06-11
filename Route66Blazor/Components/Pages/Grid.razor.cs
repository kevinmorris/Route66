using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Services;
using Services.Models;
using Util;

namespace Route66Blazor.Components.Pages
{
    public partial class Grid
    {
        [Inject]
        protected ILogger<Grid> Logger { get; set; }

        private IGridHandler<XElement> _handler;

        internal IGridHandler<XElement> Handler
        {
            get => _handler;
            set
            {
                _handler = value;
                _handler.GridUpdated += OnGridUpdated;
            }
        }

        private readonly object _fieldDataLock = new();
        public IList<FieldData>[] FieldData { get; private set; } = new IList<FieldData>[Constants.SCREEN_HEIGHT];

        protected override void OnInitialized()
        {
            for (var i = 0; i < FieldData.Length; i++)
            {
                FieldData[i] = new List<FieldData>();
            }
        }

        private void OnGridUpdated(object? sender, GridUpdateEventArgs<XElement> e)
        {
            var updatedFieldData = new IList<FieldData>[Constants.SCREEN_HEIGHT];
            for (var i = 0; i < FieldData.Length; i++)
            {
                updatedFieldData[i] = new List<FieldData>();
            }

            foreach (var row in e.Data.Elements())
            {
                var iAttr = row.Attribute("i") ??
                            throw new InvalidOperationException($"Row {row} missing index attribute");

                var i = int.Parse(iAttr.Value);
                foreach (var field in row.Elements())
                {
                    var fieldData = new FieldData
                    {
                        Row = int.Parse(field.Attribute("row").Value),
                        Col = int.Parse(field.Attribute("col").Value),
                        Value = field.Value,
                        IsProtected = field.Name.ToString() != "input",
                        Address = field.Attribute("address")?
                            .Let(a => int.Parse(a.Value)) ?? -1,
                        Length = int.Parse(field.Attribute("length").Value),
                        Cursor = field.Attribute("cursor")?
                            .Let(c => int.Parse(c.Value)) ?? -1
                    };
                    updatedFieldData[i].Add(fieldData);
                }
            }

            lock (_fieldDataLock)
            {
                FieldData = updatedFieldData;
            }

            InvokeAsync(StateHasChanged);
        }
    }
}
