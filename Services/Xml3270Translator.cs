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
        public XElement Translate(byte[] buffer, IDictionary<int, IDictionary<byte, byte>> attributes)
        {
            var rowRoot = new XElement("row");
            XElement? current = null;
            StringBuilder? str = null;

            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == 0)
                {
                    if (current != null && str != null)
                    {
                        current.SetValue(str.ToString());
                        rowRoot.Add(current);
                        str.Clear();
                        current = null;
                    }
                }
                else if (attributes.Keys.Contains(i) && attributes[i].ContainsKey(Attributes.FIELD))
                { //This is the start of a field
                    if (current != null && str != null)
                    {
                        current.SetValue(str.ToString());
                        rowRoot.Add(current);
                        str.Clear();
                    }

                    current = new XElement("label", new XAttribute("col", i + 1));
                }
                else if (EBCDIC.Chars.Keys.Contains(buffer[i]))
                {
                    str?.Append(EBCDIC.Chars[buffer[i]]);
                }
            }

            return rowRoot;
        }
    }
}
