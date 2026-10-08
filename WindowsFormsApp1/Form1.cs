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
            mostrarBotonesColor(false);
            roboEsteTurno = false;
            partida = new Partida();
            partida.repartir();
            partida.iniciaDescarte();
            actualizarCantidadCartas();
            lblDescarte.Text = partida.getJuego().getUltimaCarta().ToString();
            mostrarJugadorActual();
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
                if (partida.hayGanador())
                {
                    Jugador ganador = partida.getGanador();
                    MessageBox.Show("¡" + ganador.getNombre() + " ganó la partida!");
                    pnlMano.Controls.Clear();
                    pnlMano3.Controls.Clear();
                    pnlMano2.Controls.Clear();
                    btnmazo.Enabled = false;
                    btnpasar.Enabled = false;
                    return;
                }
                if (partida.necesitaElegirColor())
                {
                    mostrarBotonesColor(true);
                    pnlMano.Enabled = false;
                    btnmazo.Enabled = false;
                    btnpasar.Enabled = false;
                }
                else
                {
                    mostrarJugadorActual();
                }
            }
            else
            {
                MessageBox.Show("No se puede jugar esa carta");
            }
        }
        private void mostrarJugadorActual()
        {
            pnlMano.Controls.Clear();
            pnlMano2.Controls.Clear();
            pnlMano3.Controls.Clear();
            Jugador jugador1 = partida.getJugador(0);
            Jugador jugador2 = partida.getJugador(1);
            Jugador jugador3 = partida.getJugador(2);
            int turnoActual = partida.getTurnoActual();
            lblturno.Text = "Turno de: " + partida.getJugador(turnoActual).ToString();
            switch (turnoActual)
            {
                case 0:
                    pnlMano.Enabled = true;
                    pnlMano2.Enabled = false;
                    pnlMano3.Enabled = false;
                    break;
                case 1:
                    pnlMano.Enabled = false;
                    pnlMano2.Enabled = true;
                    pnlMano3.Enabled = false;
                    break;
                case 2:
                    pnlMano.Enabled = false;
                    pnlMano2.Enabled = false;
                    pnlMano3.Enabled = true;
                    break;
            }
            foreach(Carta carta in jugador1.getCartas())
            {
                Button botonCarta = new Button();
                botonCarta.Width = 80;
                botonCarta.Height = 120;
                botonCarta.Text = carta.ToString();
                botonCarta.Tag = carta;
                botonCarta.Click += jugarCarta;
                pnlMano.Controls.Add(botonCarta);
            }
            foreach (Carta carta in jugador2.getCartas())
            {
                Button botonCarta = new Button();
                botonCarta.Width = 80;
                botonCarta.Height = 120;
                botonCarta.Text = carta.ToString();
                botonCarta.Tag = carta;
                botonCarta.Click += jugarCarta;
                pnlMano2.Controls.Add(botonCarta);
            }
            foreach (Carta carta in jugador3.getCartas())
            {
                Button botonCarta = new Button();
                botonCarta.Width = 80;
                botonCarta.Height = 120;
                botonCarta.Text = carta.ToString();
                botonCarta.Tag = carta;
                botonCarta.Click += jugarCarta;
                pnlMano3.Controls.Add(botonCarta);
            }
        }
        private void mostrarBotonesColor(bool mostrar)
        {
            btnRojo.Visible = mostrar;
            btnAzul.Visible = mostrar;
            btnVerde.Visible = mostrar;
            btnAmarillo.Visible = mostrar;
        }
        private void actualizarCantidadCartas()
        {
            List<Jugador> jugadores = partida.getJugadores();
            lbljugador1.Text = jugadores[0].getNombre() + "\n" + jugadores[0].getCartas().Count + " cartas";
            lbljugador2.Text = jugadores[1].getNombre() + "\n" + jugadores[1].getCartas().Count + " cartas";
            lbljugador3.Text = jugadores[2].getNombre() + "\n" + jugadores[2].getCartas().Count + " cartas";
            lblMazo.Text = "Mazo: " + partida.getMazo().getMazoSize().ToString() + " cartas";
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
            if (roboEsteTurno)
            {
                MessageBox.Show("Ya robaste este turno");
                return;
            }
            partida.robarCarta();
            roboEsteTurno = true;
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }

        private void btnElegirColor_Click(object sender, EventArgs e)
        {
            partida.elegirColor("Rojo");
            actualizarDescarteComodin();
            mostrarBotonesColor(false);
            pnlMano.Enabled = true;
            btnmazo.Enabled = true;
            btnpasar.Enabled = true;
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            partida.elegirColor("Azul");
            actualizarDescarteComodin();
            mostrarBotonesColor(false);
            pnlMano.Enabled = true;
            btnmazo.Enabled = true;
            btnpasar.Enabled = true;
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }

        private void btnVerde_Click(object sender, EventArgs e)
        {
            partida.elegirColor("Verde");
            actualizarDescarteComodin();
            mostrarBotonesColor(false);
            pnlMano.Enabled = true;
            btnmazo.Enabled = true;
            btnpasar.Enabled = true;
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }

        private void btnAmarillo_Click(object sender, EventArgs e)
        {
            partida.elegirColor("Amarillo");
            actualizarDescarteComodin();
            mostrarBotonesColor(false);
            pnlMano.Enabled = true;
            btnmazo.Enabled = true;
            btnpasar.Enabled = true;
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }
        private void actualizarDescarteComodin()
        {
            Carta carta = partida.getJuego().getUltimaCarta();
            if (carta.getValor() == 13)
            {
                lblDescarte.Text = "Comodin\nColor: " + partida.getColorActual();
            }
            else if (carta.getValor() == 14)
            {
                lblDescarte.Text = "+4\nColor: " + partida.getColorActual();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lblMazo_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
