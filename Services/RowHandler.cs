using System;
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
        public event EventHandler<RowUpdateEventArgs<T>>? RowUpdated;

        internal void SetBuffer(byte[] bytes, int offset)
        {
            Dirty = true;
            Array.Copy(bytes, 0, Buffer, offset, bytes.Length);
        }

        internal void Update()
        {
            if (Dirty)
            {
                RowUpdated?.Invoke(this, new RowUpdateEventArgs<T>()
                {
                    Data = translator.Translate(Buffer)
                });

                Dirty = false;
            }
        }
    }
}
