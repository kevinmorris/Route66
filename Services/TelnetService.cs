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
        protected TcpClient Tcp;

        public Task Connect(string address, int port)
        {
            Tcp = new TcpClient(address, port);
            return ProcessStream(Tcp.GetStream());
        }

        public override void ProcessOutbound(StreamWriter writer, byte[] buffer)
        {
            switch (buffer[0])
            {
                case Telnet.IAC: writer.Write(ProcessIAC(buffer));
                    break;
                //case Telnet.TN3270: writer.Write(ProcessTN3270(buffer));
                //    break;
            }
        }

        private static byte[] ProcessIAC(byte[] buffer)
        {
            byte responseVerb = 0;

            switch (buffer[2])
            {
                case Telnet.BINARY_TRANSMISSION:
                case Telnet.END_OF_RECORD:
                    responseVerb = buffer[1] switch
                    {
                        Telnet.WILL => Telnet.DO,
                        Telnet.WONT => Telnet.DONT,
                        Telnet.DO => Telnet.WILL,
                        Telnet.DONT => Telnet.WONT,
                        _ => (byte)0
                    };
                    break;
                case Telnet.TERMINAL_TYPE:
                    responseVerb = buffer[1] switch
                    {
                        Telnet.WILL => Telnet.DONT,
                        Telnet.WONT => Telnet.DONT,
                        Telnet.DO => Telnet.WONT,
                        Telnet.DONT => Telnet.WONT,
                        _ => (byte)0
                    };
                    break;
            }

            return [Telnet.IAC, responseVerb, buffer[2]];
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
    }
}
