namespace Route66Blazor.Models
{
    public record FieldData
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Value { get; set; } = "";
        public bool IsProtected { get; set; }
    };
}
