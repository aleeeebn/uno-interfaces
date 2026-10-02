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
                    pnlMano.Enabled = false;
                    btnmazo.Enabled = false;
                    btnpasar.Enabled = false;
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
            int turnoActual = partida.getTurnoActual();
            lblturno.Text = "Turno de: " + partida.getJugador(turnoActual).ToString();

            FlowLayoutPanel[] paneles = { pnlMano, pnlMano2, pnlMano3 };
            for (int i = 0; i < paneles.Length; i++)
            {
                llenarMano(paneles[i], partida.getJugador(i), i == turnoActual);
            }
            actualizarDescarte();
        }
        private void llenarMano(FlowLayoutPanel panel, Jugador jugador, bool esSuTurno)
        {
            panel.Controls.Clear();
            panel.Enabled = esSuTurno;
            foreach (Carta carta in jugador.getCartas())
            {
                Button b = new Button();
                b.Width = ImagenesCartas.Ancho;
                b.Height = ImagenesCartas.Alto;
                b.FlatStyle = FlatStyle.Flat;
                b.BackgroundImageLayout = ImageLayout.Stretch;
                b.Tag = carta;
                if (esSuTurno)
                {
                    b.BackgroundImage = ImagenesCartas.Obtener(carta);
                    b.Click += jugarCarta;
                    if (!partida.sePuedeJugar(carta))
                        b.FlatAppearance.BorderColor = Color.Gray;
                    else
                        b.FlatAppearance.BorderColor = Color.LimeGreen;
                    b.FlatAppearance.BorderSize = 3;
                }
                else
                {
                    b.BackgroundImage = ImagenesCartas.Dorso();
                }
                panel.Controls.Add(b);
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
