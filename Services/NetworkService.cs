using System;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using Services.Models;
using Util;
// ReSharper disable InconsistentNaming

namespace Services
{
    public class NetworkService<T>(I3270Translator<T> translator)
    {
        protected TcpClient? Tcp;
        protected List<byte> Inbound = [];

        public RowHandler<T>? CurrentRow { get; private set; }

        public bool CommandReady { get; set; }= false;

        public RowHandler<T>[] Handlers { get; init; } =
        [
            new RowHandler<T>(0, translator),
            new RowHandler<T>(1, translator),
            new RowHandler<T>(2, translator),
            new RowHandler<T>(3, translator),
            new RowHandler<T>(4, translator),
            new RowHandler<T>(5, translator),
            new RowHandler<T>(6, translator),
            new RowHandler<T>(7, translator),
            new RowHandler<T>(8, translator),
            new RowHandler<T>(9, translator),
            new RowHandler<T>(10, translator),
            new RowHandler<T>(11, translator),
            new RowHandler<T>(12, translator),
            new RowHandler<T>(13, translator),
            new RowHandler<T>(14, translator),
            new RowHandler<T>(15, translator),
            new RowHandler<T>(16, translator),
            new RowHandler<T>(17, translator),
            new RowHandler<T>(18, translator),
            new RowHandler<T>(19, translator),
            new RowHandler<T>(20, translator),
            new RowHandler<T>(21, translator),
            new RowHandler<T>(22, translator),
            new RowHandler<T>(23, translator),
        ];

        protected Stream? Stream;
        internal byte Aid { get; private set; } = AID.NO_AID;

        public void Connect(string address, int port)
        {
            Tcp = new TcpClient(address, port);
            Task.Run(() => Run(Tcp.GetStream()));
        }

        public virtual (int, int) ProcessOutbound(Span<byte> data, int i, int a)
        {
            while (i < data.Length)
            {
                //System.Diagnostics.Debug.WriteLine($"XXXXXA128: {i}");
                var d = data[i];
                if (d == Telnet.IAC)
                {
                    (i, a) = ProcessIAC(data, i, a);
                    CommandReady = true;
                }
                else if (CommandReady && Commands.ALL.Contains(d))
                {
                    CommandReady = false;
                    (i, a) = ProcessCommand(data, i, a);
                }
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                else if (d == Orders.START_FIELD)
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
                else if (d == Orders.SET_ATTRIBUTE)
                {
                    (i, a) = OrderSetAttribute(data, i, a);
                }
                else if (EBCDIC.Chars.ContainsKey(d))
                {
                    (i, a) = AddCharacter(data, i, a);
                }
                else
                {
                    i = data.Length;
                }
            }

            return (i, a);
        }

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
                    Inbound.AddRange([Telnet.IAC, agreeVerb, data[i + 2]]);
                    break;
                case Telnet.TERMINAL_TYPE_ABILITY:
                    if (Telnet.SUB_OPTION == data[1])
                    {
                        Inbound.AddRange( //We are going to act like an IBM-3279-2-E 
                        [
                            Telnet.IAC,
                            Telnet.SUB_OPTION,
                            Telnet.TERMINAL_TYPE_ABILITY,
                            0x00,
                            0x49,
                            0x42,
                            0x4d,
                            0x2d,
                            0x33,
                            0x32,
                            0x37,
                            0x39,
                            0x2d,
                            0x32,
                            0x2d,
                            0x45,
                            Telnet.IAC,
                            0xf0
                        ]);
                    }
                    else
                    {
                        Inbound.AddRange([Telnet.IAC, agreeVerb, data[i + 2]]);
                    }

                    break;
                default:
                    Inbound.AddRange([Telnet.IAC, refuseVerb, data[i + 2]]);
                    break;
            }

            (i, a) = ProcessOutbound(data, i + 3, a);

            return (i, a);
        }

        protected void Run(Stream stream)
        {
            Stream = stream;

            while (Stream?.CanRead ?? false)
            {
                try
                {
                    //The 3270 data stream uses the term "outbound" for data coming
                    //from the server i.e. "outbound from the server".
                    //It uses the term 'inbound' for data leaving the client
                    //i.e. "inbound to the server"
                    var outbound = new byte[4 * 1024];

                    Inbound.Clear();

                    _ = Stream.Read(outbound, 0, outbound.Length);
                    _ = ProcessOutbound(outbound, 0, 0);

                    CurrentRow = null;
                    foreach (var rowHandler in Handlers)
                    {
                        rowHandler.Update();
                    }

                    var inbound = Inbound.ToArray();
                    if (inbound.Length > 0)
                    {
                        Stream.Write(inbound, 0, inbound.Length);
                    }
                }
                catch (ObjectDisposedException ode)
                {
                }
            }

            Stream?.Close();

            Stream = null;
        }

        protected (int, int) ProcessCommand(Span<byte> data, int i, int a)
        {
            switch (data[i])
            {
                case Commands.ERASE_WRITE:
                    Aid = AID.NO_AID;
                    foreach (var handler in Handlers)
                    {
                        handler.Reset();
                    }

                    return (i + 2, a);
                    
                default: return (i, a);
            }
        }

        public async Task SendKeyAsync(byte aid)
        {
            byte[] aidArray = [aid];
            await WriteAndEndRecord(aidArray);
        }

        public async Task SendKeyAsync(byte aid, int cursorRow, int cursorCol)
        {
            var address12Bit = BinaryUtil.AddressBuffer12Bit(
                BinaryUtil.CoordinateAddress((cursorRow, cursorCol)));

            byte[] aidArray = [aid];

            await WriteAndEndRecord([.. aidArray, .. address12Bit]);
        }

        public async Task SendFieldAsync(byte aid, int cursorRow, int cursorCol, IEnumerable<FieldData> fieldData)
        {

        }

        protected async Task Write(byte[] buffer)
        {
            if (Stream != null)
            {
                await Stream.WriteAsync(buffer);
            }
        }

        protected async Task WriteAndEndRecord(byte[] buffer)
        {
            byte[] endOfRecord = [Telnet.IAC, Telnet.END_OF_RECORD];
            await Write([.. buffer, .. endOfRecord]);
        }

        #region Orders

        public (int, int) AddCharacter(Span<byte> data, int i, int a)
        {
            CurrentRow?.AddCharacter(data[i]);
            return (i + 1, a + 1);
        }
        
        public (int, int) OrderStartField(Span<byte> data, int i, int a)
        {
            var (row, col) = BinaryUtil.AddressCoordinates(a + 1);

            CurrentRow?.FinalizeField();
            if (row >= Handlers.Length)
            {
                return (i + 2, a + 1);
            }

            CurrentRow = Handlers[row];
            CurrentRow.CurrentCol = col;

            i += 1;

            var fieldAttr = data[i];
            i += 1;

            CurrentRow.SetExtendedAttribute(col, Attributes.FIELD, fieldAttr);
            a += 1;


            return (i, a);
        }

        public (int, int) OrderModifyField(Span<byte> data, int i, int a)
        {
            var (row, col) = BinaryUtil.AddressCoordinates(a);
            var handler = Handlers[row];

            i += 1;
            var attrKeyValuePairCount = (data[i] & 0b00001111);
            i += 1;

            for (var j = 0; j < attrKeyValuePairCount; j++)
            {
                var key = data[i];
                i += 1;
                var value = data[i];
                i += 1;

                if (Attributes.FIELD == key)
                {
                    handler.SetCharacters([value], col);
                }

                handler.SetExtendedAttribute(col, key, value);
            }

            a += 1;
            return (i, a);
        }

        public (int, int) OrderInsertCursor(Span<byte> _, int i, int a)
        {
            if (CurrentRow != null)
            {
                CurrentRow.Cursor = true;
            }

            return (i + 1, a);
        }
        
        public (int, int) OrderSetBufferAddress(Span<byte> data, int i)
        {
            i += 1;
            var a = BinaryUtil.BufferAddress(data.Slice(i, 2));
            var (row, col) = BinaryUtil.AddressCoordinates(a);

            CurrentRow?.FinalizeField();
            CurrentRow = Handlers[row];
            CurrentRow.CurrentCol = col;

            i += 2;

            return (i, a);
        }

        public (int, int) OrderSetAttribute(Span<byte> data, int i, int a)
        {
            var (_, col) = BinaryUtil.AddressCoordinates(a);

            i += 1;
            CurrentRow?.SetExtendedAttribute(col, data[i], data[i + 1]);
            i += 2;

            return (i, a);
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