// ReSharper disable InconsistentNaming

namespace Services
{
    public static class Telnet
    {
        public const byte IAC = 0xff;
        public const byte BINARY_TRANSMISSION = 0x00;
        public const byte TERMINAL_TYPE = 0x18;
        public const byte END_OF_RECORD = 0x19;

        public const byte SUB_OPTION = 0xfa;
        public const byte WILL = 0xfb;
        public const byte WONT = 0xfc;
        public const byte DO = 0xfd;
        public const byte DONT = 0xfe;
    }

    public static class Commands
    {
        public const byte WRITE = 0xf1;
        public const byte ERASE_WRITE = 0xf5;
        public const byte READ_BUFFER = 0xf2;
        public const byte READ_MODIFIED = 0xf6;
        public const byte READ_MODIFIED_ALL = 0x6e;
        public const byte ERASE_ALL_UNPROTECTED = 0x6f;
        public const byte WRITE_STRUCTURED_FIELD = 0xf3;
    }

    public static class WCC
    {
        public const byte RESET = 0b0100_0000;
        public const byte KEYBOARD_RESTORE = 0b0000_0010;
        public const byte RESET_MDT = 0b0000_0001;
    }

    public static class Orders
    {
        public const byte START_FIELD = 0x1d;
        public const byte START_FIELD_EXTENDED = 0x29;
        public const byte SET_BUFFER_ADDRESS = 0x11;
        public const byte SET_ATTRIBUTE = 0x28;
        public const byte MODIFY_FIELD = 0x2c;
        public const byte INSERT_CURSOR = 0x13;
        public const byte PROGRAM_TAB = 0x05;
        public const byte REPEAT_TO_ADDRESS = 0x3c;
        public const byte ERASE_UNPROTECTED_TO_ADDRESS = 0x12;
        public const byte GRAPHIC_ESCAPE = 0x08;
    }

    public static class AID
    {
    }
}
