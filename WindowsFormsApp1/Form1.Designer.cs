namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbljugador1 = new System.Windows.Forms.Label();
            this.lbljugador2 = new System.Windows.Forms.Label();
            this.lbljugador3 = new System.Windows.Forms.Label();
            this.lblturno = new System.Windows.Forms.Label();
            this.pnlMano = new System.Windows.Forms.FlowLayoutPanel();
            this.btnmazo = new System.Windows.Forms.Button();
            this.btnpasar = new System.Windows.Forms.Button();
            this.btnRojo = new System.Windows.Forms.Button();
            this.btnAmarillo = new System.Windows.Forms.Button();
            this.btnAzul = new System.Windows.Forms.Button();
            this.btnVerde = new System.Windows.Forms.Button();
            this.lblMazo = new System.Windows.Forms.Label();
            this.picDescarte = new System.Windows.Forms.PictureBox();
            this.picColor = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picDescarte)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picColor)).BeginInit();
            this.SuspendLayout();
            // 
            // lbljugador1
            // 
            this.lbljugador1.AutoSize = true;
            this.lbljugador1.Location = new System.Drawing.Point(126, 150);
            this.lbljugador1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbljugador1.Name = "lbljugador1";
            this.lbljugador1.Size = new System.Drawing.Size(108, 25);
            this.lbljugador1.TabIndex = 0;
            this.lbljugador1.Text = "Jugador 1";
            this.lbljugador1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lbljugador2
            // 
            this.lbljugador2.AutoSize = true;
            this.lbljugador2.Location = new System.Drawing.Point(720, 73);
            this.lbljugador2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbljugador2.Name = "lbljugador2";
            this.lbljugador2.Size = new System.Drawing.Size(108, 25);
            this.lbljugador2.TabIndex = 1;
            this.lbljugador2.Text = "Jugador 2";
            // 
            // lbljugador3
            // 
            this.lbljugador3.AutoSize = true;
            this.lbljugador3.Location = new System.Drawing.Point(1330, 150);
            this.lbljugador3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbljugador3.Name = "lbljugador3";
            this.lbljugador3.Size = new System.Drawing.Size(108, 25);
            this.lbljugador3.TabIndex = 2;
            this.lbljugador3.Text = "Jugador 3";
            // 
            // lblturno
            // 
            this.lblturno.AutoSize = true;
            this.lblturno.Location = new System.Drawing.Point(-4, 496);
            this.lblturno.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblturno.Name = "lblturno";
            this.lblturno.Size = new System.Drawing.Size(80, 25);
            this.lblturno.TabIndex = 3;
            this.lblturno.Text = "Turno; ";
            // 
            // pnlMano
            // 
            this.pnlMano.AutoScroll = true;
            this.pnlMano.Location = new System.Drawing.Point(2, 577);
            this.pnlMano.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.pnlMano.Name = "pnlMano";
            this.pnlMano.Size = new System.Drawing.Size(1574, 192);
            this.pnlMano.TabIndex = 4;
            this.pnlMano.WrapContents = false;
            // 
            // btnmazo
            // 
            this.btnmazo.Location = new System.Drawing.Point(164, 527);
            this.btnmazo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnmazo.Name = "btnmazo";
            this.btnmazo.Size = new System.Drawing.Size(150, 44);
            this.btnmazo.TabIndex = 5;
            this.btnmazo.Text = "robar";
            this.btnmazo.UseVisualStyleBackColor = true;
            this.btnmazo.Click += new System.EventHandler(this.btnmazo_Click);
            // 
            // btnpasar
            // 
            this.btnpasar.Location = new System.Drawing.Point(2, 527);
            this.btnpasar.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnpasar.Name = "btnpasar";
            this.btnpasar.Size = new System.Drawing.Size(150, 44);
            this.btnpasar.TabIndex = 6;
            this.btnpasar.Text = "pasar";
            this.btnpasar.UseVisualStyleBackColor = true;
            this.btnpasar.Click += new System.EventHandler(this.btnpasar_Click);
            // 
            // btnRojo
            // 
            this.btnRojo.Location = new System.Drawing.Point(1172, 521);
            this.btnRojo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnRojo.Name = "btnRojo";
            this.btnRojo.Size = new System.Drawing.Size(150, 44);
            this.btnRojo.TabIndex = 9;
            this.btnRojo.Text = "Rojo";
            this.btnRojo.UseVisualStyleBackColor = true;
            // 
            // btnAmarillo
            // 
            this.btnAmarillo.Location = new System.Drawing.Point(1334, 521);
            this.btnAmarillo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnAmarillo.Name = "btnAmarillo";
            this.btnAmarillo.Size = new System.Drawing.Size(150, 44);
            this.btnAmarillo.TabIndex = 10;
            this.btnAmarillo.Text = "Amarillo";
            this.btnAmarillo.UseVisualStyleBackColor = true;
            // 
            // btnAzul
            // 
            this.btnAzul.Location = new System.Drawing.Point(1010, 521);
            this.btnAzul.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnAzul.Name = "btnAzul";
            this.btnAzul.Size = new System.Drawing.Size(150, 44);
            this.btnAzul.TabIndex = 11;
            this.btnAzul.Text = "Azul";
            this.btnAzul.UseVisualStyleBackColor = true;
            // 
            // btnVerde
            // 
            this.btnVerde.Location = new System.Drawing.Point(848, 521);
            this.btnVerde.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnVerde.Name = "btnVerde";
            this.btnVerde.Size = new System.Drawing.Size(150, 44);
            this.btnVerde.TabIndex = 12;
            this.btnVerde.Text = "Verde";
            this.btnVerde.UseVisualStyleBackColor = true;
            // 
            // lblMazo
            // 
            this.lblMazo.AutoSize = true;
            this.lblMazo.Location = new System.Drawing.Point(326, 537);
            this.lblMazo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMazo.Name = "lblMazo";
            this.lblMazo.Size = new System.Drawing.Size(175, 25);
            this.lblMazo.TabIndex = 13;
            this.lblMazo.Text = "Cartas en mazo: ";
            // 
            // picDescarte
            // 
            this.picDescarte.Location = new System.Drawing.Point(715, 278);
            this.picDescarte.Name = "picDescarte";
            this.picDescarte.Size = new System.Drawing.Size(80, 120);
            this.picDescarte.TabIndex = 14;
            this.picDescarte.TabStop = false;
            // 
            // picColor
            // 
            this.picColor.Location = new System.Drawing.Point(869, 278);
            this.picColor.Name = "picColor";
            this.picColor.Size = new System.Drawing.Size(40, 40);
            this.picColor.TabIndex = 15;
            this.picColor.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 865);
            this.Controls.Add(this.picColor);
            this.Controls.Add(this.picDescarte);
            this.Controls.Add(this.lblMazo);
            this.Controls.Add(this.btnVerde);
            this.Controls.Add(this.btnAzul);
            this.Controls.Add(this.btnAmarillo);
            this.Controls.Add(this.btnRojo);
            this.Controls.Add(this.btnpasar);
            this.Controls.Add(this.btnmazo);
            this.Controls.Add(this.pnlMano);
            this.Controls.Add(this.lblturno);
            this.Controls.Add(this.lbljugador3);
            this.Controls.Add(this.lbljugador2);
            this.Controls.Add(this.lbljugador1);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.picDescarte)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbljugador1;
        private System.Windows.Forms.Label lbljugador2;
        private System.Windows.Forms.Label lbljugador3;
        private System.Windows.Forms.Label lblturno;
        private System.Windows.Forms.FlowLayoutPanel pnlMano;
        private System.Windows.Forms.Button btnmazo;
        private System.Windows.Forms.Button btnpasar;
        private System.Windows.Forms.Button btnRojo;
        private System.Windows.Forms.Button btnAmarillo;
        private System.Windows.Forms.Button btnAzul;
        private System.Windows.Forms.Button btnVerde;
        private System.Windows.Forms.Label lblMazo;
        private System.Windows.Forms.PictureBox picDescarte;
        private System.Windows.Forms.PictureBox picColor;
    }
}

