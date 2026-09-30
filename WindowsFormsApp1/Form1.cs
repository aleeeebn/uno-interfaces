using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Partida partida;
        public Form1()
        {
            InitializeComponent();
            partida = new Partida();
            partida.repartir();
            mostrarJugadorActual();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(partida.getMazo().getMazoSize().ToString());
            //MessageBox.Show(partida.getMazo().getCarta(0).ToString());
            //partida.getMazo().barajear();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        public void jugarCarta(object sender, EventArgs e)
        {
            Button boton = (Button)sender;
            Carta carta = (Carta)boton.Tag;
            partida.jugarCarta(carta);
            lblDescarte.Text = carta.ToString();
            mostrarJugadorActual();
        }
        private void mostrarJugadorActual()
        {
            pnlMano.Controls.Clear();
            Jugador jugador = partida.getJugadorActual();
            lblturno.Text = "Turno de: " + jugador.getNombre();
            foreach(Carta carta in jugador.getCartas())
            {
                Button botonCarta = new Button();
                botonCarta.Width = 80;
                botonCarta.Height = 120;
                botonCarta.Text = carta.ToString();
                botonCarta.Tag = carta;
                botonCarta.Click += jugarCarta;
                pnlMano.Controls.Add(botonCarta);
            }
        }
    }
}
