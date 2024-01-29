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

        public static IEnumerable<byte> ALL =
        [
            WRITE,
            ERASE_WRITE,
            READ_BUFFER,
            READ_MODIFIED,
            READ_MODIFIED_ALL,
            ERASE_ALL_UNPROTECTED,
            WRITE_STRUCTURED_FIELD
        ];
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

        public static IEnumerable<byte> ALL =
        [
            START_FIELD,
            START_FIELD_EXTENDED,
            SET_BUFFER_ADDRESS,
            SET_ATTRIBUTE,
            MODIFY_FIELD,
            INSERT_CURSOR,
            PROGRAM_TAB,
            REPEAT_TO_ADDRESS,
            ERASE_UNPROTECTED_TO_ADDRESS,
            GRAPHIC_ESCAPE
        ];
    }

    public static class Attributes
    {
        public const byte ALL = 0x00;
        public const byte FIELD = 0xc0;
        public const byte VALIDATION = 0xc1;
        public const byte OUTLINE = 0xc2;
        public const byte HIGHLIGHT = 0x41;
        public const byte FOREGROUND_COLOR = 0x42;
        public const byte CHARACTER_SET = 0x43;
        public const byte BACKGROUND_COLOR = 0x45;
        public const byte TRANSPARENCY = 0x46;
    }

    public static class Colors
    {
        public const byte NEUTRAL_BLACK = 0x00;
        public const byte BLUE = 0x01;
        public const byte RED = 0x02;
        public const byte PINK = 0x03;
        public const byte GREEN = 0x04;
        public const byte TURQUOISE = 0x05;
        public const byte YELLOW = 0x06;
        public const byte NEUTRAL_WHITE = 0x07;
        public const byte BLACK = 0x08;
        public const byte DEEP_BLUE = 0x09;
        public const byte ORANGE = 0x0a;
        public const byte PURPLE = 0x0b;
        public const byte PALE_GREEN = 0x0c;
        public const byte PALE_TURQUOISE = 0x0d;
        public const byte GRAY = 0x0e;
        public const byte WHITE = 0x0f;

        public static IDictionary<byte, string> ColorClasses { get; }

        static Colors()
        {
            ColorClasses = new Dictionary<byte, string>()
            {
                [NEUTRAL_BLACK] = "neutral_black",
                [BLUE] = "blue",
                [RED] = "red",
                [PINK] = "pink",
                [GREEN] = "green",
                [TURQUOISE] = "turquoise",
                [YELLOW] = "yellow",
                [NEUTRAL_WHITE] = "neutral_white",
                [BLACK] = "black",
                [DEEP_BLUE] = "deep_blue",
                [ORANGE] = "orange",
                [PURPLE] = "purple",
                [PALE_GREEN] = "pale_green",
                [PALE_TURQUOISE] = "pale_turquoise",
                [GRAY] = "gray",
                [WHITE] = "white"
            };
        }
    }

    public static class AID
    {
    }
}
