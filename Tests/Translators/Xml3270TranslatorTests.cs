using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Services;
using Services.Translators;
using Util;

namespace Tests.Translators
{
    public class Xml3270TranslatorTests
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

            var expectedStr =
                """
                <row>
                    <label row="5" col="7" length="12">BROWSE      </label>
                    <label row="5" col="20" length="39"> Display source data using Review      </label>
                    <label row="5" col="60" length="8">TERMINAL</label>
                </row>
                """;

            var expected = XElement.Parse(expectedStr);
            var actual = new Xml3270Translator()
            {
                Row = 5
            }.Translate(data, attrs, route66Attributes);
            Assert.That(actual, Is.EqualTo(expected).Using<XElement>(XNode.DeepEquals));
        }

        [Test]
        public void InputTest()
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

            var expectedStr =
                """
                <row>
                  <label row="7" col="0" length="10">Logon ===&gt;</label>
                  <input row="7" col="11" address="571" length="68">                                                                    </input>
                </row>
                """;

            var actual = new Xml3270Translator()
            {
                Row = 7
            }.Translate(data, attrs, route66Attributes);
            Assert.AreEqual(expectedStr, actual.ToString());
        }
    }
}
