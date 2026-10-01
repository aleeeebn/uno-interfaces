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
                    btnmazo.Enabled = false;
                    btnpasar.Enabled = false;
                    return;
                }
                if (partida.necesitaElegirColor())
                {
                    mostrarBotonesColor(true);
                } else
                {
                    mostrarJugadorActual();
                }
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
            roboEsteTurno = true;
            partida.robarCarta();
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }

        private void btnElegirColor_Click(object sender, EventArgs e)
        {
            partida.elegirColor("Rojo");
            actualizarDescarteComodin();
            mostrarBotonesColor(false);
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            partida.elegirColor("Azul");
            actualizarDescarteComodin();
            mostrarBotonesColor(false);
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }

        private void btnVerde_Click(object sender, EventArgs e)
        {
            partida.elegirColor("Verde");
            actualizarDescarteComodin();
            mostrarBotonesColor(false);
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }

        private void btnAmarillo_Click(object sender, EventArgs e)
        {
            partida.elegirColor("Amarillo");
            actualizarDescarteComodin();
            mostrarBotonesColor(false);
            actualizarCantidadCartas();
            mostrarJugadorActual();
        }
        private void actualizarDescarteComodin()
        {
            Carta carta = partida.getJuego().getUltimaCarta();
            if(carta.getValor() == 13)
            {
                lblDescarte.Text = "Comodin\nColor: " + partida.getColorActual();
            }
            else if(carta.getValor() == 14)
            {
                lblDescarte.Text = "+4\nColor: " + partida.getColorActual();
            }
        }
    }
}
