using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

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