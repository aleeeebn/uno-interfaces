using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal static class InicioApi
    {
        private static readonly HttpClient cliente = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(1)
        };

        private static Task inicio;

        public static Task AsegurarInicioAsync()
        {
            if (inicio == null || inicio.IsFaulted || inicio.IsCanceled)
                inicio = IniciarAsync();

            return inicio;
        }

        private static async Task<bool> EstaDisponibleAsync()
        {
            try
            {
                using (var respuesta = await cliente.GetAsync(
                    "http://127.0.0.1:8000/openapi.json"))
                {
                    return respuesta.IsSuccessStatusCode;
                }
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        private static string BuscarCarpetaApi()
        {
            DirectoryInfo carpeta =
                new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (carpeta != null)
            {
                string ruta = Path.Combine(carpeta.FullName, "api");

                if (File.Exists(Path.Combine(ruta, "main.py")) &&
                    File.Exists(Path.Combine(
                        ruta, ".venv", "Scripts", "python.exe")))
                {
                    return ruta;
                }

                carpeta = carpeta.Parent;
            }

            throw new DirectoryNotFoundException(
                "No se encontró la carpeta api con su entorno .venv.");
        }

        private static async Task IniciarAsync()
        {
            if (await EstaDisponibleAsync())
                return;

            string carpetaApi = BuscarCarpetaApi();

            var configuracion = new ProcessStartInfo
            {
                FileName = Path.Combine(
                    carpetaApi, ".venv", "Scripts", "python.exe"),

                Arguments =
                    "-m uvicorn main:app --host 127.0.0.1 --port 8000",

                WorkingDirectory = carpetaApi,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process proceso = Process.Start(configuracion))
            {
                if (proceso == null)
                    throw new InvalidOperationException(
                        "No se pudo iniciar la API.");

                for (int intento = 0; intento < 20; intento++)
                {
                    if (await EstaDisponibleAsync())
                        return;

                    if (proceso.HasExited)
                        throw new InvalidOperationException(
                            "La API se cerró durante el inicio. " +
                            "Comprueba su entorno Python y dependencias.");

                    await Task.Delay(500);
                }

                throw new TimeoutException(
                    "La API está tardando demasiado en responder.");
            }
        }
    }
}