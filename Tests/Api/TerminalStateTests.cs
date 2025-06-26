using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.State;
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
            var expectedFieldData = new List<FieldData>()
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

            var mockHandler = new Mock<IGridHandler<IEnumerable<FieldData>>>();
            var mockService = new Mock<ITN3270Service<IEnumerable<FieldData>>>();

            var customHandlerCalled = false;
            void CustomHandler(object? sender, FieldsChangedEventArgs args)
            {
                customHandlerCalled = true;
            }

            mockService
                .Setup(s => s.Handler)
                .Returns(mockHandler.Object);

            mockService
                .Setup(s => s.Connect(It.IsAny<string>(), It.IsAny<int>()));
            
            var terminalState = new TerminalState(mockService.Object, "", 0, CustomHandler);
            Assert.False(terminalState.NewDataAvailable);
            
            mockHandler.Raise(h => h.GridUpdated += null,
                new GridUpdateEventArgs<IEnumerable<FieldData>>()
                {
                    Data = expectedFieldData
                });
            
            Assert.True(terminalState.NewDataAvailable);
            
            var actual = terminalState.FieldData;
            Assert.AreEqual(expectedFieldData, actual[0]);
            
            Assert.False(terminalState.NewDataAvailable);
            Assert.True(customHandlerCalled);
        }
    }
}
