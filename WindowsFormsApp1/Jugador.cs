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
        public string getNombre()
        {
            return this.nombre;
        }
        public void setNombre(string nombre)
        {
            this.nombre = nombre;
        }
    }
}
