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
        private NetworkService<(byte[], IDictionary<int, IDictionary<byte, byte>>)>? _service;

        [SetUp]
        public void SetUp()
        {
            _service = new NetworkService<(byte[], IDictionary<int, IDictionary<byte, byte>>)>(new ByteArray3270Translator());
        }



        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }
    }
}
