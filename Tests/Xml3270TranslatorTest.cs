using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Services;

namespace Tests
{
    public class Xml3270TranslatorTest
    {
        private byte[] _data = new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0xc2, 0xd9, 0xd6, 0xe6, 0xe2, 0xc5, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0xf0, 0x40, 0xc4, 0x89, 0xa2, 0x97, 0x93, 0x81, 0xa8, 0x40, 0xa2, 0x96, 0xa4, 0x99, 0x83,
            0x85, 0x40, 0x84, 0x81, 0xa3, 0x81, 0x40, 0xa4, 0xa2, 0x89, 0x95, 0x87, 0x40, 0xd9, 0x85, 0xa5, 0x89,
            0x85, 0xa6, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0xf0, 0xe3, 0xc5, 0xd9, 0xd4, 0xc9, 0xd5, 0xc1, 0xd3,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        [Test]
        public void FieldTest_NoEnhandedAttributes()
        {
            var attrs = new Dictionary<int, IDictionary<byte, byte>>()
            {
                [6] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b00000000
                },
                [19] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b00000000
                },
                [59] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b00000000
                }
            };

            var expectedStr =
                """
                <row>
                    <label col="7">BROWSE</label>
                    <label col="20">Display source data using Review</label>
                    <label col="60">TERMINAL</label>
                </row>
                """;

            var expected = XElement.Parse(expectedStr);
            var actual = new Xml3270Translator().Translate(_data, attrs);
            Assert.That(actual, Is.EqualTo(expected).Using<XElement>(XNode.DeepEquals));
        }

        [Test]
        public void FieldTest_EnhandedAttributes()
        {
            var attrs = new Dictionary<int, IDictionary<byte, byte>>()
            {
                [6] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b00000000,
                    [Attributes.FOREGROUND_COLOR] = Colors.TURQUOISE
                },
                [19] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b00000000,
                    [Attributes.HIGHLIGHTING] = Highlighting.BLINK
                },
                [59] = new Dictionary<byte, byte>()
                {
                    [Attributes.FIELD] = 0b00000000,
                    [Attributes.FOREGROUND_COLOR] = Colors.GREEN,
                    [Attributes.HIGHLIGHTING] = Highlighting.UNDERSCORE
                }
            };

            var expectedStr =
                """
                <row>
                    <label col="7" foreground-color="turquoise">BROWSE</label>
                    <label col="20" highlighting="blink">Display source data using Review</label>
                    <label col="60" foreground-color="green" highlighting="underscore">TERMINAL</label>
                </row>
                """;

            var expected = XElement.Parse(expectedStr);
            var actual = new Xml3270Translator().Translate(_data, attrs);
            Assert.That(actual, Is.EqualTo(expected).Using<XElement>(XNode.DeepEquals));
        }
    }
}
