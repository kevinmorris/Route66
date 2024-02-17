using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.Models
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public record FieldData
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Value { get; set; } = "";
        public bool IsProtected { get; set; }
        public int Length { get; set; }
        public bool Dirty { get; set; } = false;
        public int Cursor { get; set; } = -1;
    }
}
