using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using Services.Models;
using Util;
// ReSharper disable InconsistentNaming

namespace Services
{
    /// <summary>
    /// Implements IBM's TN3270 protocol.  An instance of this class wraps
    /// a TcpClient in order to communicate with a TN3270 service.
    /// </summary>
    /// <typeparam name="T">The type of output from each of the row translators</typeparam>
    /// <param name="translator">the translator that converts the TN3270 byte stream to <code>T</code></param>
    /// <see href="https://www.rfc-editor.org/rfc/rfc1576"></see>
    /// <see href="http://bitsavers.trailing-edge.com/pdf/ibm/3270/GA23-0059-4_3270_Data_Stream_Programmers_Reference_Dec88.pdf"></see>
    /// 
    public class TN3270Service<T>
    {
        protected TcpClient? Tcp;

        protected I3270Translator<T> _translator;

        /// <summary>
        /// Commands control such things as whether the application program writes to or
        /// reads from a display and whether the screen is erased before new data is written.
        /// </summary>
        public byte ActiveCommand { get; set; }= 0x00;

        /// <summary>
        /// The collection of row handlers with each handler corresponding to
        /// a single row of a terminal display.
        /// </summary>
        public RowHandler<T>[] Handlers { get; init; }

        public Stream? Stream { get; set; }
        internal byte Aid { get; private set; } = AID.NO_AID;

        public TN3270Service(I3270Translator<T> translator, string address, int port)
        {
            _translator = translator;
            Connect(address, port);
            Handlers =
            [
                new RowHandler<T>(0, _translator),
                new RowHandler<T>(1, _translator),
                new RowHandler<T>(2, _translator),
                new RowHandler<T>(3, _translator),
                new RowHandler<T>(4, _translator),
                new RowHandler<T>(5, _translator),
                new RowHandler<T>(6, _translator),
                new RowHandler<T>(7, _translator),
                new RowHandler<T>(8, _translator),
                new RowHandler<T>(9, _translator),
                new RowHandler<T>(10, _translator),
                new RowHandler<T>(11, _translator),
                new RowHandler<T>(12, _translator),
                new RowHandler<T>(13, _translator),
                new RowHandler<T>(14, _translator),
                new RowHandler<T>(15, _translator),
                new RowHandler<T>(16, _translator),
                new RowHandler<T>(17, _translator),
                new RowHandler<T>(18, _translator),
                new RowHandler<T>(19, _translator),
                new RowHandler<T>(20, _translator),
                new RowHandler<T>(21, _translator),
                new RowHandler<T>(22, _translator),
                new RowHandler<T>(23, _translator),
            ];
        }

        /// <summary>
        /// Connects this instance to the remote 3270 service
        /// </summary>
        /// <param name="address">the address of the remote service</param>
        /// <param name="port">the tcp port</param>
        public void Connect(string address, int port)
        {
            Tcp = new TcpClient(address, port);
            Task.Run(() => Run(Tcp.GetStream()));
        }

        /// <summary>
        /// Processes the stream of bytes coming from the TN3270 service at the root
        /// of the parse tree for the TN3270 data format.
        /// </summary>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        public virtual (int, int) ProcessOutbound(Span<byte> data, int i, int a)
        {
            while (i < data.Length)
            {
                var d = data[i];
                if (d == Telnet.IAC)
                {
                    (i, a) = ProcessIAC(data, i, a);
                    ActiveCommand = 0x00;
                }
                else if (ActiveCommand == 0x00 && Commands.ALL.Contains(d))
                {
                    (i, a) = ProcessCommand(data, i, a);
                }
                else if (new[] {Commands.ERASE_WRITE, Commands.WRITE }.Contains(ActiveCommand))
                {
                    (i, a) = ProcessOrder(data, i, a);
                }
                else if (ActiveCommand == Commands.WRITE_STRUCTURED_FIELD)
                {
                    i = ProcessWriteStructuredField(data, i);
                }
                else
                {
                    ActiveCommand = 0x00;
                    i = data.Length;
                }
            }

            return (i, a);
        }

        /// <summary>
        /// Processes a telnet command in accordance with RFC1576
        /// </summary>
        /// <see href="https://www.rfc-editor.org/rfc/rfc1576"></see>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the  new character display address</returns>
        /// 
        private (int, int) ProcessIAC(Span<byte> data, int i, int a)
        {
            if (i >= data.Length - 1 || data[i + 1] == Telnet.END_OF_RECORD)
            {
                return (i+2, a);
            }

            var agreeVerb = data[i + 1] switch
            {
                Telnet.WILL => Telnet.DO,
                Telnet.WONT => Telnet.DONT,
                Telnet.DO => Telnet.WILL,
                Telnet.DONT => Telnet.WONT,
                _ => (byte)0 
            };

            var refuseVerb = data[i + 1] switch
            {
                Telnet.WILL => Telnet.DONT,
                Telnet.WONT => Telnet.DONT,
                Telnet.DO => Telnet.WONT,
                Telnet.DONT => Telnet.WONT,
                _ => (byte)0
            };

            switch (data[i + 2])
            {
                case Telnet.BINARY_TRANSMISSION_ABILITY:
                case Telnet.END_OF_RECORD_ABILITY:
                    Write([Telnet.IAC, agreeVerb, data[i + 2]]).Wait();
                    break;
                case Telnet.TERMINAL_TYPE_ABILITY:
                    if (Telnet.SUB_OPTION == data[1])
                    {
                        //We are going to act like an IBM-3279-2-E 
                        Write(Telnet.IBM32792E).Wait();
                    }
                    else
                    {
                        Write([Telnet.IAC, agreeVerb, data[i + 2]]).Wait();
                    }

                    break;
                default:
                    Write([Telnet.IAC, refuseVerb, data[i + 2]]).Wait();
                    break;
            }

            (i, a) = ProcessOutbound(data, i + 3, a);

            return (i, a);
        }

        protected void Run(NetworkStream stream)
        {
            Stream = stream;

            using (var s = Stream)
            {
                while (Stream?.CanRead ?? false)
                {
                    try
                    {
                        //The 3270 data stream uses the term "outbound" for data coming
                        //from the server i.e. "outbound from the server".
                        //It uses the term 'inbound' for data leaving the client
                        //i.e. "inbound to the server"
                        var outbound = new byte[4 * 1024];

                        _ = Stream.Read(outbound, 0, outbound.Length);
                        _ = ProcessOutbound(outbound, 0, 0);

                        Update();
                    }
                    catch (ObjectDisposedException ode)
                    {
                    }
                }

                Stream?.Close();
            }

            Stream = null;
        }

        public void Update(bool force = false)
        {
            foreach (var rowHandler in Handlers)
            {
                rowHandler.Update(force);
            }
        }

        /// <summary>
        /// Processes a TN3270 Command.
        /// Commands are sent to a display to initiate the total or partial writing, reading, or
        /// erasing of data in a selected character buffer.
        /// </summary>
        /// <see href="http://bitsavers.trailing-edge.com/pdf/ibm/3270/GA23-0059-4_3270_Data_Stream_Programmers_Reference_Dec88.pdf"></see>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        protected (int, int) ProcessCommand(Span<byte> data, int i, int a)
        {
            switch (data[i])
            {
                case Commands.ERASE_WRITE:
                    Aid = AID.NO_AID;
                    ActiveCommand = Commands.ERASE_WRITE;
                    foreach (var handler in Handlers)
                    {
                        handler.Reset();
                    }

                    return (i + 2, a);

                case Commands.WRITE:
                    ActiveCommand = Commands.WRITE;
                    return (i + 2, a);

                case Commands.WRITE_STRUCTURED_FIELD:
                    ActiveCommand = Commands.WRITE_STRUCTURED_FIELD;
                    return (i + 1, a);

                    
                default: return (i, a);
            }
        }

        #region Inbound

        /// <summary>
        /// Sends an AID to the TN3270 service. An AID, which is always the first byte of an inbound data stream, describes the
        /// action that caused the inbound data stream to be transmitted.
        /// </summary>
        /// <param name="aid">the aid command</param>
        /// <returns>an asynchronous Task</returns>
        public async Task SendKeyAsync(byte aid)
        {
            byte[] aidArray = [aid];
            await WriteAndEndRecord(aidArray);
        }

        /// <summary>
        /// Sends an AID to the TN3270 service along with cursor coordiantes.
        /// An AID, which is always the first byte of an inbound data stream, describes the
        /// action that caused the inbound data stream to be transmitted.
        /// </summary>
        /// <param name="aid">the aid command</param>
        /// <param name="cursorRow">the current row location of the cursor</param>
        /// <param name="cursorCol">the current column location of the cursor</param>
        /// <returns>an asynchronous Task</returns>
        public async Task SendKeyAsync(byte aid, int cursorRow, int cursorCol)
        {
            var address12Bit = BinaryUtil.AddressBuffer12Bit(
                BinaryUtil.CoordinateAddress((cursorRow, cursorCol)));

            byte[] aidArray = [aid];

            await WriteAndEndRecord([.. aidArray, .. address12Bit]);
        }

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
        public async Task SendFieldsAsync(byte aid, int cursorRow, int cursorCol, IEnumerable<FieldData> fieldData)
        {
            var cursorAddress = BinaryUtil.AddressBuffer12Bit(
                BinaryUtil.CoordinateAddress((cursorRow, cursorCol)));
            
            var fieldBytes = fieldData.SelectMany<FieldData, byte>(f =>
            {
                var fieldAddress = BinaryUtil.AddressBuffer12Bit(f.Address);
                var fieldTextBytes = f.Value.Select(c => EBCDIC.EBCDICBytes[c]);

                return
                [
                    Orders.SET_BUFFER_ADDRESS,
                    ..fieldAddress,
                    ..fieldTextBytes
                ];
            });

            await WriteAndEndRecord([aid, ..cursorAddress, ..fieldBytes]);
        }

        protected async Task Write(byte[] buffer)
        {
            if (Stream != null)
            {
                await Stream.WriteAsync(buffer, CancellationToken.None);
            }
        }

        protected async Task WriteAndEndRecord(byte[] buffer)
        {
            byte[] endOfRecord = [Telnet.IAC, Telnet.END_OF_RECORD];
            await Write([.. buffer, .. endOfRecord]);
        }

        #endregion

        #region Orders

        /// <summary>
        /// Processes a TN3270 Order. Orders are included in the inbound and outbound data streams to provide
        /// additional control function.
        /// </summary>
        /// <see href="http://bitsavers.trailing-edge.com/pdf/ibm/3270/GA23-0059-4_3270_Data_Stream_Programmers_Reference_Dec88.pdf"></see>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        public (int, int) ProcessOrder(Span<byte> data, int i, int a)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            var d = data[i];

            if (d == Orders.START_FIELD)
            {
                (i, a) = OrderStartField(data, i, a);
            }
            else if (d == Orders.SET_BUFFER_ADDRESS)
            {
                (i, a) = OrderSetBufferAddress(data, i);
            }
            else if (d == Orders.MODIFY_FIELD)
            {
            }
            else if (d == Orders.INSERT_CURSOR)
            {
                (i, a) = OrderInsertCursor(data, i, a);
            }
            else if (d == Orders.REPEAT_TO_ADDRESS)
            {
                (i, a) = OrderRepeatToAddress(data, i, a);
            }
            else if (d == Orders.SET_ATTRIBUTE)
            {
                (i, a) = OrderSetAttribute(data, i, a);
            }
            else if (EBCDIC.Chars.ContainsKey(d) || d == 0)
            {
                (i, a) = AddCharacter(data, i, a);
            }
            else
            {
                ActiveCommand = 0x00;
                i = data.Length;
            }

            return (i, a);
        }

        /// <summary>
        /// Adds a single EBCDIC character to a display row
        /// </summary>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        public (int, int) AddCharacter(Span<byte> data, int i, int a)
        {
            var (row, col) = BinaryUtil.AddressCoordinates(a);
            Handlers[row].SetCharacter(col, data[i]);
            return (i + 1, a + 1);
        }

        /// <summary>
        /// The Start Field (SF) order indicates the start of a field.
        /// In a write data stream, this order identifies to the display that the next byte is a
        /// field attribute.  The byte following the SF order in the write data stream is always treated as a field
        /// attribute.
        /// </summary>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        public (int, int) OrderStartField(Span<byte> data, int i, int a)
        {
            a += 1;

            var (row, col) = BinaryUtil.AddressCoordinates(a);
            row = row % Handlers.Length;

            i += 1;
            var fieldAttr = data[i];
            i += 1;

            Handlers[row].SetExtendedAttribute(col, Attributes.FIELD, fieldAttr);
            Handlers[row].SetRoute66Attribute(col, Route66Attributes.ADDRESS, a);

            return (i, a);
        }

        /// <summary>
        /// The Insert Cursor (IC) order repositions the cursor to the location specified by the current buffer
        /// address.Execution of this order does not change the current buffer address.
        /// </summary>
        /// <param name="_">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        public (int, int) OrderInsertCursor(Span<byte> _, int i, int a)
        {
            var (row, col) = BinaryUtil.AddressCoordinates(a);
            Handlers[row].SetCursor(col);

            return (i + 1, a);
        }

        /// <summary>
        /// The Repeat to Address (RA) order stores a specified character in all character buffer locations, starting
        /// at the current buffer address and ending at (but not including) the specified stop address.
        /// </summary>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        public (int, int) OrderRepeatToAddress(Span<byte> data, int i, int a)
        {
            i += 1;
            var stopA = BinaryUtil.BufferAddress(data.Slice(i, 2));
            i += 2;

            for (var j = a; j < stopA; j++)
            {
                var (row, col) = BinaryUtil.AddressCoordinates(j);
                row %= Handlers.Length;

                Handlers[row].SetCharacter(col, data[i]);
            }

            return (i, stopA);
        }

        /// <summary>
        /// The Set Buffer Address (SBA) order specifies a new character buffer address from which operations are
        /// to start or continue.  In a write data stream, Set Buffer Address orders can be used to write data into
        /// various areas of the buffer.
        /// </summary>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        public (int, int) OrderSetBufferAddress(Span<byte> data, int i)
        {
            i += 1;
            var a = BinaryUtil.BufferAddress(data.Slice(i, 2));
            i += 2;

            return (i, a);
        }

        /// <summary>
        /// The Set Attribute (SA) order is used to specify :J. character's attribute type and its value so that
        /// subsequently interpreted characters in the data stream apply the character
        /// properties defined by the type/value pair.
        /// </summary>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <param name="a">the current character display address within the character buffer</param>
        /// <returns>a tuple containing the new pointer within the stream and the new character display address</returns>
        public (int, int) OrderSetAttribute(Span<byte> data, int i, int a)
        {
            var (row, col) = BinaryUtil.AddressCoordinates(a);

            i += 1;
            Handlers[row].SetExtendedAttribute(col, data[i], data[i + 1]);
            i += 2;

            return (i, a);
        }
        #endregion

        #region WriteStructuredField

        /// <summary>
        /// The Write Structured Field (WSF) command is used to send structured fields from the application program
        /// to the display.
        /// </summary>
        /// <param name="data">the data stream</param>
        /// <param name="i">the current pointer within the stream</param>
        /// <returns></returns>
        public int ProcessWriteStructuredField(Span<byte> data, int i)
        {
            var length = (data[i] << 8) + data[i + 1];

            if (data[i + 2] == StructuredFields.READ_PARTITION)
            {
                if (data[i + 5] == StructuredFields.READ_PARTITION_QUERY)
                {
                    WriteAndEndRecord(StructuredFields.READ_PARTITION_REPLY).Wait();
                }
            }

            ActiveCommand = 0x00;
            return i + length + 1;
        }

        #endregion
        public void Dispose()
        {
            Stream?.Close();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
        }
    }
}