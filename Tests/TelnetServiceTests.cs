using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Services;
using Moq.Protected;
using Moq.Sequences;

namespace Tests
{
    public class TelnetServiceTests
    {
        private TelnetService<byte[]> _service;

        [SetUp]
        public void SetUp()
        {
            _service = new TelnetService<byte[]>(new ByteArray3270Translator());
        }

        [Test]
        public void TestHandshake()
        {
            var mockWriter = new Mock<StreamWriter>([new MemoryStream()]);

            var doTerminalType = new byte[] { 0xff, 0xfd, 0x18 };
            var wontTerminalType = new byte[] { 0xff, 0xfc, 0x18 };
            var willBinaryTransmission = new byte[] { 0xff, 0xfb, 0x00 };
            var doBinaryTransmission = new byte[] { 0xff, 0xfd, 0x00 };
            var willEndOfRecord = new byte[] { 0xff, 0xfb, 0x19 };
            var doEndOfRecord = new byte[] { 0xff, 0xfd, 0x19 };

            using (Sequence.Create())
            {
                mockWriter.Setup(w => w.Write(wontTerminalType)).InSequence(Times.Exactly(1));
                mockWriter.Setup(w => w.Write(doBinaryTransmission)).InSequence(Times.Exactly(1));
                mockWriter.Setup(w => w.Write(willBinaryTransmission)).InSequence(Times.Exactly(1));
                mockWriter.Setup(w => w.Write(doEndOfRecord)).InSequence(Times.Exactly(1));
                mockWriter.Setup(w => w.Write(willEndOfRecord)).InSequence(Times.Exactly(1));

                _service.ProcessRead(mockWriter.Object, doTerminalType);
                _service.ProcessRead(mockWriter.Object, willBinaryTransmission);
                _service.ProcessRead(mockWriter.Object, doBinaryTransmission);
                _service.ProcessRead(mockWriter.Object, willEndOfRecord);
                _service.ProcessRead(mockWriter.Object, doEndOfRecord);
            }
        }
    }
}
