using Services.Translators;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Services.Models;
using Util;

namespace Tests.Translators
{
    public class Poco3270TranslatorTests
    {
        [Test]
        public void FieldTest_NoEnhandedAttributes()
        {
            var data = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc2, 0xd9, 0xd6, 0xe6, 0xe2, 0xc5, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x00, 0x40, 0xc4, 0x89, 0xa2, 0x97, 0x93, 0x81, 0xa8, 0x40, 0xa2, 0x96, 0xa4, 0x99, 0x83,
                0x85, 0x40, 0x84, 0x81, 0xa3, 0x81, 0x40, 0xa4, 0xa2, 0x89, 0x95, 0x87, 0x40, 0xd9, 0x85, 0xa5, 0x89,
                0x85, 0xa6, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x00, 0xe3, 0xc5, 0xd9, 0xd4, 0xc9, 0xd5, 0xc1, 0xd3,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            var attrs = new Dictionary<int, IDictionary<byte, byte>>()
            {
                [7] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b11111000
                },
                [20] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b11111000
                },
                [60] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b11111000
                }
            };

            var route66Attributes = new Dictionary<int, IDictionary<string, object>>()
            {
                [7] = new Dictionary<string, object>()
                {
                    [Route66Attributes.ADDRESS] = BinaryUtil.CoordinateAddress((5, 7))
                },
                [20] = new Dictionary<string, object>()
                {
                    [Route66Attributes.ADDRESS] = BinaryUtil.CoordinateAddress((5, 20))
                },
                [60] = new Dictionary<string, object>()
                {
                    [Route66Attributes.ADDRESS] = BinaryUtil.CoordinateAddress((5, 60))
                }
            };

            var expected = new List<FieldData>
            {
                new()
                {
                    Row = 5,
                    Col = 7,
                    Length = "BROWSE      ".Length,
                    Value = "BROWSE      ",
                    IsProtected = true,
                    Address = 407
                },
                new()
                {
                    Row = 5,
                    Col = 20,
                    Length = " Display source data using Review      ".Length,
                    Value = " Display source data using Review      ",
                    IsProtected = true,
                    Address = 420
                },
                new()
                {
                    Row = 5,
                    Col = 60,
                    Length = "TERMINAL".Length,
                    Value = "TERMINAL",
                    IsProtected = true,
                    Address = 460
                },
            };

            var actual = new Poco3270Translator()
            {
                Row = 5
            }.Translate(data, attrs, route66Attributes);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InputTest01()
        {
            var data = new byte[]
            {
                0xd3, 0x96, 0x87, 0x96, 0x95, 0X40, 0x7e, 0x7e, 0x7e, 0x6e, 0x00, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40,
                0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x40,
                0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x40,
                0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x40
            };

            var attrs = new Dictionary<int, IDictionary<byte, byte>>()
            {
                [0] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b11111000,
                },
                [11] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b11001000,
                }
            };


            var route66Attributes = new Dictionary<int, IDictionary<string, object>>()
            {
                [0] = new Dictionary<string, object>()
                {
                    [Route66Attributes.ADDRESS] = BinaryUtil.CoordinateAddress((7, 0))
                },
                [11] = new Dictionary<string, object>()
                {
                    [Route66Attributes.ADDRESS] = BinaryUtil.CoordinateAddress((7, 11))
                }
            };

            var expected = new List<FieldData>
            {
                new()
                {
                    Row = 7,
                    Col = 0,
                    Length = "Logon ===>".Length,
                    Value = "Logon ===>",
                    IsProtected = true,
                    Address = 560
                },
                new()
                {
                    Row = 7,
                    Col = 11,
                    Length = "                                                                    ".Length,
                    Value = "                                                                    ",
                    IsProtected = false,
                    Address = 571
                },
            };

            var actual = new Poco3270Translator()
            {
                Row = 7
            }.Translate(data, attrs, route66Attributes);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RowTest01()
        {
            var data = new byte[]
            {
                0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
                0x40, 0x40, 0x40, 0x40, 0x40, 0x00, 0xd9, 0xe4, 0xd5,
                0xd5, 0xc9, 0xd5, 0xc7, 0x40, 0x40, 0xe3, 0xd2, 0xf5,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            var attrs = new Dictionary<int, IDictionary<byte, byte>>()
            {
                [60] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b11111000,
                },
                [76] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b11111000,
                }
            };

            var route66Attributes = new Dictionary<int, IDictionary<string, object>>()
            {
                [1] = new Dictionary<string, object>()
                {
                    [Route66Attributes.ADDRESS] = 18225
                },
                [60] = new Dictionary<string, object>()
                {
                    [Route66Attributes.ADDRESS] = BinaryUtil.CoordinateAddress((23, 60))
                },
                [76] = new Dictionary<string, object>()
                {
                    [Route66Attributes.ADDRESS] = BinaryUtil.CoordinateAddress((23, 76))
                }
            };

            var expected = new List<FieldData>
            {
                new()
                {
                    Row = 23,
                    Col = 0,
                    Length = 59,
                    Value = string.Concat(Enumerable.Repeat(" ", 59)),
                    IsProtected = true,
                },
                new()
                {
                    Row = 23,
                    Col = 60,
                    Length = "RUNNING  TK5".Length,
                    Value = "RUNNING  TK5",
                    IsProtected = true,
                    Address = 1900
                },
                new()
                {
                    Row = 23,
                    Col = 76,
                    Length = 1,
                    Value = " ",
                    IsProtected = true,
                    Address = 1916
                },
            };

            var actual = new Poco3270Translator()
            {
                Row = 23
            }.Translate(data, attrs, route66Attributes);
            Assert.AreEqual(expected, actual);
        }
    }
}
