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
            this.lblDescarte = new System.Windows.Forms.Label();
            this.btnRojo = new System.Windows.Forms.Button();
            this.btnAmarillo = new System.Windows.Forms.Button();
            this.btnAzul = new System.Windows.Forms.Button();
            this.btnVerde = new System.Windows.Forms.Button();
            this.lblMazo = new System.Windows.Forms.Label();
            this.pnlMano2 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlMano3 = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // lbljugador1
            // 
            this.lbljugador1.AutoSize = true;
            this.lbljugador1.Location = new System.Drawing.Point(-2, 20);
            this.lbljugador1.Name = "lbljugador1";
            this.lbljugador1.Size = new System.Drawing.Size(54, 13);
            this.lbljugador1.TabIndex = 0;
            this.lbljugador1.Text = "Jugador 1";
            this.lbljugador1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lbljugador2
            // 
            this.lbljugador2.AutoSize = true;
            this.lbljugador2.Location = new System.Drawing.Point(1193, 20);
            this.lbljugador2.Name = "lbljugador2";
            this.lbljugador2.Size = new System.Drawing.Size(54, 13);
            this.lbljugador2.TabIndex = 1;
            this.lbljugador2.Text = "Jugador 2";
            // 
            // lbljugador3
            // 
            this.lbljugador3.AutoSize = true;
            this.lbljugador3.Location = new System.Drawing.Point(-2, 437);
            this.lbljugador3.Name = "lbljugador3";
            this.lbljugador3.Size = new System.Drawing.Size(54, 13);
            this.lbljugador3.TabIndex = 2;
            this.lbljugador3.Text = "Jugador 3";
            // 
            // lblturno
            // 
            this.lblturno.AutoSize = true;
            this.lblturno.Location = new System.Drawing.Point(732, 437);
            this.lblturno.Name = "lblturno";
            this.lblturno.Size = new System.Drawing.Size(41, 13);
            this.lblturno.TabIndex = 3;
            this.lblturno.Text = "Turno; ";
            // 
            // pnlMano
            // 
            this.pnlMano.AutoScroll = true;
            this.pnlMano.Location = new System.Drawing.Point(1, 54);
            this.pnlMano.Name = "pnlMano";
            this.pnlMano.Size = new System.Drawing.Size(553, 117);
            this.pnlMano.TabIndex = 4;
            this.pnlMano.WrapContents = false;
            // 
            // btnmazo
            // 
            this.btnmazo.Location = new System.Drawing.Point(285, 263);
            this.btnmazo.Name = "btnmazo";
            this.btnmazo.Size = new System.Drawing.Size(75, 23);
            this.btnmazo.TabIndex = 5;
            this.btnmazo.Text = "robar";
            this.btnmazo.UseVisualStyleBackColor = true;
            this.btnmazo.Click += new System.EventHandler(this.btnmazo_Click);
            // 
            // btnpasar
            // 
            this.btnpasar.Location = new System.Drawing.Point(285, 324);
            this.btnpasar.Name = "btnpasar";
            this.btnpasar.Size = new System.Drawing.Size(75, 23);
            this.btnpasar.TabIndex = 6;
            this.btnpasar.Text = "pasar";
            this.btnpasar.UseVisualStyleBackColor = true;
            this.btnpasar.Click += new System.EventHandler(this.btnpasar_Click);
            // 
            // lblDescarte
            // 
            this.lblDescarte.AutoSize = true;
            this.lblDescarte.Location = new System.Drawing.Point(576, 300);
            this.lblDescarte.Name = "lblDescarte";
            this.lblDescarte.Size = new System.Drawing.Size(61, 13);
            this.lblDescarte.TabIndex = 7;
            this.lblDescarte.Text = "última carta";
            // 
            // btnRojo
            // 
            this.btnRojo.Location = new System.Drawing.Point(960, 279);
            this.btnRojo.Name = "btnRojo";
            this.btnRojo.Size = new System.Drawing.Size(75, 23);
            this.btnRojo.TabIndex = 9;
            this.btnRojo.Text = "Rojo";
            this.btnRojo.UseVisualStyleBackColor = true;
            this.btnRojo.Click += new System.EventHandler(this.btnElegirColor_Click);
            // 
            // btnAmarillo
            // 
            this.btnAmarillo.Location = new System.Drawing.Point(960, 237);
            this.btnAmarillo.Name = "btnAmarillo";
            this.btnAmarillo.Size = new System.Drawing.Size(75, 23);
            this.btnAmarillo.TabIndex = 10;
            this.btnAmarillo.Text = "Amarillo";
            this.btnAmarillo.UseVisualStyleBackColor = true;
            this.btnAmarillo.Click += new System.EventHandler(this.btnAmarillo_Click);
            // 
            // btnAzul
            // 
            this.btnAzul.Location = new System.Drawing.Point(960, 324);
            this.btnAzul.Name = "btnAzul";
            this.btnAzul.Size = new System.Drawing.Size(75, 23);
            this.btnAzul.TabIndex = 11;
            this.btnAzul.Text = "Azul";
            this.btnAzul.UseVisualStyleBackColor = true;
            this.btnAzul.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnVerde
            // 
            this.btnVerde.Location = new System.Drawing.Point(960, 368);
            this.btnVerde.Name = "btnVerde";
            this.btnVerde.Size = new System.Drawing.Size(75, 23);
            this.btnVerde.TabIndex = 12;
            this.btnVerde.Text = "Verde";
            this.btnVerde.UseVisualStyleBackColor = true;
            this.btnVerde.Click += new System.EventHandler(this.btnVerde_Click);
            // 
            // lblMazo
            // 
            this.lblMazo.AutoSize = true;
            this.lblMazo.Location = new System.Drawing.Point(282, 300);
            this.lblMazo.Name = "lblMazo";
            this.lblMazo.Size = new System.Drawing.Size(86, 13);
            this.lblMazo.TabIndex = 13;
            this.lblMazo.Text = "Cartas en mazo: ";
            this.lblMazo.Click += new System.EventHandler(this.lblMazo_Click);
            // 
            // pnlMano2
            // 
            this.pnlMano2.AutoScroll = true;
            this.pnlMano2.Location = new System.Drawing.Point(694, 54);
            this.pnlMano2.Name = "pnlMano2";
            this.pnlMano2.Size = new System.Drawing.Size(553, 117);
            this.pnlMano2.TabIndex = 14;
            this.pnlMano2.WrapContents = false;
            this.pnlMano2.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // pnlMano3
            // 
            this.pnlMano3.AutoScroll = true;
            this.pnlMano3.Location = new System.Drawing.Point(1, 469);
            this.pnlMano3.Name = "pnlMano3";
            this.pnlMano3.Size = new System.Drawing.Size(553, 117);
            this.pnlMano3.TabIndex = 15;
            this.pnlMano3.WrapContents = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1259, 755);
            this.Controls.Add(this.pnlMano3);
            this.Controls.Add(this.pnlMano2);
            this.Controls.Add(this.lblMazo);
            this.Controls.Add(this.btnVerde);
            this.Controls.Add(this.btnAzul);
            this.Controls.Add(this.btnAmarillo);
            this.Controls.Add(this.btnRojo);
            this.Controls.Add(this.lblDescarte);
            this.Controls.Add(this.btnpasar);
            this.Controls.Add(this.btnmazo);
            this.Controls.Add(this.pnlMano);
            this.Controls.Add(this.lblturno);
            this.Controls.Add(this.lbljugador3);
            this.Controls.Add(this.lbljugador2);
            this.Controls.Add(this.lbljugador1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
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
        private System.Windows.Forms.Label lblDescarte;
        private System.Windows.Forms.Button btnRojo;
        private System.Windows.Forms.Button btnAmarillo;
        private System.Windows.Forms.Button btnAzul;
        private System.Windows.Forms.Button btnVerde;
        private System.Windows.Forms.Label lblMazo;
        private System.Windows.Forms.FlowLayoutPanel pnlMano2;
        private System.Windows.Forms.FlowLayoutPanel pnlMano3;
    }
}

