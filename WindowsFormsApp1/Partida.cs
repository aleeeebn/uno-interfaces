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
        private string colorActual;
        public Partida()
        {
            mazo = new Mazo();
            cantTotalCartas = mazo.getCantCartas();
            jugadores = new List<Jugador>();
            juego = new CartasJuego();
            turnoActual = 0;
            agregarJugadoresDefault();
        }
        public string getColorActual()
        {
            return colorActual;
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
        public CartasJuego getJuego()
        {
            return juego;
        }
        public void setColorActual(string color)
        {
            colorActual = color;
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
            juego.añadirCartas(carta);
            if (carta.getColor() != "Comodin")
            {
                colorActual = carta.getColor();
            }
            this.siguienteTurno();
        }
        public bool sePuedeJugar(Carta carta)
        {
            Carta cartaActual = juego.getUltimaCarta();
            if (cartaActual == null)
                return true;
            if (carta.getColor() == "Comodin")
                return true;
            if (carta.getColor() == colorActual)
                return true;
            if (carta.getValor() == cartaActual.getValor())
                return true;
            return false;
        }
        public void iniciaDescarte()
        {
            Carta carta = mazo.getCarta(0);
            juego.añadirCartas(carta);
            mazo.removeCarta(0);
        }
        public void robarCarta()
        {
            siMazoEstaVacio();
            if (!mazo.estaVacio())
            {
                Carta carta = mazo.getCarta(0);
                jugadores[turnoActual].añadirCarta(carta);
                mazo.removeCarta(0);
            }
        }
        public void robarCartas(Jugador jugador, int cantidad)
        {
            for(int i = 0; i < cantidad; i++)
            {
                siMazoEstaVacio();
                Carta carta = mazo.getCarta(0);
                jugador.añadirCarta(carta);
                mazo.removeCarta(0);
            }
        }
        private void aplicarEfecto(Carta carta)
        {
            switch (carta.getValor())
            {
                case 10:
                    break;
                case 11:
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
            }
        }
    }
}
