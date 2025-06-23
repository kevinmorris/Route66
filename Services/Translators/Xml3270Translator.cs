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
    /// <summary>
    /// Translates 3270 row data and attributes to XML.
    /// </summary>
    public class Xml3270Translator : I3270Translator<XElement>
    {
        /// <summary>
        /// </summary>
        /// <see cref="I3270Translator{T}"/>
        /// <param name="buffer"></param>
        /// <param name="attributeSet"></param>
        /// <param name="route66AttributeSet"></param>
        /// <returns></returns>
        public XElement Translate(
            byte[] buffer,
            IDictionary<byte, byte>[] attributeSet,
            IDictionary<string, object>[] route66AttributeSet,
            int cursor)
        {
            var gridRoot = new XElement("grid");
            XElement? currentRow = null;
            XElement? current = null;
            Coords? nextCoords = null;
            var str = new StringBuilder();

            byte fieldAttribute = 0x00;
            var address = -1;
            var inputRowContinuation = false;

            for (var index = 0; index < buffer.Length; index++)
            {
                var d = buffer[index];
                //We're starting a new row
                if (index % Constants.SCREEN_WIDTH == 0)
                {
                    if (current != null)
                    {
                        inputRowContinuation = current.Name.LocalName == "input";
                        current.Value = str.ToString();
                        current.Add(new XAttribute("length", str.Length));
                        currentRow?.Add(current);
                        current = null;
                        str.Clear();
                    }

                    currentRow?.Elements("label")
                        .Where(e => string.IsNullOrWhiteSpace(e.Value))
                        .Remove();
                    currentRow = new XElement("row", new XAttribute("i", index / Constants.SCREEN_WIDTH));
                    gridRoot.Add(currentRow);
                }

                if (nextCoords != null)
                {
                    current = new XElement(
                        BinaryUtil.IsProtected(fieldAttribute) ? "label" : "input",
                        new XAttribute("row", nextCoords.Value.Row),
                        new XAttribute("col", nextCoords.Value.Col));

                    if (!BinaryUtil.IsProtected(fieldAttribute))
                    {
                        current.Add(new XAttribute("address", address));
                    }

                    nextCoords = null;
                }

                var structuredFieldStart =
                    attributeSet[index].TryGetValue(Attributes.FIELD, out fieldAttribute);

                if (structuredFieldStart)
                {
                    inputRowContinuation = false;
                    if (current != null)
                    {
                        current.Value = str.ToString();
                        current.Add(new XAttribute("length", str.Length));
                        currentRow?.Add(current);
                        current = null;
                        str.Clear();
                        address = 0x00;
                    }

                    if (route66AttributeSet[index].TryGetValue(Route66Attributes.ADDRESS, out var attribute))
                    {
                        address = int.Parse(attribute.ToString() ?? "-1");
                    }

                    nextCoords = new Coords(
                        index / Constants.SCREEN_WIDTH,
                        index % Constants.SCREEN_WIDTH).Increment();

                    continue;
                }


                if (EBCDIC.Chars.ContainsKey(d))
                {
                    current ??= new XElement(inputRowContinuation ? "input" : "label",
                            new XAttribute("row", index / Constants.SCREEN_WIDTH),
                            new XAttribute("col", index % Constants.SCREEN_WIDTH));

                    if (inputRowContinuation)
                    {
                        current.Add(new XAttribute("address", address));
                    }

                    inputRowContinuation = false;
                    var c = EBCDIC.Chars[d];
                    if (cursor == index)
                    {
                        current.Add(new XAttribute("cursor", str.Length));
                    }

                    str.Append(c);
                }

                if (d == 0 && current?.Name.LocalName == "input")
                {
                    str.Append(' ');
                    if (cursor == index)
                    {
                        current.Add(new XAttribute("cursor", str.Length));
                    }

                }
            }

            if (current != null)
            {
                current.Value = str.ToString();
                current.Add(new XAttribute("length", str.Length));
                currentRow?.Add(current);
            }

            currentRow?.Elements("label")
                .Where(e => string.IsNullOrWhiteSpace(e.Value))
                .Remove();

            return gridRoot;
        }
    }
}
