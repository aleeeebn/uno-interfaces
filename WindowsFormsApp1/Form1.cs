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
        private bool roboEsteTurno;
        public Form1()
        {
            InitializeComponent();
            roboEsteTurno = false;
            partida = new Partida();
            partida.repartir();
            partida.iniciaDescarte();
            actualizarCantidadCartas();
            lblDescarte.Text = partida.getJuego().getUltimaCarta().ToString();
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
            if (partida.sePuedeJugar(carta))
            {
                partida.jugarCarta(carta);
                lblDescarte.Text = carta.ToString();
                roboEsteTurno = false;
                actualizarCantidadCartas();
                mostrarJugadorActual();
            } else
            {
                MessageBox.Show("No se puede jugar esa carta");
            }
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
        private void actualizarCantidadCartas()
        {
            List<Jugador> jugadores = partida.getJugadores();
            lbljugador1.Text = jugadores[0].getNombre() + "\n" + jugadores[0].getCartas().Count + " cartas";
            lbljugador2.Text = jugadores[1].getNombre() + "\n" + jugadores[1].getCartas().Count + " cartas";
            lbljugador3.Text = jugadores[2].getNombre() + "\n" + jugadores[2].getCartas().Count + " cartas";
        }

        private void btnpasar_Click(object sender, EventArgs e)
        {
            if (!roboEsteTurno)
            {
                MessageBox.Show("Primero se debe de robar una carta");
                return;
            }
            partida.siguienteTurno();
            roboEsteTurno = false;
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }

        private void btnmazo_Click(object sender, EventArgs e)
        {
            roboEsteTurno = true;
            partida.robarCarta();
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }
    }
}
