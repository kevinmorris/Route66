using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;

namespace Tests.Helpers
{
    public class ByteArray3270Translator : I3270Translator<(byte[], IDictionary<int, IDictionary<byte, byte>>)>
    {
        public (byte[], IDictionary<int, IDictionary<byte, byte>>) Translate(byte[] buffer, IDictionary<int, IDictionary<byte, byte>> attributes)
        {
            return (buffer, attributes);
        }
    }
}
