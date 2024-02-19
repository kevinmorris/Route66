using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Util;

namespace Services
{
    public class Xml3270Translator : I3270Translator<XElement>
    {
        public int Row { get; set; }

        public XElement Translate(
            byte[] buffer,
            IDictionary<int, IDictionary<byte, byte>> attributeSet,
            IDictionary<int, IDictionary<string, object>> route66AttributeSet)
        {
            var rowRoot = new XElement("row");
            XElement? current = null;
            var str = new StringBuilder();

            for (var col = 0; col < buffer.Length; col++)
            {
                byte fieldAttribute = 0x00;
                var structuredFieldStart =
                    (attributeSet.TryGetValue(col, out var attributes) && 
                     attributes.TryGetValue(Attributes.FIELD, out fieldAttribute));

                var address = -1;
                if (route66AttributeSet.TryGetValue(col, out var route66Attributes) &&
                    route66Attributes.TryGetValue(Route66Attributes.ADDRESS, out var attribute))
                {
                    address = int.Parse(attribute?.ToString() ?? "-1");
                }

                var structuredFieldAhead =
                    (attributeSet.TryGetValue(col + 1, out var attributesAhead) &&
                     attributesAhead.ContainsKey(Attributes.FIELD));

                if (EBCDIC.Chars.ContainsKey(buffer[col]))
                {
                    if(structuredFieldStart)
                    {
                        if (current != null)
                        {
                            current.Value = str.ToString();
                            current.Add(new XAttribute("length", str.Length));
                            rowRoot.Add(current);
                            str.Clear();
                        }

                        current = new XElement(
                            BinaryUtil.isProtected(fieldAttribute) ? "label" : "input",
                            new XAttribute("row", Row),
                            new XAttribute("col", col));

                        if (!BinaryUtil.isProtected(fieldAttribute))
                        {
                            current.Add(new XAttribute("address", address));
                        }
                    }
                    else current ??= new XElement(
                            "label",
                            new XAttribute("row", Row),
                            new XAttribute("col", col));

                    var c = EBCDIC.Chars[buffer[col]];
                    if ((attributes?.TryGetValue(Orders.INSERT_CURSOR, out var cursor) ?? false) &&
                        cursor == 1)
                    {
                        current.Add(new XAttribute("cursor", str.Length));
                    }

                    str.Append(c);
                }
                else if(structuredFieldStart)
                {
                    if (current != null)
                    {
                        current.Value = str.ToString();
                        current.Add(new XAttribute("length", str.Length));
                        rowRoot.Add(current);
                        str.Clear();
                    }

                    current = new XElement(
                        BinaryUtil.isProtected(fieldAttribute) ? "label" : "input",
                        new XAttribute("row", Row),
                        new XAttribute("col", col));

                    if (!BinaryUtil.isProtected(fieldAttribute))
                    {
                        current.Add(new XAttribute("address", address));
                    }

                    if ((attributes?.TryGetValue(Orders.INSERT_CURSOR, out var cursor) ?? false) &&
                        cursor == 1)
                    {
                        current.Add(new XAttribute("cursor", str.Length));
                    }

                    str.Append(' ');
                }
                else if (current?.Name.LocalName == "input" && !structuredFieldAhead)
                {
                    str.Append(' ');
                }
            }

            if (current != null)
            {
                current.Value = str.ToString();
                current.Add(new XAttribute("length", str.Length));
                rowRoot.Add(current);
            }

            return rowRoot;
        }
    }
}
