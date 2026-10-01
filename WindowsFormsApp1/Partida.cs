using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
        private int direccion;
        private int comodinPendiente;
        public Partida()
        {
            mazo = new Mazo();
            cantTotalCartas = mazo.getCantCartas();
            jugadores = new List<Jugador>();
            juego = new CartasJuego();
            turnoActual = 0;
            direccion = 1;
            comodinPendiente = 0;
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
        public Jugador getGanador()
        {
            foreach(Jugador jugador in jugadores)
            {
                if (jugador.noTieneCartas())
                {
                    return jugador;
                }
            }
            return null;
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
                while(juego.getCantidadCartas() > 1)
                {
                    mazo.add(juego.getPrimerCarta());
                    juego.eliminaPrimerCarta();
                }
                mazo.barajear();
            }
        }
        public void siguienteTurno()
        {
            turnoActual += direccion;
            if(turnoActual >= jugadores.Count)
            {
                turnoActual = 0;
            }
            if(turnoActual < 0)
            {
                turnoActual = jugadores.Count - 1;
            }
        }
        public void jugarCarta(Carta carta)
        {
            jugadores[turnoActual].removeCarta(carta);
            juego.añadirCartas(carta);
            if (carta.getValor() == 13 || carta.getValor() == 14)
            {
                comodinPendiente = carta.getValor();
                return;
            }
            colorActual = carta.getColor();
            aplicarEfecto(carta);
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
            int i = 0;
            while(i < mazo.getMazoSize() && mazo.getCarta(i).getValor() > 9)
            {
                i++;
            }
            if (i >= mazo.getMazoSize()) return;
            Carta carta = mazo.getCarta(i);
            juego.añadirCartas(carta);
            mazo.removeCarta(i);
            colorActual = carta.getColor();
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
                if (mazo.estaVacio()) return;
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
                    siguienteTurno();
                    robarCartas(jugadores[turnoActual], 2);
                    siguienteTurno();
                    break;
                case 11:
                    direccion *= -1;
                    siguienteTurno();
                    break;
                case 12:
                    siguienteTurno();
                    siguienteTurno();
                    break;
                default:
                    siguienteTurno();
                    break;
            }
        }
        public bool hayGanador()
        {
            foreach(Jugador jugador in jugadores)
            {
                if (jugador.noTieneCartas())
                    return true;
            }
            return false;
        }
        public void elegirColor(string color)
        {
            colorActual = color;
            if(comodinPendiente == 14)
            {
                siguienteTurno();
                robarCartas(jugadores[turnoActual], 4);
                siguienteTurno();
            } else
            {
                siguienteTurno();
            }
            comodinPendiente = 0;
        }
        public bool necesitaElegirColor()
        {
            return comodinPendiente == 13 || comodinPendiente == 14;
        }
    }
}
