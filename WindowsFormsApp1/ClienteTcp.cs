using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal sealed class ClienteTcp : IDisposable
    {
        private readonly TcpClient socket = new TcpClient();
        private ConexionTcp conexion;
        private int iniciado;
        private int cerrado;

        public event Action<string> MensajeRecibido;
        public event Action Desconectado;
        public event Action<string> Error;

        public async Task ConectarAsync(string ip, int puerto)
        {
            if (Interlocked.Exchange(ref iniciado, 1) != 0)
                throw new InvalidOperationException("Crea un nuevo cliente para volver a conectar.");

            try
            {
                Task conectar = socket.ConnectAsync(ip, puerto);

                Task resultado = await Task.WhenAny(conectar, Task.Delay(10000)).ConfigureAwait(false);

                if (resultado != conectar)
                {
                    socket.Close();

                    try
                    {
                        await conectar.ConfigureAwait(false);
                    }
                    catch
                    {
                    }

                    throw new TimeoutException("El servidor no respondió en 10 segundos.");
                }

                await conectar.ConfigureAwait(false);

                if (Volatile.Read(ref cerrado) != 0)
                    throw new ObjectDisposedException(nameof(ClienteTcp));

                conexion = new ConexionTcp(socket);
                _ = EscucharAsync();
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private async Task EscucharAsync()
        {
            try
            {
                await conexion.RecibirAsync(mensaje => MensajeRecibido?.Invoke(mensaje)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Volatile.Read(ref cerrado) == 0)
                    Error?.Invoke(ex.Message);
            }
            finally
            {
                Dispose();
                Desconectado?.Invoke();
            }
        }

        public Task EnviarAsync(string mensaje)
        {
            if (conexion == null || Volatile.Read(ref cerrado) != 0)
                throw new InvalidOperationException("No hay conexión con el servidor.");

            return conexion.EnviarAsync(mensaje);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref cerrado, 1) != 0)
                return;

            conexion?.Dispose();
            socket.Close();
        }
    }
}