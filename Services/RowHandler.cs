﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Util;

namespace Services
{
    public class RowHandler<T>(I3270Translator<T> translator)
    {
        public byte[] Buffer { get; } = new byte[Constants.SCREEN_WIDTH];
        public bool Dirty { get; private set; } = false;

        private readonly IDictionary<int, IDictionary<byte, byte>> _extendedFieldAttr = 
            new Dictionary<int, IDictionary<byte, byte>>();

        public event EventHandler<RowUpdateEventArgs<T>>? RowUpdated;

        internal void SetCharacters(byte[] bytes, int offset)
        {
            Dirty = true;
            Array.Copy(bytes, 0, Buffer, offset, bytes.Length);
        }

        internal void SetExtendedAttribute(int index, byte attrKey, byte attrValue)
        {
            Dirty = true;

            if (!_extendedFieldAttr.ContainsKey(index))
            {
                _extendedFieldAttr[index] = new Dictionary<byte, byte>();
            }

            var attrs = _extendedFieldAttr[index];
            attrs[attrKey] = attrValue;
        }

        public void Update()
        {
            if (Dirty)
            {
                RowUpdated?.Invoke(this, new RowUpdateEventArgs<T>()
                {
                    Data = translator.Translate(Buffer, _extendedFieldAttr)
                });

                Dirty = false;
            }
        }
    }
}