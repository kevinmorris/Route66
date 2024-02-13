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

        public XElement Translate(byte[] buffer, IDictionary<int, IDictionary<byte, byte>> attributeSet)
        {
            var rowRoot = new XElement("row");
            XElement? current = null;
            var str = new StringBuilder();

            for (var col = 0; col < buffer.Length; col++)
            {
                if (buffer[col] == 0)
                {
                    if (current != null)
                    {
                        current.Value = str.ToString();
                        current.Add(new XAttribute("length", str.Length));
                        rowRoot.Add(current);
                        str.Clear();
                        current = null;
                    }
                }
                else if (attributeSet.TryGetValue(col, out var attributes))
                {
                    if (attributes.TryGetValue(Attributes.FIELD, out var fieldAttribute))
                    { //This is the start of a field
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

                        if (attributes.TryGetValue(Orders.INSERT_CURSOR, out var cursor) &&
                            cursor == 1)
                        {
                            current.Add(new XAttribute("cursor", true));
                        }

                        var c = EBCDIC.Chars[buffer[col]];
                        str.Append(c);
                    }

                    foreach (var key in attributes.Keys.Except([Attributes.FIELD, Orders.INSERT_CURSOR]))
                    {
                        //TODO: Need to figure out color constants
                        //current?.Add(new XAttribute(Attributes.ExtendedNames[key], Attributes.ExtendedValues[key][attributes[key]]));
                    }
                }
                else if (EBCDIC.Chars.ContainsKey(buffer[col]))
                {
                    var c = EBCDIC.Chars[buffer[col]];
                    str.Append(c);
                }
            }

            if (current != null)
            {
                rowRoot.Add(current);
                current.Add(new XAttribute("length", str.Length));
            }

            return rowRoot;
        }
    }
}
