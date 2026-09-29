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
        public void añadirCartas(Carta carta)
        {
            cartas.Add(carta);
        }
    }
}
