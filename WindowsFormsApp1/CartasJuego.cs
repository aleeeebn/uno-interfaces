using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class CartasJuego
    {
        private List<Carta> cartas;
        public CartasJuego()
        {
            cartas = new List<Carta>();
        }
        public Carta getCarta(int i)
        {
            return cartas[i];
        }
        public int getCantidadCartas()
        {
            return cartas.Count();
        }
        public Carta getPrimerCarta()
        {
            return cartas[0];
        }
        public void eliminaPrimerCarta()
        {
            cartas.RemoveAt(0);
        }
        public void añadirCartas(Carta carta)
        {
            cartas.Add(carta);
        }
    }
}
