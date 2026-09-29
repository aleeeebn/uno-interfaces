using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class Carta
    {
        private string color;
        private int valor;
        public Carta(string color, int valor)
        {
            this.color = color;
            this.valor = valor;
        }
        public override string ToString()
        {
            return color + " " + valor;
        }
    }
}
