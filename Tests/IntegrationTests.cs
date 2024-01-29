using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using Tests.Helpers;

namespace Tests
{
    public class IntegrationTests
    {
        private TelnetService<(byte[], IDictionary<int, IDictionary<byte, byte>>)>? _service;

        [SetUp]
        public void SetUp()
        {
            _service = new TelnetService<(byte[], IDictionary<int, IDictionary<byte, byte>>)>(new ByteArray3270Translator());
        }

        [Test]
        [Ignore("server not running")]
        public async Task? TestConnection()
        {
            await Task.WhenAny(
                (_service?.Connect("127.0.0.1", 3270) ?? Task.CompletedTask),
                Task.Delay(1000));
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }
    }
}
