using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class Jugador
    {
        private string nombre;
        private List<Carta> cartas;
        public Jugador(string nombre)
        {
            this.nombre = nombre;
            cartas = new List<Carta>();
        }
        public List<Carta> getCartas()
        {
            return cartas;
        }
        public string getNombre()
        {
            return this.nombre;
        }
        public void setNombre(string nombre)
        {
            this.nombre = nombre;
        }
        public bool uno()
        {
            if (cartas.Count() == 1)
                return true;
            return false;
        }
        public bool noTieneCartas()
        {
            if (cartas.Count == 0)
                return true;
            return false;
        }
        public void añadirCarta(Carta carta)
        {
            cartas.Add(carta);
        }
        public void removeCarta(Carta carta)
        {
            cartas.Remove(carta);
        }
        public override string ToString()
        {
            return nombre;
        }
    }
}