using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Services.Models;
using Util;

namespace Services.Translators
{
    public class Poco3270Translator : I3270Translator<IList<FieldData>>
    {
        public IList<FieldData> Translate(
            byte[] buffer,
            IDictionary<byte, byte>[] attributeSet,
            IDictionary<string, object>[] route66AttributeSet,
            int cursor)
        {
            var gridRoot = new List<FieldData>();
            FieldData? current = null;
            var address = -1;
            Coords? nextCoords = null;
            byte fieldAttribute = 0x00;
            var inputRowContinuation = false;


            var str = new StringBuilder();

            for (var index = 0; index < buffer.Length; index++)
            {
                var d = buffer[index];
                var row = index / Constants.SCREEN_WIDTH;
                var col = index % Constants.SCREEN_WIDTH;

                if (index % Constants.SCREEN_WIDTH == 0 && current != null)
                {
                    inputRowContinuation = !current.IsProtected;
                    current.Value = str.ToString();
                    current.Length = str.Length;
                    gridRoot.Add(current);
                    current = null;
                    str.Clear();
                }

                if (nextCoords != null)
                {
                    current = new FieldData()
                    {
                        IsProtected = BinaryUtil.IsProtected(fieldAttribute),
                        Row = row,
                        Col = col,
                        Address = address
                    };

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
                        current.Length = str.Length;
                        gridRoot.Add(current);
                        str.Clear();
                    }

                    if (route66AttributeSet[index].TryGetValue(Route66Attributes.ADDRESS, out var attribute))
                    {
                        address = int.Parse(attribute?.ToString() ?? "-1");
                    }

                    nextCoords = new Coords(
                        index / Constants.SCREEN_WIDTH,
                        index % Constants.SCREEN_WIDTH).Increment();

                    continue;
                }

                if (EBCDIC.Chars.ContainsKey(d))
                {
                    current ??= new FieldData()
                    {
                        IsProtected = !inputRowContinuation,
                        Row = row,
                        Col = col,
                    };

                    if (inputRowContinuation)
                    {
                        current.Address = address;
                    }

                    inputRowContinuation = false;
                    var c = EBCDIC.Chars[d];
                    if (cursor == index)
                    {
                        current.Cursor = index;
                    }

                    str.Append(c);
                }

                if(d == 0 && !(current?.IsProtected ?? true))
                {
                    str.Append(' ');
                    if (cursor == index)
                    {
                        current.Cursor = index;
                    }
                }
            }

            if (current != null)
            {
                current.Value = str.ToString();
                current.Length = str.Length;
                gridRoot.Add(current);
                str.Clear();
            }

            gridRoot = gridRoot.FindAll(field => !(field.IsProtected && string.IsNullOrWhiteSpace(field.Value)));

            return gridRoot;
        }
    }
}
