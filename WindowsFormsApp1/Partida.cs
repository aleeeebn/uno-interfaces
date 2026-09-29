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
        private List<Jugador> jugadores;
        private static int CANTIDADJUGADORESDEFAULT = 3;
        public Partida()
        {
            mazo = new Mazo();
            jugadores = new List<Jugador>();
            agregarJugadoresDefault();
        }
        private void agregarJugadoresDefault()
        {
            for(int i = 0; i < CANTIDADJUGADORESDEFAULT; i++)
            {
                Jugador nuevoJugador = new Jugador("Jugador " + i);
                jugadores.Add(nuevoJugador);
            }
        }
    }
}
