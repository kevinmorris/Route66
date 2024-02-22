using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Util;

namespace Services
{
    /// <summary>
    /// Contains the byte data for a single character display row from TN3270 service.
    /// </summary>
    /// <typeparam name="T">The type of output from the row translator</typeparam>
    /// <param name="row">the row number (y coodindate) of this handler</param>
    /// <param name="translator">the translator responsible for converting the TN3270 display bytes to <code>T</code></param>
    public class RowHandler<T>(int row, I3270Translator<T> translator)
    {
        /// <summary>
        /// The byte buffer that holds display data for the row
        /// </summary>
        public byte[] Buffer { get; } = new byte[Constants.SCREEN_WIDTH];

        /// <summary>
        /// Whether this row data is "dirty" in need of retranslation.
        /// </summary>
        public bool Dirty { get; private set; } = false;
        private readonly IDictionary<int, IDictionary<byte, byte>> _extendedFieldAttr = 
            new Dictionary<int, IDictionary<byte, byte>>();

        private readonly IDictionary<int, IDictionary<string, object>> _route66AttributeSet =
            new Dictionary<int, IDictionary<string, object>>();

        /// <summary>
        /// Fired when the row's buffer has been translated to <code>T</code>
        /// </summary>
        public event EventHandler<RowUpdateEventArgs<T>>? RowUpdated;

        /// <summary>
        /// Sets a single character within the display buffer
        /// </summary>
        /// <param name="col">the index or column (x coordinate) of the character to be set</param>
        /// <param name="d">the character to be set</param>
        public void SetCharacter(int col, byte d)
        {
            if (col < Buffer.Length)
            {
                Dirty = true;
                Buffer[col] = d;
            }
        }

        /// <summary>
        /// Sets the location of the display's "cursor".  This can mean different things
        /// depending on how the bytes are rendered.
        /// </summary>
        /// <param name="col">the index or column (x coordinate) of the cursor</param>
        public void SetCursor(int col)
        {
            SetExtendedAttribute(col, Orders.INSERT_CURSOR, 1);
        }

        /// <summary>
        /// Sets an extended attribute on this row.  The extended field attribute provides additional field definition beyond that
        /// provided by the field attribute.The extended field attribute defines field
        /// characteristics such as color, character set, field validation, field outlining, and
        /// extended highlighting.  This is also where the <code>TN3270Service</code> will set the
        /// main 3270 field attribute that defines a field.
        /// </summary>
        /// <param name="col">the index or column (x coordinate) of the attribute</param>
        /// <param name="attrKey">the key of the attribute</param>
        /// <param name="attrValue">the value of the attribute</param>
        internal void SetExtendedAttribute(int col, byte attrKey, byte attrValue)
        {
            Dirty = true;

            if (!_extendedFieldAttr.ContainsKey(col))
            {
                _extendedFieldAttr[col] = new Dictionary<byte, byte>();
            }

            var attrs = _extendedFieldAttr[col];
            attrs[attrKey] = attrValue;
        }

        /// <summary>
        /// Sets a custom attribute on this row.  These attributes are not part of the
        /// 3270 spec.  Instead, these attributes are domain specific to Route66.
        /// </summary>
        /// <param name="col">the index or column (x coordinate) of the attribute</param>
        /// <param name="attrKey">the key of the attribute</param>
        /// <param name="attrValue">the value of the attribute</param>
        internal void SetRoute66Attribute(int col, string attrKey, object attrValue)
        {
            Dirty = true;

            if (!_route66AttributeSet.ContainsKey(col))
            {
                _route66AttributeSet[col] = new Dictionary<string, object>();
            }

            var attrs = _route66AttributeSet[col];
            attrs[attrKey] = attrValue;
        }

        /// <summary>
        /// Clears this row's buffer.  All characters are set to zero.
        /// </summary>
        public void Reset()
        {
            Dirty = true;
            Array.Fill<byte>(Buffer, 0b0);
            _extendedFieldAttr.Clear();
        }

        /// <summary>
        /// Starts the translation of this row's buffer bytes to <code>T</code>
        /// if the Dirty flag is set.  The row then fires the RowUpdated event with
        /// the translation 
        /// </summary>
        public void Update()
        {
            if (Dirty)
            {
                translator.Row = row;

                RowUpdated?.Invoke(this, new RowUpdateEventArgs<T>()
                {
                    Data = translator.Translate(Buffer, _extendedFieldAttr, _route66AttributeSet)
                });

                Dirty = false;
            }
        }
    }
}
