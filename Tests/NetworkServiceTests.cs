using Moq.Sequences;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Moq;
using Tests.Helpers;
using System.IO;
using Services.Models;

namespace Tests
{
    public class NetworkServiceTests
    {
        private NetworkService<XElement> _service;

        [SetUp]
        public void SetUp()
        {
            _service = new NetworkService<XElement> (new Xml3270Translator());
        }

        [Test]
        public void StartField()
        {
            var data = new byte[] { 0x1d, 0x60, 0xc8, 0x96, 0xa2, 0xa3 };
            Assert.AreEqual((2, 341), _service.OrderStartField(data, 0, 340));
        }

        [Test]
        public void SetBufferAddress()
        {
            var data = new byte[] { 0x11, 0xc3, 0xe4, 0x1d, 0xf0, 0x28 };
            Assert.AreEqual((3, 228), _service.OrderSetBufferAddress(data, 0));
        }

        [Test]
        public void SendKeyAsync()
        {
            var args = new List<byte[]>();
            var expected = new byte[] { AID.ENTER, Telnet.IAC, Telnet.END_OF_RECORD };
            var mockStream = new Mock<Stream>();
            mockStream.Setup(stream => stream.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), CancellationToken.None))
                .Callback<ReadOnlyMemory<byte>, CancellationToken>((data, token) =>
                {
                    args.Add(data.ToArray());
                });  

            _service.Stream = mockStream.Object;
            _service.SendKeyAsync(AID.ENTER).Wait();

            Assert.AreEqual(1, args.Count);
            Assert.AreEqual(expected, args[0]);
        }

        [Test]
        public void SendKeyAsync_Cursor()
        {
            var args = new List<byte[]>();
            var expected = new byte[] { AID.ENTER, 0xdd, 0x7f, Telnet.IAC, Telnet.END_OF_RECORD };
            var mockStream = new Mock<Stream>();
            mockStream.Setup(stream => stream.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), CancellationToken.None))
                .Callback<ReadOnlyMemory<byte>, CancellationToken>((data, _) =>
                {
                    args.Add(data.ToArray());
                });

            _service.Stream = mockStream.Object;
            _service.SendKeyAsync(AID.ENTER, 23, 79).Wait();

            Assert.AreEqual(1, args.Count);
            Assert.AreEqual(expected, args[0]);
        }

        [Test]
        public void SendFieldsAsync()
        {
            var args = new List<byte[]>();

            var fields = new List<FieldData>()
            {
                new ()
                {
                    Row = 5,
                    Col = 31,
                    Value = "Alpha",
                },

                new ()
                {
                    Row = 19,
                    Col = 56,
                    Value = "Bravo",
                }
            };

            var expected = new byte[] { AID.ENTER, 0xdd, 0x7f, 0x11, 0xc6, 0x6f, 0xc1, 0x93, 0x97, 0x88, 0x81, 0x11, 0xd8, 0x68, 0xc2, 0x99, 0x81, 0xa5, 0x96, Telnet.IAC, Telnet.END_OF_RECORD };
            var mockStream = new Mock<Stream>();
            mockStream.Setup(stream => stream.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), CancellationToken.None))
                .Callback<ReadOnlyMemory<byte>, CancellationToken>((data, _) =>
                {
                    args.Add(data.ToArray());
                });

            _service.Stream = mockStream.Object;
            _service.SendFieldsAsync(AID.ENTER, 23, 79, fields).Wait();

            Assert.AreEqual(1, args.Count);
            Assert.AreEqual(expected, args[0]);
        }
    }
}
