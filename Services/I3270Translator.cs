using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface I3270Translator<out T>
    {
        public T Translate(byte[] buffer, IDictionary<int, IDictionary<byte, byte>> attributes);
    }
}
