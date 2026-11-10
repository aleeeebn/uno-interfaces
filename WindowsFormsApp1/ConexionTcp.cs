using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal sealed class ConexionTcp : IDisposable
    {
        private readonly TcpClient socket;
        private readonly StreamReader lector;
        private readonly StreamWriter escritor;
        private readonly SemaphoreSlim bloqueoEnvio = new SemaphoreSlim(1, 1);
        private int cerrada;
        public ConexionTcp(TcpClient socket)
        {
            this.socket = socket;
            socket.NoDelay = true;

            NetworkStream stream = socket.GetStream();

            lector = new StreamReader(
                stream, new UTF8Encoding(false), false, 4096, true);

            escritor = new StreamWriter(
                stream, new UTF8Encoding(false), 4096, true);

            escritor.AutoFlush = true;
        }

        public async Task EnviarAsync(string mensaje)
        {
            if (mensaje == null)
                throw new ArgumentNullException(nameof(mensaje));

            if (mensaje.Contains("\n") || mensaje.Contains("\r"))
                throw new ArgumentException(
                    "El mensaje debe ocupar una sola línea.");

            await bloqueoEnvio.WaitAsync().ConfigureAwait(false);

            try
            {
                if (Volatile.Read(ref cerrada) != 0)
                    throw new IOException("La conexión está cerrada.");

                await escritor.WriteLineAsync(mensaje).ConfigureAwait(false);
            }
            finally
            {
                bloqueoEnvio.Release();
            }
        }

        public async Task RecibirAsync(Action<string> alRecibir)
        {
            try
            {
                while (Volatile.Read(ref cerrada) == 0)
                {
                    string mensaje = await lector.ReadLineAsync().ConfigureAwait(false);

                    if (mensaje == null)
                        break;

                    alRecibir(mensaje);
                }
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref cerrada, 1) != 0)
                return;

            socket.Close();
        }
    }
}