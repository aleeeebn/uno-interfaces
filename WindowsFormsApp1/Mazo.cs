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
        private static int CANTIDAD = 12; // cuantas cartas hay en cada color x2 y comodines (o sea hay dos cartas de uno, dos de 6 y asi)
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
            for(int i = 0; i < CANTIDAD; i++)
            {
                Carta nuevaCartaRoja = new Carta("Rojo", i);
                cartas.Add(nuevaCartaRoja);
                cartas.Add(nuevaCartaRoja);
                Carta nuevaCartaAzul = new Carta("Azul", i);
                cartas.Add(nuevaCartaAzul);
                cartas.Add(nuevaCartaAzul);
                Carta nuevaCartaVerde = new Carta("Verde", i);
                cartas.Add(nuevaCartaVerde);
                cartas.Add(nuevaCartaVerde);
                Carta nuevaCartaAmarilla = new Carta("Amarilla", i);
                cartas.Add(nuevaCartaAmarilla);
                cartas.Add(nuevaCartaAmarilla);

                cant += 8;
            }
            for(int i = 0; i < CANTIDADCOMODINES; i++)
            {
                Carta nuevaCartaCambiaColor = new Carta("Comodin", 13);
                cartas.Add(nuevaCartaCambiaColor);
                Carta nuevaCartaComeCuatro = new Carta("Comodin", 14);
                cartas.Add(nuevaCartaComeCuatro);

                cant += 2;
            }
        }
        public void barajear()
        {
            for(int i = 0; i < cant; i++)
            {
                Random random = new Random();
                int cartaAleatoria = random.Next(0, cant);
                Carta temp = this.getCarta(i);
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
