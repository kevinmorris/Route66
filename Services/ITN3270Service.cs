using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITN3270Service<T>
    {
        /// <summary>
        /// The collection of row handlers with each handler corresponding to
        /// a single row of a terminal display.
        /// </summary>
        public IGridHandler<T> Handler { get; init; }

        /// <summary>
        /// Connects this instance to the remote 3270 service
        /// </summary>
        /// <param name="address">the address of the remote service</param>
        /// <param name="port">the tcp port</param>
        public void Connect(string address, int port);

        /// <summary>
        /// Processes the stream of bytes coming from the TN3270 service at the root
        /// of the parse tree for the TN3270 data format.
        /// </summary>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        public (int, int) ProcessOutbound(Span<byte> data, int i, int a);

        /// <summary>
        /// Sends an AID to the TN3270 service. An AID, which is always the first byte of an inbound data stream, describes the
        /// action that caused the inbound data stream to be transmitted.
        /// </summary>
        /// <param name="aid">the aid command</param>
        /// <returns>an asynchronous Task</returns>
        public Task SendKeyAsync(byte aid);


        /// <summary>
        /// Sends an AID to the TN3270 service along with cursor coordiantes.
        /// An AID, which is always the first byte of an inbound data stream, describes the
        /// action that caused the inbound data stream to be transmitted.
        /// </summary>
        /// <param name="aid">the aid command</param>
        /// <param name="cursorRow">the current row location of the cursor</param>
        /// <param name="cursorCol">the current column location of the cursor</param>
        /// <returns>an asynchronous Task</returns>
        public Task SendKeyAsync(byte aid, int cursorRow, int cursorCol);

        /// <summary>
        /// Sends an AID to the TN3270 service along with cursor coordiantes and unprotected
        /// field data.  An AID, which is always the first byte of an inbound data stream, describes the
        /// action that caused the inbound data stream to be transmitted.
        /// </summary>
        /// <param name="aid">the aid command</param>
        /// <param name="cursorRow">the current row location of the cursor</param>
        /// <param name="cursorCol">the current column location of the cursor</param>
        /// <param name="fieldData">unprotected field data to be sent to the TN3270 service</param>
        /// <returns>an asynchronous Task</returns>
        public Task SendFieldsAsync(byte aid, int cursorRow, int cursorCol, IEnumerable<FieldData> fieldData);

        public void Update(bool force = false);

    }
}
