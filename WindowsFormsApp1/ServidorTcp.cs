using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal sealed class ServidorTcp : IDisposable
    {
        private readonly ConcurrentDictionary<int, ConexionTcp> clientes =
            new ConcurrentDictionary<int, ConexionTcp>();

        private readonly TcpListener listener;
        private int siguienteId;
        private int iniciado;
        private int detenido;

        public event Action<int> ClienteConectado;
        public event Action<int, string> MensajeRecibido;
        public event Action<int> ClienteDesconectado;
        public event Action<string> Error;

        public ServidorTcp(int puerto)
        {
            listener = new TcpListener(IPAddress.Any, puerto);
        }

        public Task IniciarAsync()
        {
            if (Interlocked.Exchange(ref iniciado, 1) != 0)
                throw new InvalidOperationException("Esta instancia ya fue iniciada.");

            if (Volatile.Read(ref detenido) != 0)
                throw new ObjectDisposedException(nameof(ServidorTcp));

            listener.Start();
            return AceptarAsync();
        }

        private async Task AceptarAsync()
        {
            try
            {
                while (Volatile.Read(ref detenido) == 0)
                {
                    TcpClient socket = await listener
                        .AcceptTcpClientAsync().ConfigureAwait(false);

                    var conexion = new ConexionTcp(socket);
                    int id = Interlocked.Increment(ref siguienteId);

                    clientes[id] = conexion;

                    if (Volatile.Read(ref detenido) != 0)
                    {
                        ConexionTcp retirada;
                        clientes.TryRemove(id, out retirada);
                        conexion.Dispose();
                        break;
                    }
                    _ = AtenderAsync(id, conexion);
                }
            }
            catch (ObjectDisposedException)
                when (Volatile.Read(ref detenido) != 0)
            {
            }
            catch (SocketException)
                when (Volatile.Read(ref detenido) != 0)
            {
            }
            finally
            {
                Dispose();
            }
        }

        private async Task AtenderAsync(int id, ConexionTcp conexion)
        {
            try
            {
                ClienteConectado?.Invoke(id);

                await conexion.RecibirAsync(mensaje => MensajeRecibido?.Invoke(id, mensaje)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Volatile.Read(ref detenido) == 0)
                    Error?.Invoke("Cliente " + id + ": " + ex.Message);
            }
            finally
            {
                ConexionTcp retirada;
                clientes.TryRemove(id, out retirada);
                conexion.Dispose();
                ClienteDesconectado?.Invoke(id);
            }
        }

        public Task EnviarAsync(int id, string mensaje)
        {
            ConexionTcp conexion;

            if (!clientes.TryGetValue(id, out conexion))
                throw new IOException("El cliente ya no está conectado.");

            return conexion.EnviarAsync(mensaje);
        }

        public Task EnviarATodosAsync(string mensaje)
        {
            var tareas = clientes.Values.Select(c => EnviarSeguroAsync(c, mensaje)).ToArray();

            return Task.WhenAll(tareas);
        }

        private async Task EnviarSeguroAsync(
            ConexionTcp conexion, string mensaje)
        {
            try
            {
                await conexion.EnviarAsync(mensaje).ConfigureAwait(false);
            }
            catch (IOException)
            {
                conexion.Dispose();
            }
            catch (SocketException)
            {
                conexion.Dispose();
            }
            catch (ObjectDisposedException)
            {
                conexion.Dispose();
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref detenido, 1) != 0)
                return;

            listener.Stop();

            foreach (ConexionTcp conexion in clientes.Values)
                conexion.Dispose();

            clientes.Clear();
        }
    }
}