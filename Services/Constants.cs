﻿// ReSharper disable InconsistentNaming

namespace Services
{
    public static class Telnet
    {
        public const int SCREEN_WIDTH = 80;
        public const int SCREEN_HEIGHT = 24;

        public const byte IAC = 0xff;
        public const byte BINARY_TRANSMISSION_ABILITY = 0x00;
        public const byte TERMINAL_TYPE_ABILITY = 0x18;
        public const byte END_OF_RECORD_ABILITY = 0x19;
        public const byte END_OF_RECORD = 0xef;

        public const byte SUB_OPTION = 0xfa;
        public const byte WILL = 0xfb;
        public const byte WONT = 0xfc;
        public const byte DO = 0xfd;
        public const byte DONT = 0xfe;

        public static byte[] IBM32792E =
        [
            IAC,
            SUB_OPTION,
            TERMINAL_TYPE_ABILITY,
            0x00,
            0x49,
            0x42,
            0x4d,
            0x2d,
            0x33,
            0x32,
            0x37,
            0x39,
            0x2d,
            0x32,
            0x2d,
            0x45,
            IAC,
            0xf0
        ];
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

    public static class Route66Attributes
    {
        public const string ADDRESS = "address";
    }

    public static class Attributes
    {
        public const byte ALL = 0x00;
        public const byte FIELD = 0xc0;
        public const byte VALIDATION = 0xc1;
        public const byte OUTLINE = 0xc2;
        public const byte HIGHLIGHTING = 0x41;
        public const byte FOREGROUND_COLOR = 0x42;
        public const byte CHARACTER_SET = 0x43;
        public const byte BACKGROUND_COLOR = 0x45;
        public const byte TRANSPARENCY = 0x46;

        public static IDictionary<byte, string> ExtendedNames = new Dictionary<byte, string>()
        {
            [HIGHLIGHTING] = "highlighting",
            [FOREGROUND_COLOR] = "foreground-color",
            [BACKGROUND_COLOR] = "background-color",
        };

        public static IDictionary<byte, IDictionary<byte, string>> ExtendedValues =
            new Dictionary<byte, IDictionary<byte, string>>()
            {
                [HIGHLIGHTING] = Highlighting.Classes,
                [FOREGROUND_COLOR] = Colors.Classes,
                [BACKGROUND_COLOR] = Colors.Classes,
            };
    }

    public static class EBCDIC
    {
        public static IDictionary<byte, char> Chars = new Dictionary<byte, char>()
        {
            [0x40] = ' ',
            [0x4A] = '[',
            [0x4B] = '.',
            [0x4C] = '<',
            [0x4D] = '(',
            [0x4E] = '+',
            [0x4F] = '!',
            [0x50] = '&',
            [0x5A] = ']',
            [0x5B] = '$',
            [0x5C] = '*',
            [0x5D] = ')',
            [0x5E] = ';',
            [0x5F] = '^',
            [0x60] = '-',
            [0x61] = '/',
            [0x6A] = '|',
            [0x6B] = ',',
            [0x6C] = '%',
            [0x6D] = '_',
            [0x6E] = '>',
            [0x6F] = '?',
            [0x79] = '`',
            [0x7A] = ':',
            [0x7B] = '#',
            [0x7C] = '@',
            [0x7D] = '\'',
            [0x7E] = '=',
            [0x7F] = '"',
            [0x81] = 'a',
            [0x82] = 'b',
            [0x83] = 'c',
            [0x84] = 'd',
            [0x85] = 'e',
            [0x86] = 'f',
            [0x87] = 'g',
            [0x88] = 'h',
            [0x89] = 'i',
            [0x91] = 'j',
            [0x92] = 'k',
            [0x93] = 'l',
            [0x94] = 'm',
            [0x95] = 'n',
            [0x96] = 'o',
            [0x97] = 'p',
            [0x98] = 'q',
            [0x99] = 'r',
            [0xA1] = '~',
            [0xA2] = 's',
            [0xA3] = 't',
            [0xA4] = 'u',
            [0xA5] = 'v',
            [0xA6] = 'w',
            [0xA7] = 'x',
            [0xA8] = 'y',
            [0xA9] = 'z',
            [0xC0] = '{',
            [0xC1] = 'A',
            [0xC2] = 'B',
            [0xC3] = 'C',
            [0xC4] = 'D',
            [0xC5] = 'E',
            [0xC6] = 'F',
            [0xC7] = 'G',
            [0xC8] = 'H',
            [0xC9] = 'I',
            [0xD0] = '}',
            [0xD1] = 'J',
            [0xD2] = 'K',
            [0xD3] = 'L',
            [0xD4] = 'M',
            [0xD5] = 'N',
            [0xD6] = 'O',
            [0xD7] = 'P',
            [0xD8] = 'Q',
            [0xD9] = 'R',
            [0xE0] = '\\',
            [0xE2] = 'S',
            [0xE3] = 'T',
            [0xE4] = 'U',
            [0xE5] = 'V',
            [0xE6] = 'W',
            [0xE7] = 'X',
            [0xE8] = 'Y',
            [0xE9] = 'Z',
            [0xF0] = '0',
            [0xF1] = '1',
            [0xF2] = '2',
            [0xF3] = '3',
            [0xF4] = '4',
            [0xF5] = '5',
            [0xF6] = '6',
            [0xF7] = '7',
            [0xF8] = '8',
            [0xF9] = '9',
        };

        public static IDictionary<char, byte> EBCDICBytes =
            Chars.ToDictionary(x => x.Value, x => x.Key);
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

        public static IDictionary<byte, string> Classes { get; }

        static Colors()
        {
            Classes = new Dictionary<byte, string>()
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

    public static class Highlighting
    {
        public const byte DEFAULT = 0x00;
        public const byte NORMAL = 0xf0;
        public const byte BLINK = 0xf1;
        public const byte REVERSE_VIDEO = 0xf2;
        public const byte UNDERSCORE = 0xf4;

        public static IDictionary<byte, string> Classes = new Dictionary<byte, string>()
        {
            [0x00] = "default",
            [0xf0] = "normal",
            [0xf1] = "blink",
            [0xf2] = "reverse-video",
            [0xf4] = "underscore"
        };
    }

    public static class AID
    {
        public const byte NO_AID = 0x60;
        public const byte STRUCTURED_FIELD = 0x88;
        public const byte TRIGGER_ACTION = 0x7f;
        public const byte SYSREQ = 0xf0;
        public const byte PF1 = 0xf1;
        public const byte PF2 = 0xf2;
        public const byte PF3 = 0xf3;
        public const byte PF4 = 0xf4;
        public const byte PF5 = 0xf5;
        public const byte PF6 = 0xf6;
        public const byte PF7 = 0xf7;
        public const byte PF8 = 0xf8;
        public const byte PF9 = 0xf9;
        public const byte PF10 = 0x7a;
        public const byte PF11 = 0x7b;
        public const byte PF12 = 0x7c;
        public const byte PF13 = 0xc1;
        public const byte PF14 = 0xc2;
        public const byte PF15 = 0xc3;
        public const byte PF16 = 0xc4;
        public const byte PF17 = 0xc5;
        public const byte PF18 = 0xc6;
        public const byte PF19 = 0xc7;
        public const byte PF20 = 0xc8;
        public const byte PF21 = 0xc9;
        public const byte PF22 = 0x4a;
        public const byte PF23 = 0x4b;
        public const byte PF24 = 0x4c;
        public const byte PA1 = 0x6c;
        public const byte PA2 = 0x6e;
        public const byte PA3 = 0x6b;
        public const byte CLEAR = 0x6d;
        public const byte CLEAR_PARTITION = 0x6a;
        public const byte ENTER = 0x7d;
    }

    public static class StructuredFields
    {
        public const byte READ_PARTITION = 0x01;

        public const byte READ_PARTITION_QUERY = 0x02;

        public static byte[] READ_PARTITION_REPLY =
        [
            0x88, 0x00, 0x0d, 0x81,
            0x80, 0x80, 0x81, 0x85, 0x86, 0x87, 0x88, 0x8f, 0x95, 0xa6, 0x00, 0x17, 0x81, 0x81, 0x01, 0x00,
            0x00, 0x50, 0x00, 0x18, 0x01, 0x00, 0x19, 0x00, 0x50, 0x00, 0x4b, 0x01, 0x00, 0x0c, 0x12, 0x00,
            0x00, 0x00, 0x1f, 0x81, 0x85, 0x82, 0x10, 0x0c, 0x12, 0x00, 0x00, 0x00, 0x00, 0x09, 0x01, 0x00,
            0xf1, 0x03, 0xc3, 0x01, 0x36, 0x01, 0x36, 0x00, 0x10, 0x00, 0x02, 0xb9, 0x04, 0x17, 0x04, 0x17,
            0x00, 0x2a, 0x81, 0x86, 0x00, 0x10, 0x00, 0xf4, 0xf1, 0xf1, 0xf2, 0xf2, 0xf3, 0xf3, 0xf4, 0xf4,
            0xf5, 0xf5, 0xf6, 0xf6, 0xf7, 0xf7, 0xf8, 0xf8, 0xf9, 0xf9, 0xfa, 0xfa, 0xfb, 0xfb, 0xfc, 0xfc,
            0xfd, 0xfd, 0xfe, 0xfe, 0xff, 0xff, 0xff, 0xff, 0x04, 0x02, 0x00, 0xf8, 0x00, 0x0f, 0x81, 0x87,
            0x05, 0x00, 0xf0, 0xf1, 0xf1, 0xf2, 0xf2, 0xf4, 0xf4, 0xf8, 0xf8, 0x00, 0x07, 0x81, 0x88, 0x00,
            0x01, 0x02, 0x00, 0x0c, 0x81, 0x95, 0x00, 0x00, 0x10, 0x00, 0x10, 0x00, 0x01, 0x01, 0x00, 0x11,
            0x81, 0xa6, 0x00, 0x00, 0x0b, 0x01, 0x00, 0x00, 0x50, 0x00, 0x18, 0x00, 0x50, 0x00, 0x18
        ];
    }
}
