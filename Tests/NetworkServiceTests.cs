using Moq.Sequences;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tests.Helpers;

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
    }
}
