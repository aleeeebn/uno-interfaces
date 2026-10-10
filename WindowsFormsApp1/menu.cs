using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsApp1
{ 

    public class Menu : Form
    {
        static readonly Color RosaFondo1 = Color.FromArgb(255, 244, 244);
        static readonly Color RosaFondo2 = Color.FromArgb(255, 214, 226);
        static readonly Color Rosa = Color.FromArgb(255, 160, 184);
        static readonly Color Amarillo = Color.FromArgb(255, 214, 140);
        static readonly Color Verde = Color.FromArgb(160, 220, 190);
        static readonly Color Lila = Color.FromArgb(205, 190, 245);
        static readonly Color Texto = Color.FromArgb(125, 65, 95);

        BotonPastel btnJugar, btnHistorial, btnReglas, btnSalir;
        Panel panelNombres;
        TextBox[] nombres = new TextBox[3];
        public Menu()
        {
            Text = "UNO";
            ClientSize = new Size(1100, 740);
            MinimumSize = new Size(900, 640);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            btnJugar = CrearBoton("Jugar", Rosa);
            btnHistorial = CrearBoton("Historial", Amarillo);
            btnReglas = CrearBoton("Reglas", Verde);
            btnSalir = CrearBoton("Salir", Lila);

            btnJugar.Click += (s, e) => MostrarPantallaNombres();
            btnHistorial.Click += (s, e) => AbrirHistorial();
            btnReglas.Click += (s, e) => MostrarReglas();
            btnSalir.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { btnJugar, btnHistorial, btnReglas, btnSalir });

            Resize += (s, e) => { Acomodar(); Invalidate(); };
            Acomodar();
        }

        

        BotonPastel CrearBoton(string texto, Color color)
        {
            return new BotonPastel { Text = texto, ColorBase = color, ColorTexto = Texto, Size = new Size(320, 78) };
        }
        void MostrarPantallaNombres()
        {
            btnJugar.Visible = false;
            btnHistorial.Visible = false;
            btnReglas.Visible = false;
            btnSalir.Visible = false;

            panelNombres = new Panel{Size = new Size(440, 440),BackColor = Color.FromArgb(255, 250, 245)};
            Label lblError = new Label{Text = "",ForeColor = Color.FromArgb(210, 75, 110),Font = new Font("Segoe UI", 10, FontStyle.Bold),TextAlign = ContentAlignment.MiddleCenter,Location = new Point(30, 326),Size = new Size(380, 25)};

            panelNombres.Controls.Add(lblError);

            var titulo = new Label{Text = "Nombres de los jugadores",Font = new Font("Segoe UI", 20, FontStyle.Bold),ForeColor = Texto,TextAlign = ContentAlignment.MiddleCenter,Location = new Point(20, 20),Size = new Size(400, 55)};

            panelNombres.Controls.Add(titulo);

            for (int i = 0; i < 3; i++)
            {
                var etiqueta = new Label{Text = "Jugador " + (i + 1),ForeColor = Texto,Font = new Font("Segoe UI", 12, FontStyle.Bold),Location = new Point(55, 85 + i * 80),Size = new Size(320, 25)};

                nombres[i] = new TextBox{Text = "",Font = new Font("Segoe UI", 14),ForeColor = Texto, BackColor = Color.FromArgb(255, 240, 245), BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center,Location = new Point(55, 112 + i * 80),Size = new Size(330, 36),MaxLength = 20};
                
                nombres[i].Enter += (s, e) =>
                {
                    ((TextBox)s).BackColor =
                        Color.FromArgb(255, 225, 235);
                };

                nombres[i].Leave += (s, e) =>
                {
                    ((TextBox)s).BackColor =
                        Color.FromArgb(255, 240, 245);
                };

                panelNombres.Controls.Add(etiqueta);
                panelNombres.Controls.Add(nombres[i]);
            }

            var comenzar = new BotonPastel{Text = "Comenzar",ColorBase = Rosa,ColorTexto = Texto,Size = new Size(180, 65),Location = new Point(35, 355)};

            var volver = new BotonPastel{Text = "Volver",ColorBase = Lila,ColorTexto = Texto,Size = new Size(180, 65),Location = new Point(225, 355)};


            comenzar.Click += (s, e) =>
            {
                string[] jugadores = new string[3];
                lblError.Text = "";
                for (int i = 0; i < 3; i++)
                {
                    jugadores[i] = nombres[i].Text.Trim();
                    if (string.IsNullOrWhiteSpace(jugadores[i]))
                        jugadores[i] = "Jugador " + (i + 1);
                }
                for (int i = 0; i < jugadores.Length; i++)
                {
                    for (int j = i + 1; j < jugadores.Length; j++)
                    {
                        if (string.Equals(
                            jugadores[i],
                            jugadores[j],
                            StringComparison.OrdinalIgnoreCase))
                        {
                            lblError.Text = "Los nombres no pueden repetirse";
                            nombres[j].Focus();
                            nombres[j].SelectAll();
                            return;
                        }
                    }
                }
                AbrirJuego(jugadores);
            };


            volver.Click += (s, e) =>
            {
                Controls.Remove(panelNombres);
                panelNombres.Dispose();
                panelNombres = null;
                btnJugar.Visible = true;
                btnHistorial.Visible = true;
                btnReglas.Visible = true;
                btnSalir.Visible = true;
            };
            panelNombres.Controls.Add(comenzar);
            panelNombres.Controls.Add(volver);
            Controls.Add(panelNombres);
            panelNombres.Location = new Point((ClientSize.Width - panelNombres.Width) / 2, (ClientSize.Height - panelNombres.Height) / 2);
            panelNombres.BringToFront();
        }
        void Acomodar()
        {
            int x = (ClientSize.Width - 320) / 2;
            int y = ClientSize.Height / 2 - 40;
            foreach (var b in new[] { btnJugar, btnHistorial, btnReglas, btnSalir })
            {
                b.Location = new Point(x, y);
                y += 98;
            }
            if (panelNombres != null)
            {
                panelNombres.Location = new Point((ClientSize.Width - panelNombres.Width) / 2,(ClientSize.Height - panelNombres.Height) / 2);
            }
        }

        void AbrirJuego(string[] jugadores)
        {
            var juego = new Form2(jugadores);
            juego.FormClosed += (s, e) =>
            {
                if (panelNombres != null)
                {
                    Controls.Remove(panelNombres);
                    panelNombres.Dispose();
                    panelNombres = null;
                }
                btnJugar.Visible = true;
                btnHistorial.Visible = true;
                btnReglas.Visible = true;
                btnSalir.Visible = true;
                Show();
            };
            Hide();
            juego.Show();
        }

        void AbrirHistorial()
        {
            MessageBox.Show("Aquí irá el historial de partidas.", "Historial");
        }

        void MostrarReglas()
        {
            MessageBox.Show(
                "• Cada jugador empieza con 7 cartas.\n" +
                "• Juega una carta del mismo color o número que la del centro.\n" +
                "• Si no puedes, roba una carta del mazo.\n" +
                "• Las cartas especiales (+2, +4, reversa, salto, color) cambian el juego.\n" +
                "• Cuando te quede una carta, ¡presiona UNO!\n" +
                "• Gana quien se quede sin cartas primero.",
                "Reglas");
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (var br = new LinearGradientBrush(ClientRectangle, RosaFondo1, RosaFondo2, 90f))
                e.Graphics.FillRectangle(br, ClientRectangle);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Decorar(e.Graphics, 0.10f, 0.20f, 90, Color.FromArgb(70, 255, 190, 170));
            Decorar(e.Graphics, 0.88f, 0.28f, 60, Color.FromArgb(70, 205, 190, 245));
            Decorar(e.Graphics, 0.15f, 0.80f, 70, Color.FromArgb(70, 160, 220, 190));
            Decorar(e.Graphics, 0.85f, 0.82f, 100, Color.FromArgb(70, 255, 214, 140));
        }

        void Decorar(Graphics g, float fx, float fy, int d, Color c)
        {
            using (var br = new SolidBrush(c))
                g.FillEllipse(br, ClientSize.Width * fx - d / 2f, ClientSize.Height * fy - d / 2f, d, d);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            string[] letras = { "U", "N", "O" };
            Color[] colores = { Rosa, Amarillo, Verde };
            using (var fuente = new Font("Segoe UI", 96, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                float ancho = 0;
                var tam = new SizeF[3];
                for (int i = 0; i < 3; i++)
                {
                    tam[i] = g.MeasureString(letras[i], fuente, 1000, StringFormat.GenericTypographic);
                    ancho += tam[i].Width + 6;
                }
                float x = (ClientSize.Width - ancho) / 2f;
                float y = ClientSize.Height / 2f - 260;
                for (int i = 0; i < 3; i++)
                {
                    using (var sombra = new SolidBrush(Color.FromArgb(60, 120, 70, 90)))
                        g.DrawString(letras[i], fuente, sombra, x + 4, y + 5, StringFormat.GenericTypographic);
                    using (var br = new SolidBrush(colores[i]))
                        g.DrawString(letras[i], fuente, br, x, y, StringFormat.GenericTypographic);
                    x += tam[i].Width + 6;
                }
            }

            var cap = new Rectangle((ClientSize.Width - 300) / 2, (int)(ClientSize.Height / 2f - 130), 300, 44);
            using (var path = BotonPastel.RoundedRect(cap, 22))
            using (var fondo = new SolidBrush(Color.FromArgb(255, 244, 200)))
            using (var borde = new Pen(Amarillo, 3))
            {
                g.FillPath(fondo, path);
                g.DrawPath(borde, path);
            }
            using (var f = new Font("Segoe UI", 15, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var br = new SolidBrush(Texto))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("¡Bienvenido!", f, br, cap, sf);
            }
        }
    }


    public class BotonPastel : Control
    {
        public Color ColorBase { get; set; } = Color.FromArgb(255, 160, 184);
        public Color ColorTexto { get; set; } = Color.FromArgb(125, 65, 95);

        bool hover, presionado;

        public BotonPastel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 26, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; presionado = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { presionado = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { presionado = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            int desplazamiento = presionado ? 3 : 0;
            var r = new Rectangle(4, 4 + desplazamiento, Width - 10, Height - 14);

            // Sombra
            var rs = new Rectangle(r.X, r.Y + 5 - desplazamiento, r.Width, r.Height);
            using (var p = RoundedRect(rs, r.Height / 2))
            using (var br = new SolidBrush(Color.FromArgb(50, 140, 80, 100)))
                g.FillPath(br, p);

            // Cuerpo (más claro al pasar el mouse)
            Color c = hover ? ControlPaint.Light(ColorBase, 0.25f) : ColorBase;
            using (var p = RoundedRect(r, r.Height / 2))
            {
                using (var br = new SolidBrush(c)) g.FillPath(br, p);
                using (var pen = new Pen(Color.White, 4)) g.DrawPath(pen, p);
            }

            // Texto
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using (var br = new SolidBrush(ColorTexto))
                g.DrawString(Text, Font, br, r, sf);
        }

        public static GraphicsPath RoundedRect(Rectangle r, int radio)
        {
            int d = radio * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
