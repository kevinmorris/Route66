using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using Util;

namespace Services
{
    public abstract class NetworkService<T>() : IDisposable
    {
        public RowHandler<T>[] Handlers { get; init; } = [];

        protected NetworkService(RowHandler<T>[] handlers) : this()
        {
            Handlers = handlers;
        }

        public abstract Task Connect(string address, int port);

        public virtual byte[] ProcessOutbound(Span<byte> buffer)
        {
            return [];
        }

        protected async Task ProcessStream(Stream stream)
        {
            await Task.Run(() => Run(stream));
        }

        private void Run(Stream stream)
        {
            var writer = new StreamWriter(stream, Encoding.ASCII);
            while (stream.CanRead)
            {
                //The 3270 data stream uses the term "outbound" for data coming
                //from the server i.e. "outbound from the server".
                //It uses the term 'inbound' for data leaving the client
                //i.e. "inbound to the server"
                var outbound = new byte[4 * 1024];
                _ = stream.Read(outbound, 0, outbound.Length);
                var inbound = ProcessOutbound(outbound);
                foreach (var rowHandler in Handlers)
                {
                    rowHandler.Update();
                }

                if (inbound.Length > 0)
                {
                    stream.Write(inbound, 0, inbound.Length);
                }
            }

            stream.Close();
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

            while (i < data.Length && !Orders.ALL.Contains(data[i]) && data[i] != Telnet.IAC)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
        }
    }
}