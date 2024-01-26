using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Services
{
    public static class Telnet
    {
        public const byte IAC = 0xff;
        public const byte BINARY_TRANSMISSION = 0x00;
        public const byte TERMINAL_TYPE = 0x18;
        public const byte END_OF_RECORD = 0x19;

        public const byte WILL = 0xfb;
        public const byte WONT = 0xfc;
        public const byte DO = 0xfd;
        public const byte DONT = 0xfe;
    }

    public static class Commands
    {
    }

    public static class WCC
    {
    }

    public static class Orders
    {
    }

    public static class AID
    {
    }
}
