using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Models;
using Moq;
using Services;
using Services.Models;
using Services.Translators;

namespace Tests.Api
{
    public class TerminalStateTests
    {
        [Test]
        public void NewFieldData()
        {
            var expectedFieldData0 = new List<FieldData>()
            {
                new()
                {
                    Address = Random.Shared.Next(),
                },
                new()
                {
                    Address = Random.Shared.Next(),
                }
            };

            var expectedFieldData1 = new List<FieldData>()
            {
                new()
                {
                    Address = Random.Shared.Next(),
                },
                new()
                {
                    Address = Random.Shared.Next(),
                }
            };

            var mockHandler0 = new Mock<IRowHandler<IEnumerable<FieldData>>>();
            var mockHandler1 = new Mock<IRowHandler<IEnumerable<FieldData>>>();
            var mockService = new Mock<ITN3270Service<IEnumerable<FieldData>>>();

            mockService
                .Setup(s => s.Handlers)
                .Returns([
                    mockHandler0.Object,
                    mockHandler1.Object
                ]);

            mockService
                .Setup(s => s.Connect(It.IsAny<string>(), It.IsAny<int>()));

            var terminalState = new TerminalState(mockService.Object, "", 0);
            Assert.False(terminalState.NewDataAvailable);

            mockHandler0.Raise(h => h.RowUpdated += null,
                new RowUpdateEventArgs<IEnumerable<FieldData>>()
                {
                    Data = expectedFieldData0
                });

            mockHandler1.Raise(h => h.RowUpdated += null,
                new RowUpdateEventArgs<IEnumerable<FieldData>>()
                {
                    Data = expectedFieldData1
                });

            Assert.True(terminalState.NewDataAvailable);

            var actual = terminalState.FieldData;
            Assert.AreEqual(expectedFieldData0, actual[0]);
            Assert.AreEqual(expectedFieldData1, actual[1]);

            Assert.False(terminalState.NewDataAvailable);

        }
    }
}
