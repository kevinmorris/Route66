using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Util;

namespace Services
{
    public class RowHandler<T>(int row, I3270Translator<T> translator)
    {
        public byte[] Buffer { get; } = new byte[Constants.SCREEN_WIDTH];
        public bool Dirty { get; private set; } = false;
        private readonly IDictionary<int, IDictionary<byte, byte>> _extendedFieldAttr = 
            new Dictionary<int, IDictionary<byte, byte>>();

        public event EventHandler<RowUpdateEventArgs<T>>? RowUpdated;

        public void SetCharacter(int col, byte d)
        {
            if (col < Buffer.Length)
            {
                Dirty = true;
                Buffer[col] = d;
            }
        }

        public void SetCursor(int col)
        {
            SetExtendedAttribute(col, Orders.INSERT_CURSOR, 1);
        }

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

        public void Reset()
        {
            Dirty = true;
            Array.Fill<byte>(Buffer, 0b0);
            _extendedFieldAttr.Clear();
        }

        public void Update()
        {
            if (Dirty)
            {
                translator.Row = row;

                RowUpdated?.Invoke(this, new RowUpdateEventArgs<T>()
                {
                    Data = translator.Translate(Buffer, _extendedFieldAttr)
                });

                Dirty = false;
            }
        }
    }
}
