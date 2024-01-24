using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace Services
{
    public abstract class NetworkService<T>()
    {
        public RowUpdateHandler<T>[]? Handlers { get; init; }

        protected NetworkService(RowUpdateHandler<T>[] handlers) : this()
        {
            Handlers = handlers;
        }

        public virtual void ProcessRead(StreamWriter writer, byte[] buffer)
        {
        }

        protected Task ProcessStream(Stream stream)
        {
            return Task.Run(() => Run(stream));
        }

        private void Run(Stream stream)
        {
            var writer = new StreamWriter(stream, Encoding.ASCII);
            while (stream.CanRead)
            {
                var buffer = new byte[4 * 1024];
                _ = stream.Read(buffer, 0, buffer.Length);
                ProcessRead(writer, buffer);
            }

            stream.Close();
        }
    }
}