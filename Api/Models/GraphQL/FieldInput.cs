namespace Api.Models.GraphQL
{
    public record FieldInput
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Value { get; set; } = "";
        public int Address { get; set; } = -1;
    }
}
