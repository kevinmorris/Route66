using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Util
{
    public static class BinaryUtil
    {
        /// <summary>
        /// When 12- or 14-bit address mode is specified for the partition (implicit partition 0 is
        /// always set to 12/14), bits 0 and 1 of the first address byte following an SBA are flag
        /// bits and have the following significance:
        /// 
        /// Setting         Meaning
        /// B '00'          14-bit binary address follows
        /// B '01'          12-bit coded address follows
        /// B '10'          Reserved
        /// B '11'          12-bit coded address follows
        /// 
        /// When the flag bits are 00, the next 14 bits(the remainder of this byte and all 8 bits
        /// of the next byte) contain a buffer address in binary form.No address translation is
        /// necessary.
        ///
        /// If the flag bits are 01 or 11, the next 14 bits are to be interpreted as a 2-character
        /// address (6 bits in each byte). The 6 low-order bits of each byte are joined to
        /// provide a 12-bit address.The address specifies the buffer position, not the line and
        /// column position on the display surface. For example, on a 480-character display,
        /// the buffer addresses are from 0 to 479. To specify a 12-bit buffer address of 160
        /// (binary 000010100000), bits 2-7 of the first byte are set to 000010. Bits 2-7 of the
        /// second byte are set to 100000.
        /// </summary>
        /// <param name="bytes">the 2 address bytes</param>
        /// <returns>the single dimensional address</returns>
        public static int BufferAddress(Span<byte> bytes)
        {
            var address = (bytes[0] << 8) + bytes[1];
            var twelveBitMode = ((address >> 14) & 1) > 0;

            if (twelveBitMode)
            {
                address = ((bytes[0] & 0b00111111) << 6) + (bytes[1] & 0b00111111);
            }
            else
            {
                address &= 0b00111111_11111111;
            }

            return address;
        }

        public static (int, int) AddressCoordinates(int address)
        {
            var y = address / Constants.SCREEN_WIDTH;
            var x = address % Constants.SCREEN_WIDTH;

            return (y, x);
        }

        public static byte[] AddressBuffer12Bit(int address)
        {
            var b1 = (byte)(address & 0b00111111);
            var b0 = (byte)((address - b1) >> 6);

            return [(byte)(0b11000000 + b0), (byte)(0b11000000 + b1)];
        }

        public static int CoordinateAddress((int, int) coords)
        {
            return coords.Item1 * Constants.SCREEN_WIDTH + coords.Item2;
        }

        public static bool isProtected(byte fieldAttribute)
        {
            return (fieldAttribute & 0b0010_0000) > 0;
        }
    }
}
