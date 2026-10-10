using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class ApiCliente
    {
        private static readonly HttpClient cliente = new HttpClient();
        private const string BaseUrl = "http://127.0.0.1:8000";

        public static async Task<string> ObtenerJugadores()
        {
            HttpResponseMessage respuesta = await cliente.GetAsync(BaseUrl + "/jugadores");
            return await respuesta.Content.ReadAsStringAsync();
        }
        public static async Task<int> CrearJugador(string nombre)
        {
            HttpResponseMessage respuesta = await cliente.PostAsync(BaseUrl + "/jugadores?nombre=" +Uri.EscapeDataString(nombre), null);
            string json = await respuesta.Content.ReadAsStringAsync();
            return (int)JObject.Parse(json)["id_jugador"];
        }
        public static async Task<int> CrearPartida(int id1, int id2, int id3)
        {
            HttpResponseMessage respuesta = await cliente.PostAsync(BaseUrl + "/partidas?id_jugador1=" + id1 + "&id_jugador2=" + id2 + "&id_jugador3=" + id3, null);
            string json = await respuesta.Content.ReadAsStringAsync();
            return (int)JObject.Parse(json)["id_partida"];
        }

        public static async Task TerminarPartida(int idPartida, int idGanador)
        {
            await cliente.PostAsync(
                BaseUrl + "/partidas/" + idPartida + "/terminar?id_ganador=" + idGanador, null);
        }

        public static async Task<string> ObtenerHistorial()
        {
            HttpResponseMessage respuesta = await cliente.GetAsync(BaseUrl + "/historial");
            return await respuesta.Content.ReadAsStringAsync();
        }
        public static async Task RegistrarMovimiento(int idPartida, int idJugador, string accion, string color = null, int? valor = null)
            {
                string url = BaseUrl + "/movimientos?id_partida=" + idPartida + "&id_jugador=" + idJugador+ "&accion=" + Uri.EscapeDataString(accion);
                if (color != null) url += "&color_carta=" + Uri.EscapeDataString(color);
                if (valor != null) url += "&valor_carta=" + valor;
                await cliente.PostAsync(url, null);
            }
    }
}