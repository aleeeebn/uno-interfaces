using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class Partida
    {
        private Mazo mazo;
        private CartasJuego juego;
        private List<Jugador> jugadores;
        private static int CANTIDADJUGADORESDEFAULT = 3;
        private static int CANTIDADCARTASPORJUGADOR = 7;
        private int cantTotalCartas;
        public Partida()
        {
            mazo = new Mazo();
            cantTotalCartas = mazo.getCantCartas();
            jugadores = new List<Jugador>();
            juego = new CartasJuego();
            agregarJugadoresDefault();
            repartir();
        }
        public Mazo getMazo()
        {
            return this.mazo;
        }
        private void agregarJugadoresDefault()
        {
            for(int i = 0; i < CANTIDADJUGADORESDEFAULT; i++)
            {
                Jugador nuevoJugador = new Jugador("Jugador " + i);
                jugadores.Add(nuevoJugador);
            }
        }
        private void repartir()
        {
            foreach(Jugador jugador in jugadores)
            {
                for(int i = 0; i < CANTIDADCARTASPORJUGADOR; i++)
                {
                    Random random = new Random();
                    int cartaSeleccionada = random.Next(0, cantTotalCartas + 1);
                    jugador.añadirCarta(mazo.getCarta(cartaSeleccionada));
                    juego.añadirCartas(mazo.getCarta(cartaSeleccionada));
                    mazo.removeCarta(cartaSeleccionada);
                    cantTotalCartas--;
                }
            }
        }
    }
}
