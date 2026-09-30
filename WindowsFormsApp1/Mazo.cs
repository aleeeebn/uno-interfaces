using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class Mazo
    {
        private List<Carta> cartas;
        private int cant;
        private static int CANTIDADCOMODINES = 4; // iterador para el número máximo de un comodin que puede haber de +4 y cambios de color(no representa la cantidad total de comodines)
        public Mazo()
        {
            cant = 0;
            cartas = new List<Carta>();
            mazoDefault();
            barajear();
        }
        public int getCantCartas()
        {
            return this.cant;
        }
        public Carta getCarta(int i)
        {
            return this.cartas[i];
        }
        public int getMazoSize()
        {
            return cartas.Count();
        }
        public void removeCarta(int i)
        {
            this.cartas.RemoveAt(i);
        }
        public bool estaVacio()
        {
            if (this.cartas.Count() == 0)
                return true;
            return false;
        }
        public void mazoDefault()
        {
            string[] colores = { "Rojo", "Azul", "Amarillo", "Verde" };

            foreach(string color in colores)
            {
                cartas.Add(new Carta(color, 0));
                for(int numero = 1; numero <= 9; numero++)
                {
                    cartas.Add(new Carta(color, numero));
                    cartas.Add(new Carta(color, numero));
                }
                cartas.Add(new Carta(color, 10));
                cartas.Add(new Carta(color, 10));

                cartas.Add(new Carta(color, 11));
                cartas.Add(new Carta(color, 11));

                cartas.Add(new Carta(color, 12));
                cartas.Add(new Carta(color, 12));
            }

            for(int i = 0; i < CANTIDADCOMODINES; i++)
            {
                Carta nuevaCartaCambiaColor = new Carta("Comodin", 13);
                cartas.Add(nuevaCartaCambiaColor);
                Carta nuevaCartaComeCuatro = new Carta("Comodin", 14);
                cartas.Add(nuevaCartaComeCuatro);
            }
            cant = cartas.Count;
        }
        public void barajear()
        {
            Random random = new Random();
            for (int i = 0; i < cant; i++)
            {
                int cartaAleatoria = random.Next(i + 1);
                Carta temp = cartas[i];
                cartas[i] = cartas[cartaAleatoria];
                cartas[cartaAleatoria] = temp;
            }
        }
        public void add(Carta nuevaCarta)
        {
            cartas.Add(nuevaCarta);
        }
    }
}
