using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class RowUpdateEventArgs<T> : EventArgs
    {
        public T Data { get; init; }
    }
}
