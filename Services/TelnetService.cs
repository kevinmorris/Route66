using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Util;

// ReSharper disable InconsistentNaming

namespace Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="translator"></param>
    /// <see cref="https://datatracker.ietf.org/doc/html/rfc1576"/>
    public class TelnetService<T>(I3270Translator<T> translator)
        : NetworkService<T>([
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator),
            new RowHandler<T>(translator)
        ])
    {
        protected TcpClient? Tcp;

        public Task Connect(string address, int port)
        {
            Tcp = new TcpClient(address, port);
            return ProcessStream(Tcp.GetStream());
        }

        public override byte[] ProcessOutbound(byte[] buffer)
        {;
            var inbound = new List<byte>();
            if (buffer.Length == 0)
            {
                return [.. inbound];
            }

            if (buffer[0] == Telnet.IAC)
            {
                inbound.AddRange(ProcessIAC(buffer));
            }
            else if (Commands.ALL.Contains(buffer[0]))
            {
                inbound.AddRange(ProcessTN3270(buffer));
            }

            return [.. inbound];
        }

        private byte[] ProcessIAC(byte[] data)
        {
            var inbound = new List<byte>();
            var agreeVerb = data[1] switch
            {
                Telnet.WILL => Telnet.DO,
                Telnet.WONT => Telnet.DONT,
                Telnet.DO => Telnet.WILL,
                Telnet.DONT => Telnet.WONT,
                _ => (byte)0
            };

            var refuseVerb = data[1] switch
            {
                Telnet.WILL => Telnet.DONT,
                Telnet.WONT => Telnet.DONT,
                Telnet.DO => Telnet.WONT,
                Telnet.DONT => Telnet.WONT,
                _ => (byte)0
            };

            switch (data[2])
            {
                case Telnet.BINARY_TRANSMISSION:
                case Telnet.END_OF_RECORD:
                    inbound.AddRange([Telnet.IAC, agreeVerb, data[2]]);
                    break;
                case Telnet.TERMINAL_TYPE:
                    if (Telnet.SUB_OPTION == data[1])
                    {
                        inbound.AddRange( //We are going to act like an IBM-3279-2-E 
                        [
                            Telnet.IAC, Telnet.SUB_OPTION, Telnet.TERMINAL_TYPE,
                            0x00, 0x49, 0x42, 0x4d, 0x2d, 0x33, 0x32, 0x37, 0x39, 0x2d, 0x32, 0x2d, 0x45,
                            Telnet.IAC, 0xf0
                        ]);
                    }
                    else
                    {
                        inbound.AddRange([Telnet.IAC, agreeVerb, data[2]]);
                    }

                    break;
                default:
                    inbound.AddRange([Telnet.IAC, refuseVerb, data[2]]);
                    break;
            }

            inbound.AddRange(ProcessOutbound(data[3..]));

            return [.. inbound];
        }

        private byte[] ProcessTN3270(byte[] data)
        {
            var i = 0;
            var a = 0;

            return [];
        }

        public (int, int) OrderStartField(Span<byte> data, int i, int a)
        {
            var (row, col) = BinaryUtil.AddressCoordinates(a);
            var handler = Handlers[row];
            i += 1;

            var fieldAttr = data[i];
            i += 1;

            while (Orders.SET_ATTRIBUTE == data[i])
            {
                i += 1;
                handler.SetExtendedAttribute(col, data[i], data[i + 1]);
                i += 2;
            }

            var text = new List<byte> { fieldAttr };
            handler.SetExtendedAttribute(col, Attributes.FIELD, fieldAttr);
            a += 1;

            while (!Orders.ALL.Contains(data[i]))
            {
                text.Add(data[i]);
                i += 1;
                a += 1;
            }

            handler.SetCharacters([.. text], col);

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

        public (int, int) OrderSetBufferAddress(Span<byte> data, int i)
        {
            i += 1;
            var a = BinaryUtil.BufferAddress(data.Slice(i, 2));
            i += 2;

            return (i, a);
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Tcp?.Close();
                Tcp?.Dispose();
            }
        }
    }
}
