using Microsoft.AspNetCore.Components;
using Route66Blazor.Models;
using Services.Models;

namespace Route66Blazor.Components.Fields
{
    public interface IFieldComponent
    {
        [Parameter]
        public FieldData FieldData { get; set; }
    }
}
