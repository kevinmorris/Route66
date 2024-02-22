using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Event arguments for a row update by a translator
    /// </summary>
    /// <typeparam name="T">the translated data type</typeparam>
    public class RowUpdateEventArgs<T> : EventArgs
    {
        public T Data { get; init; }
    }
}
