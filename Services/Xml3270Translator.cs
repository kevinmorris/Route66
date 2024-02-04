using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services
{
    public class Xml3270Translator : I3270Translator<XElement>
    {
        public XElement Translate(byte[] buffer, IDictionary<int, IDictionary<byte, byte>> attributeSet)
        {
            var rowRoot = new XElement("row");
            XElement? current = null;
            var str = new StringBuilder();

            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == 0)
                {
                    if (current != null)
                    {
                        current.Value = str.ToString();
                        rowRoot.Add(current);
                        str.Clear();
                        current = null;
                    }
                }
                else if (attributeSet.TryGetValue(i, out var attributes))
                {
                    if (attributes.ContainsKey(Attributes.FIELD))
                    { //This is the start of a field

                        if (current != null)
                        {
                            current.Value = str.ToString();
                            rowRoot.Add(current);
                            str.Clear();
                        }

                        current = new XElement("label", new XAttribute("col", i + 1));
                    }

                    foreach (var key in attributes.Keys.Except([Attributes.FIELD]))
                    {
                        current?.Add(new XAttribute(Attributes.ExtendedNames[key], Attributes.ExtendedValues[key][attributes[key]]));
                    }
                }
                else if (EBCDIC.Chars.ContainsKey(buffer[i]))
                {
                    var c = EBCDIC.Chars[buffer[i]];
                    str.Append(c);
                }
            }

            return rowRoot;
        }
    }
}
