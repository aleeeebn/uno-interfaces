using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2
    {
        private const int PuertoUno = 5000;

        private ServidorTcp servidorRed;
        private ClienteTcp clienteRed;
        private Label espera;

        private bool anfitrion;
        private bool salaLista;
        private bool cerrandoRed;

        private int miJugador;
        private int versionRed;

        private readonly Dictionary<int, int> conexiones =
            new Dictionary<int, int>();

        private Task colaRed = Task.CompletedTask;

        private void EnUiRed(Action accion)
        {
            if (cerrandoRed || IsDisposed || !IsHandleCreated)
                return;

            try
            {
                BeginInvoke(new Action(() =>
                {
                    if (!cerrandoRed && !IsDisposed)
                        accion();
                }));
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void EncolarRed(Func<Task> envio)
        {
            colaRed = EjecutarEnvioAsync(colaRed, envio);
        }

        private async Task EjecutarEnvioAsync(
            Task anterior, Func<Task> envio)
        {
            try
            {
                await anterior;
                if (!cerrandoRed)
                    await envio();
            }
            catch (Exception ex)
            {
                EnUiRed(() => CerrarSala(
                    "Se perdió la comunicación: " + ex.Message));
            }
        }

        private async Task AbrirRedAsync(
            bool crear, string nombre, string ip)
        {
            anfitrion = crear;
            miJugador = crear ? 0 : -1;

            try
            {
                if (crear)
                {
                    servidorRed = new ServidorTcp(PuertoUno);

                    servidorRed.MensajeRecibido += (id, texto) =>
                        EnUiRed(() => RecibirEnServidor(id, texto));

                    servidorRed.ClienteDesconectado += id =>
                        EnUiRed(() => SalioCliente(id));

                    Task ejecucion = servidorRed.IniciarAsync();
                    _ = VigilarServidorAsync(ejecucion);

                    espera.Text =
                        "Sala creada. Puerto 5000. Esperando jugadores (1/3).";
                }
                else
                {
                    clienteRed = new ClienteTcp();

                    clienteRed.MensajeRecibido += texto =>
                        EnUiRed(() => RecibirEnCliente(texto));

                    clienteRed.Desconectado += () =>
                        EnUiRed(() => CerrarSala(
                            "El anfitrión cerró la conexión."));

                    await clienteRed.ConectarAsync(ip, PuertoUno);

                    espera.Text = "Conectado. Esperando jugadores...";

                    await clienteRed.EnviarAsync(
                        JsonConvert.SerializeObject(new MensajeUno
                        {
                            Tipo = "UNIRSE",
                            Nombre = nombre
                        }));
                }
            }
            catch (Exception ex)
            {
                CerrarSala("No se pudo abrir la sala: " + ex.Message);
            }
        }

        private async Task VigilarServidorAsync(Task ejecucion)
        {
            try
            {
                await ejecucion;
                EnUiRed(() => CerrarSala("El servidor se detuvo."));
            }
            catch (Exception ex)
            {
                EnUiRed(() => CerrarSala(
                    "Error del servidor: " + ex.Message));
            }
        }

        private void RecibirEnServidor(int id, string texto)
        {
            MensajeUno mensaje;

            try
            {
                mensaje = JsonConvert.DeserializeObject<MensajeUno>(texto);
            }
            catch (JsonException)
            {
                ResponderError(id, "Mensaje inválido.");
                return;
            }

            if (mensaje == null)
                return;

            if (mensaje.Tipo == "UNIRSE")
            {
                RegistrarInvitado(id, mensaje.Nombre);
                return;
            }

            int jugador;

            if (!conexiones.TryGetValue(id, out jugador))
            {
                ResponderError(id, "Primero debes unirte a la sala.");
                return;
            }

            ProcesarAccion(jugador, mensaje, id);
        }

        private void RegistrarInvitado(int id, string nombre)
        {
            if (conexiones.ContainsKey(id))
                return;

            if (salaLista || conexiones.Count >= 2)
            {
                ResponderError(id, "La sala está llena.");
                return;
            }

            nombre = (nombre ?? "").Trim();

            if (nombre.Length == 0 || nombre.Length > 20)
            {
                ResponderError(id, "El nombre debe tener entre 1 y 20 letras.");
                return;
            }

            bool repetido = nombresJugadores[0].Equals(
                nombre, StringComparison.OrdinalIgnoreCase);

            foreach (int asiento in conexiones.Values)
            {
                repetido |= nombresJugadores[asiento].Equals(
                    nombre, StringComparison.OrdinalIgnoreCase);
            }

            if (repetido)
            {
                ResponderError(id, "Ese nombre ya está en la sala.");
                return;
            }

            int jugador = conexiones.Values.Contains(1) ? 2 : 1;

            conexiones.Add(id, jugador);
            nombresJugadores[jugador] = nombre;

            string aviso = "Esperando jugadores (" +
                (conexiones.Count + 1) + "/3).";

            espera.Text = aviso;

            foreach (int destino in conexiones.Keys.ToArray())
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Tipo = "ESPERA",
                    Texto = aviso
                });

                int conexion = destino;

                EncolarRed(() =>
                    servidorRed.EnviarAsync(conexion, json));
            }

            if (conexiones.Count == 2)
            {
                // Solo el anfitrión crea el mazo y reparte.
                NuevaPartida();

                salaLista = true;
                mesa.JugadorLocal = 0;
                mesa.EntradaHabilitada = true;
                espera.Visible = false;

                PublicarEstado();
            }
        }

        private void SalioCliente(int id)
        {
            if (!conexiones.ContainsKey(id))
                return;

            int jugador = conexiones[id];
            conexiones.Remove(id);

            if (salaLista)
            {
                CerrarSala(nombresJugadores[jugador] +
                    " abandonó la partida.");
                return;
            }

            nombresJugadores[jugador] = "Esperando...";
            espera.Text = "Esperando jugadores (" +
                (conexiones.Count + 1) + "/3).";

            string json = JsonConvert.SerializeObject(new
            {
                Tipo = "ESPERA",
                Texto = espera.Text
            });

            foreach (int destino in conexiones.Keys.ToArray())
            {
                int conexion = destino;
                EncolarRed(() =>
                    servidorRed.EnviarAsync(conexion, json));
            }
        }

        private void Solicitar(
            string tipo, int indice = -1, string color = null)
        {
            if (!salaLista || cerrandoRed)
                return;

            var mensaje = new MensajeUno
            {
                Tipo = tipo,
                Indice = indice,
                Color = color,
                Version = versionRed
            };

            if (anfitrion)
            {
                ProcesarAccion(0, mensaje, -1);
            }
            else
            {
                string json = JsonConvert.SerializeObject(mensaje);

                EncolarRed(() => clienteRed.EnviarAsync(json));
            }
        }

        private void ProcesarAccion(
            int jugador, MensajeUno mensaje, int conexion)
        {
            if (!salaLista)
                return;

            if (mensaje.Version != versionRed)
            {
                Rechazar(conexion,
                    "El tablero cambió. Vuelve a realizar la acción.");
                return;
            }

            if (mensaje.Tipo == "NUEVA")
            {
                if (jugador != 0 || !mesa.MostrarVictoria)
                {
                    Rechazar(conexion,
                        "El anfitrión inicia la siguiente ronda.");
                    return;
                }

                NuevaPartida();
                PublicarEstado();
                return;
            }

            if (mesa.MostrarVictoria)
                return;

            bool colorPendiente = partida.necesitaElegirColor();

            if (mensaje.Tipo == "ACUSAR")
            {
                if (colorPendiente || jugadorSinUno == null ||
                    partida.getJugador(jugador) == jugadorSinUno)
                {
                    Rechazar(conexion, "No puedes acusar ahora.");
                    return;
                }

                AlAcusarUno();
                PublicarEstado();
                return;
            }

            if (partida.getTurnoActual() != jugador)
            {
                Rechazar(conexion, "No es tu turno.");
                return;
            }

            if (colorPendiente && mensaje.Tipo != "COLOR")
            {
                Rechazar(conexion, "Primero elige el color.");
                return;
            }

            switch (mensaje.Tipo)
            {
                case "JUGAR":
                    var mano = partida.getJugador(jugador).getCartas();

                    if (mensaje.Indice < 0 || mensaje.Indice >= mano.Count)
                    {
                        Rechazar(conexion, "La carta ya no está disponible.");
                        return;
                    }

                    Carta carta = mano[mensaje.Indice];

                    if (!partida.sePuedeJugar(carta) ||
                        (cartaRobadaPendiente != null &&
                         carta != cartaRobadaPendiente))
                    {
                        Rechazar(conexion, "No puedes jugar esa carta.");
                        return;
                    }

                    AlHacerClicEnCarta(carta);
                    break;

                case "ROBAR":
                    if (cartaRobadaPendiente != null)
                    {
                        Rechazar(conexion, "Ya robaste este turno.");
                        return;
                    }

                    AlRobar();
                    break;

                case "PASAR":
                    if (cartaRobadaPendiente == null)
                    {
                        Rechazar(conexion, "Primero debes robar.");
                        return;
                    }

                    AlPasar();
                    break;

                case "UNO":
                    if (partida.getJugadorActual().getCartas().Count != 2)
                    {
                        Rechazar(conexion,
                            "Declara UNO antes de jugar tu penúltima carta.");
                        return;
                    }

                    AlPresionarUno();
                    break;

                case "COLOR":
                    if (!colorPendiente ||
                        !new[] { "Rojo", "Azul", "Verde", "Amarillo" }
                            .Contains(mensaje.Color))
                    {
                        Rechazar(conexion, "Selección de color inválida.");
                        return;
                    }

                    AlElegirColor(mensaje.Color);
                    break;

                default:
                    Rechazar(conexion, "Acción desconocida.");
                    return;
            }

            PublicarEstado();
        }

        private EstadoUno CrearEstado(int jugador)
        {
            var mano = partida.getJugador(jugador).getCartas();

            return new EstadoUno
            {
                Version = versionRed,
                MiJugador = jugador,
                Nombres = partida.getJugadores()
                    .Select(j => j.getNombre()).ToArray(),
                Cantidades = partida.getJugadores()
                    .Select(j => j.getCartas().Count).ToArray(),
                Mano = mano.Select(CartaRed.Desde).ToArray(),
                Descarte = CartaRed.Desde(
                    partida.getJuego().getUltimaCarta()),
                Color = partida.getColorActual(),
                Turno = partida.getTurnoActual(),
                CantidadMazo = partida.getMazo().getMazoSize(),
                Horario = mesa.SentidoHorario,
                ElegirColor = partida.necesitaElegirColor(),
                PuedePasar = cartaRobadaPendiente != null &&
                    partida.getTurnoActual() == jugador,
                PuedeAcusar = jugadorSinUno != null &&
                    jugadorSinUno != partida.getJugador(jugador) &&
                    !partida.necesitaElegirColor(),
                CartaRobada = cartaRobadaPendiente == null
                    ? -1 : mano.IndexOf(cartaRobadaPendiente),
                Rondas = (int[])mesa.Puntos.Clone(),
                Puntos = (int[])mesa.Puntuacion.Clone(),
                Victoria = mesa.MostrarVictoria,
                Ganador = mesa.NombreGanador,
                Titulo = mesa.TituloVictoria
            };
        }

        private void PublicarEstado()
        {
            versionRed++;

            foreach (var par in conexiones.ToArray())
            {
                int conexion = par.Key;
                string json = JsonConvert.SerializeObject(
                    CrearEstado(par.Value));

                EncolarRed(() =>
                    servidorRed.EnviarAsync(conexion, json));
            }

            mesa.MostrarSelectorColor =
                partida.necesitaElegirColor() &&
                partida.getTurnoActual() == 0;

            mesa.PasarDisponible =
                cartaRobadaPendiente != null &&
                partida.getTurnoActual() == 0;

            mesa.AcusarUnoDisponible =
                jugadorSinUno != null &&
                jugadorSinUno != partida.getJugador(0) &&
                !partida.necesitaElegirColor();

            mesa.Invalidate();
        }

        private void RecibirEnCliente(string texto)
        {
            try
            {
                JObject paquete = JObject.Parse(texto);
                string tipo = (string)paquete["Tipo"];

                if (tipo == "ESPERA")
                {
                    espera.Text = (string)paquete["Texto"];
                    return;
                }

                if (tipo == "ERROR")
                {
                    string error = (string)paquete["Texto"];

                    if (salaLista)
                        mesa.MostrarMensaje(error);
                    else
                        CerrarSala(error);

                    return;
                }

                if (tipo != "ESTADO")
                    return;

                EstadoUno estado = paquete.ToObject<EstadoUno>();

                if (salaLista && estado.Version <= versionRed)
                    return;

                miJugador = estado.MiJugador;
                versionRed = estado.Version;

                if (partida == null)
                    partida = new Partida();

                partida.CargarVistaRed(estado);

                mesa.Partida = partida;
                mesa.JugadorLocal = miJugador;
                mesa.EntradaHabilitada = true;
                mesa.SentidoHorario = estado.Horario;
                mesa.MostrarSelectorColor =
                    estado.ElegirColor && estado.Turno == miJugador;
                mesa.PasarDisponible = estado.PuedePasar;
                mesa.AcusarUnoDisponible = estado.PuedeAcusar;
                mesa.Puntos = estado.Rondas;
                mesa.Puntuacion = estado.Puntos;
                mesa.MostrarVictoria = estado.Victoria;
                mesa.NombreGanador = estado.Ganador;
                mesa.TituloVictoria = estado.Titulo;
                mesa.MostrarMenu = false;

                var mano = partida.getJugador(miJugador).getCartas();

                mesa.CartaRobadaPendiente =
                    estado.CartaRobada >= 0 &&
                    estado.CartaRobada < mano.Count
                        ? mano[estado.CartaRobada] : null;

                salaLista = true;
                espera.Visible = false;
                mesa.Invalidate();
            }
            catch (JsonException)
            {
                CerrarSala("El servidor envió un mensaje inválido.");
            }
        }

        private void Rechazar(int conexion, string texto)
        {
            if (conexion < 0)
                mesa.MostrarMensaje(texto);
            else
                ResponderError(conexion, texto);
        }

        private void ResponderError(int conexion, string texto)
        {
            string json = JsonConvert.SerializeObject(new
            {
                Tipo = "ERROR",
                Texto = texto
            });

            EncolarRed(() =>
                servidorRed.EnviarAsync(conexion, json));
        }

        private void CerrarSala(string motivo)
        {
            if (cerrandoRed)
                return;

            cerrandoRed = true;
            salaLista = false;
            mesa.EntradaHabilitada = false;

            clienteRed?.Dispose();
            servidorRed?.Dispose();

            MessageBox.Show(this, motivo, "Sala UNO");

            mesa.MostrarVictoria = true;
            Close();
        }
    }
}