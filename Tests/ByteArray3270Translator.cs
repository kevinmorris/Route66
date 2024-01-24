using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;

namespace Tests
{
    public class ByteArray3270Translator : I3270Translator<byte[]>
    {
        public byte[] Translate(byte[] buffer)
        {
            return buffer;
        }
    }
}
