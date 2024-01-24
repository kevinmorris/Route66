using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services
{
    public class RowUpdateHandler<T>(I3270Translator<T> translator)
    {
        public event EventHandler<RowUpdateEventArgs<T>>? RowUpdated;

        internal void Update(byte[] bytes)
        {
            var rowUpdated = RowUpdated;
            rowUpdated?.Invoke(this, new RowUpdateEventArgs<T>()
            {
                Data = translator.Translate(bytes)
            });
        }
    }
}
