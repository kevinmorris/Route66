using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
// ReSharper disable InconsistentNaming

namespace Services
{
    public class TelnetService<T>(I3270Translator<T> translator)
        : NetworkService<T>([
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator),
            new RowUpdateHandler<T>(translator)
        ])
    {
        private const byte IAC = 0xff;
        private const byte TN3270 = 0xf5;

        private const byte WILL = 0xfb;
        private const byte WONT = 0xfc;
        private const byte DO = 0xfd;
        private const byte DONT = 0xfe;

        private const byte BINARY_TRANSMISSION = 0x00;
        private const byte TERMINAL_TYPE = 0x18;
        private const byte END_OF_RECORD = 0x19;

        protected TcpClient Tcp;

        public Task Connect(string address, int port)
        {
            Tcp = new TcpClient(address, port);
            return ProcessStream(Tcp.GetStream());
        }

        public override void ProcessRead(StreamWriter writer, byte[] buffer)
        {
            switch (buffer[0])
            {
                case IAC: writer.Write(ProcessIAC(buffer));
                    break;
                case TN3270: writer.Write(ProcessTN3270(buffer));
                    break;
            }
        }

        private static byte[] ProcessIAC(byte[] buffer)
        {
            byte responseVerb = 0;

            switch (buffer[2])
            {
                case BINARY_TRANSMISSION:
                case END_OF_RECORD:
                    responseVerb = buffer[1] switch
                    {
                        WILL => DO,
                        WONT => DONT,
                        DO => WILL,
                        DONT => WONT,
                        _ => (byte)0
                    };
                    break;
                case TERMINAL_TYPE:
                    responseVerb = buffer[1] switch
                    {
                        WILL => DONT,
                        WONT => DONT,
                        DO => WONT,
                        DONT => WONT,
                        _ => (byte)0
                    };
                    break;
            }

            return [IAC, responseVerb, buffer[2]];
        }

        private static byte[] ProcessTN3270(byte[] buffer)
        {
            return [];
        }
    }
}
