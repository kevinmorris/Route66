using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace Services
{
    public abstract class NetworkService<T>()
    {
        public RowHandler<T>[] Handlers { get; init; } = [];

        protected NetworkService(RowHandler<T>[] handlers) : this()
        {
            Handlers = handlers;
        }

        public virtual void ProcessOutbound(StreamWriter writer, byte[] buffer)
        {
        }

        protected async Task ProcessStream(Stream stream)
        {
            await Task.Run(() => Run(stream));
            foreach (var rowHandler in Handlers)
            {
                rowHandler.Update();
            }
        }

        private void Run(Stream stream)
        {
            var writer = new StreamWriter(stream, Encoding.ASCII);
            while (stream.CanRead)
            {
                var buffer = new byte[4 * 1024];
                _ = stream.Read(buffer, 0, buffer.Length);
                ProcessOutbound(writer, buffer);
            }

            stream.Close();
        }
    }
}