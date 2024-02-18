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
        public byte ActiveCommand { get; set; }= 0x00;

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

        public Stream? Stream { get; set; }
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

                        foreach (var rowHandler in Handlers)
                        {
                            rowHandler.Update();
                        }
                    }
                    catch (ObjectDisposedException ode)
                    {
                    }
                }

                Stream?.Close();
            }

            Stream = null;
        }

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
        
        public async Task SendFieldsAsync(byte aid, int cursorRow, int cursorCol, IEnumerable<FieldData> fieldData)
        {
            var cursorAddress = BinaryUtil.AddressBuffer12Bit(
                BinaryUtil.CoordinateAddress((cursorRow, cursorCol)));
            
            var fieldBytes = fieldData.SelectMany<FieldData, byte>(f =>
            {
                var fieldAddress = BinaryUtil.AddressBuffer12Bit(
                    BinaryUtil.CoordinateAddress((f.Row, f.Col)));

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

        public (int, int) AddCharacter(Span<byte> data, int i, int a)
        {
            var (row, col) = BinaryUtil.AddressCoordinates(a);
            Handlers[row].SetCharacter(col, data[i]);
            return (i + 1, a + 1);
        }
        
        public (int, int) OrderStartField(Span<byte> data, int i, int a)
        {
            var (row, col) = BinaryUtil.AddressCoordinates(a + 1);
            row = row % Handlers.Length;

            i += 1;
            var fieldAttr = data[i];
            i += 1;

            Handlers[row].SetExtendedAttribute(col, Attributes.FIELD, fieldAttr);
            a += 1;

            return (i, a);
        }

        public (int, int) OrderInsertCursor(Span<byte> _, int i, int a)
        {
            var (row, col) = BinaryUtil.AddressCoordinates(a);
            Handlers[row].SetCursor(col);

            return (i + 1, a);
        }

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
        
        public (int, int) OrderSetBufferAddress(Span<byte> data, int i)
        {
            i += 1;
            var a = BinaryUtil.BufferAddress(data.Slice(i, 2));
            i += 2;

            return (i, a);
        }

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