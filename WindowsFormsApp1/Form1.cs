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
            partida = new Partida();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(partida.getMazo().getMazoSize().ToString());
            //MessageBox.Show(partida.getMazo().getCarta(0).ToString());
            //partida.getMazo().barajear();
        }
    }
}
