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
        /*
         0 - 9. numeros normales
        comodines
         10. +2
         11. cambio de dirección
         12. salto
         13. cambio de color
         14. +4
         */
        public Carta(string color, int valor)
        {
            this.color = color;
            this.valor = valor;
        }
        public string getColor()
        {
            return this.color;
        }
        public int getValor()
        {
            return valor;
        }
        public void setColor(string color)
        {
            this.color = color;
        }
        public void setValor(int valor)
        {
            this.valor = valor;
        }
        public override string ToString()
        {
            return color + " " + valor;
        }
    }
}
