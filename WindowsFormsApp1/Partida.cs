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
        private int turnoActual;
        public Partida()
        {
            mazo = new Mazo();
            cantTotalCartas = mazo.getCantCartas();
            jugadores = new List<Jugador>();
            juego = new CartasJuego();
            turnoActual = 0;
            agregarJugadoresDefault();
        }
        public Jugador getJugadorActual()
        {
            return jugadores[turnoActual];
        }
        public List<Jugador> getJugadores()
        {
            return jugadores;
        }
        public Mazo getMazo()
        {
            return this.mazo;
        }
        public void agregarJugadoresDefault()
        {
            for(int i = 1; i <= CANTIDADJUGADORESDEFAULT; i++)
            {
                Jugador nuevoJugador = new Jugador("Jugador " + i);
                jugadores.Add(nuevoJugador);
            }
        }
        public void repartir()
        {
            foreach(Jugador jugador in jugadores)
            {
                for(int i = 0; i < CANTIDADCARTASPORJUGADOR; i++)
                {
                    Carta carta = mazo.getCarta(0);
                    jugador.añadirCarta(carta);
                    mazo.removeCarta(0);
                    cantTotalCartas--;
                }
            }
        }
        public void siMazoEstaVacio()
        {
            if (mazo.estaVacio())
            {
                for(int i = 0; i < juego.getCantidadCartas() - 1; i++)
                {
                    mazo.add(juego.getPrimerCarta());
                    juego.eliminaPrimerCarta();
                }
                mazo.barajear();
            }
        }
        public void siguienteTurno()
        {
            turnoActual++;
            if(turnoActual >= jugadores.Count)
            {
                turnoActual = 0;
            }
        }
        public void jugarCarta(Carta carta)
        {
            jugadores[turnoActual].removeCarta(carta);
            this.siguienteTurno();
        }
    }
}
