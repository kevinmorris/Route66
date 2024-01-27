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
        {
            return buffer[0] switch
            {
                Telnet.IAC => ProcessIAC(buffer),
                _ => []
            };
        }

        private static byte[] ProcessIAC(byte[] buffer)
        {
            var agreeVerb = buffer[1] switch
            {
                Telnet.WILL => Telnet.DO,
                Telnet.WONT => Telnet.DONT,
                Telnet.DO => Telnet.WILL,
                Telnet.DONT => Telnet.WONT,
                _ => (byte)0
            };

            var refuseVerb = buffer[1] switch
            {
                Telnet.WILL => Telnet.DONT,
                Telnet.WONT => Telnet.DONT,
                Telnet.DO => Telnet.WONT,
                Telnet.DONT => Telnet.WONT,
                _ => (byte)0
            };

            switch (buffer[2])
            {
                case Telnet.BINARY_TRANSMISSION:
                case Telnet.END_OF_RECORD:
                    return [Telnet.IAC, agreeVerb, buffer[2]];
                case Telnet.TERMINAL_TYPE:
                    if (Telnet.SUB_OPTION == buffer[1])
                    {
                        return //We are going to act like an IBM-3279-2-E 
                        [
                            Telnet.IAC, Telnet.SUB_OPTION, Telnet.TERMINAL_TYPE,
                            0x00, 0x49, 0x42, 0x4d, 0x2d, 0x33, 0x32, 0x37, 0x39, 0x2d, 0x32, 0x2d, 0x45,
                            Telnet.IAC, 0xf0
                        ];
                    }
                    else
                    {
                        return [Telnet.IAC, agreeVerb, buffer[2]];
                    }
                default:
                    return [Telnet.IAC, refuseVerb, buffer[2]];
            }
        }

        private static void ProcessTN3270(byte[] buffer)
        {
            var i = 0;
        }

        public int OrderSetBufferAddress(Span<byte> buffer, int i)
        {
            i += 1;

            var (row, col) = BinaryUtil.BufferAddressYX(buffer.Slice(i, 2));
            var rowHandler = Handlers[row];

            i += 2;

            return i;
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
