using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Services.Models;
using Util;

namespace Services.Translators
{
    public class Poco3270Translator : I3270Translator<IList<FieldData>>
    {
        public int Row { get; set; }
        public IList<FieldData> Translate(
            byte[] buffer,
            IDictionary<int, IDictionary<byte, byte>> attributeSet,
            IDictionary<int, IDictionary<string, object>> route66AttributeSet)
        {
            var rowRoot = new List<FieldData>();
            FieldData? current = null;
            var str = new StringBuilder();

            for (var col = 0; col < buffer.Length; col++)
            {
                byte fieldAttribute = 0x00;
                var structuredFieldStart =
                    attributeSet.TryGetValue(col, out var attributes) &&
                     attributes.TryGetValue(Attributes.FIELD, out fieldAttribute);

                var address = -1;
                if (route66AttributeSet.TryGetValue(col, out var route66Attributes) &&
                    route66Attributes.TryGetValue(Route66Attributes.ADDRESS, out var attribute))
                {
                    address = int.Parse(attribute?.ToString() ?? "-1");
                }

                var structuredFieldAhead =
                    attributeSet.TryGetValue(col + 1, out var attributesAhead) &&
                     attributesAhead.ContainsKey(Attributes.FIELD);

                if (EBCDIC.Chars.ContainsKey(buffer[col]))
                {
                    if (structuredFieldStart)
                    {
                        if (current != null)
                        {   
                            current.Value = str.ToString();
                            current.Length = str.Length;
                            rowRoot.Add(current);
                            str.Clear();
                        }

                        current = new FieldData()
                        {
                            IsProtected = BinaryUtil.isProtected(fieldAttribute),
                            Row = Row,
                            Col = col,
                            Address = address
                        };
                    }
                    else current ??= new FieldData()
                    {
                        IsProtected = BinaryUtil.isProtected(fieldAttribute),
                        Row = Row,
                        Col = col,
                        Address = address
                    };

                    var c = EBCDIC.Chars[buffer[col]];
                    if ((attributes?.TryGetValue(Orders.INSERT_CURSOR, out var cursor) ?? false) &&
                        cursor == 1)
                    {
                        current.Cursor = col;
                    }

                    str.Append(c);
                }
                else if (structuredFieldStart)
                {
                    if (current != null)
                    {
                        current.Value = str.ToString();
                        current.Length = str.Length;
                        rowRoot.Add(current);
                        str.Clear();
                    }

                    current = new FieldData()
                    {
                        IsProtected = BinaryUtil.isProtected(fieldAttribute),
                        Row = Row,
                        Col = col,
                        Address = address
                    };

                    if ((attributes?.TryGetValue(Orders.INSERT_CURSOR, out var cursor) ?? false) &&
                        cursor == 1)
                    {
                        current.Cursor = col;
                    }

                    str.Append(' ');
                }
                else if (!(current?.IsProtected ?? true) && !structuredFieldAhead)
                {
                    str.Append(' ');
                }
            }

            if (current != null)
            {
                current.Value = str.ToString();
                current.Length = str.Length;
                rowRoot.Add(current);
                str.Clear();
            }

            return rowRoot;
        }
    }
}
