using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{

    internal static class ImagenesUno
    {
        static readonly Dictionary<string, Image> cache = new Dictionary<string, Image>();
        static string carpeta;
        static bool buscada;

        static string BuscarCarpeta()
        {
            if (buscada) return carpeta;
            buscada = true;
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 0; i < 6 && dir != null; i++)
            {
                string c = Path.Combine(dir, "assets");
                if (Directory.Exists(c)) { carpeta = c; break; }
                DirectoryInfo padre = Directory.GetParent(dir);
                dir = padre != null ? padre.FullName : null;
            }
            return carpeta;
        }

        static string Prefijo(string color, bool conjuntoAmarilloOtro)
        {
            switch (color)
            {
                case "Rojo": return "rojo";
                case "Azul": return "azul";
                case "Amarillo": return "amarilla";
                default: return "verde";
            }
        }

        public static string NombreArchivo(Carta c)
        {
            int v = c.getValor();
            string col = c.getColor();
            if (v == 13) return "cambio_color.png";
            if (v == 14) return "+4.png";
            switch (v)
            {
                case 10:
                    return (col == "Rojo" ? "roja" : Prefijo(col, false)) + "+2.png";
                case 11:
                    return Prefijo(col, false) + "_reversa.png";
                case 12:
                    return (col == "Amarillo" ? "amarillo" : Prefijo(col, false)) + "_stop.png";
                default:
                    return Prefijo(col, false) + v + ".png";
            }
        }

        public static Image Obtener(Carta c)
        {
            string nombre = NombreArchivo(c);
            Image img;
            if (cache.TryGetValue(nombre, out img)) return img;

            img = null;
            string dir = BuscarCarpeta();
            if (dir != null)
            {
                string ruta = Path.Combine(dir, nombre);
                if (File.Exists(ruta))
                {
                    try
                    {
                        using (var fs = File.OpenRead(ruta))
                        using (var original = Image.FromStream(fs))
                        {
                            var bmp = new Bitmap(300, 420);
                            using (var g = Graphics.FromImage(bmp))
                            {
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                g.DrawImage(original, 0, 0, 300, 420);
                            }
                            img = bmp;
                        }
                    }
                    catch { img = null; }
                }
            }
            cache[nombre] = img; 
            return img;
        }
        public static Image ObtenerReverso()
        {
            string nombre = "reverso.png";
            Image img;
            if (cache.TryGetValue(nombre, out img))
                return img;
            img = null;
            string dir = BuscarCarpeta();
            if (dir != null)
            {
                string ruta = Path.Combine(dir, nombre);
                if (File.Exists(ruta))
                {
                    try
                    {
                        using (var fs = File.OpenRead(ruta))
                        using (var original = Image.FromStream(fs))
                        {
                            var bmp = new Bitmap(300, 420);
                            using (var g = Graphics.FromImage(bmp))
                            {
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                g.DrawImage(original, 0, 0, 300, 420);
                            }
                            img = bmp;
                        }
                    }
                    catch
                    {
                        img = null;
                    }
                }
            }
            cache[nombre] = img;
            return img;
        }
    }

    internal class Tablero : Control
    {
        class Zona
        {
            public RectangleF R;
            public Action Accion;
            public int Indice = -1;
        }

        public Partida Partida;
        public int[] Puntos = new int[3];        
        public bool SentidoHorario = true;
        public bool MostrarSelectorColor = false;
        public bool Silencio = false;
        public bool PasarDisponible = false;
        public string Mensaje = "";
        public bool MostrarVictoria = false;
        public bool MostrarMenu = false;
        public string NombreGanador = "";
        public bool AcusarUnoDisponible = false;

        public event Action AcusarUnoClick;
        public event Action VolverAJugarClick;
        public event Action IrMenuClick;
        public event Action<Carta> CartaClick;
        public event Action RobarClick;
        public event Action UnoClick;
        public event Action SalirClick;
        public event Action<string> ColorElegido;
        public event Action PasarClick;

        static readonly Color Crema = Color.FromArgb(255, 240, 225);
        static readonly Color Rosa = Color.FromArgb(255, 214, 224);
        static readonly Color RosaFuerte = Color.FromArgb(255, 160, 185);
        static readonly Color Coral = Color.FromArgb(255, 171, 145);
        static readonly Color Mantequilla = Color.FromArgb(255, 240, 181);
        static readonly Color Menta = Color.FromArgb(200, 238, 214);
        static readonly Color Lavanda = Color.FromArgb(224, 208, 244);
        static readonly Color Cafe = Color.FromArgb(135, 85, 100);

        static readonly StringFormat Centro = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        readonly List<Zona> zonas = new List<Zona>();
        readonly Timer timerMensaje = new Timer();
        Point mouse = new Point(-1000, -1000);
        int hoverI = -1;

        public Tablero()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            timerMensaje.Interval = 2400;
            timerMensaje.Tick += (s, e) => { timerMensaje.Stop(); Mensaje = ""; Invalidate(); };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) timerMensaje.Dispose();
            base.Dispose(disposing);
        }

        public void MostrarMensaje(string texto)
        {
            Mensaje = texto;
            timerMensaje.Stop();
            timerMensaje.Start();
            Invalidate();
        }

        Zona BuscarZona()
        {
            for (int i = zonas.Count - 1; i >= 0; i--)
                if (zonas[i].R.Contains(mouse)) return zonas[i];
            return null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mouse = e.Location;
            Zona z = BuscarZona();
            hoverI = (z != null) ? z.Indice : -1;
            Cursor = z != null ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mouse = new Point(-1000, -1000);
            hoverI = -1;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Zona z = BuscarZona();
            if (z != null && z.Accion != null) z.Accion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            float W = ClientSize.Width, H = ClientSize.Height;
            if (W < 100 || H < 100) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            zonas.Clear();

            float cw = Math.Max(60f, H * 0.115f);
            float ch = cw * 1.4f;

            DibujarFondo(g, W, H);
            DibujarTitulo(g, W, H);

            if (Partida != null)
            {
                DibujarMesa(g, W, H, cw, ch);
                for (int seat = 0; seat < 3; seat++) DibujarMano(g, seat, W, H, cw, ch);
                DibujarPastillas(g, W, H, cw, ch);
                if (MostrarSelectorColor) DibujarSelectorColor(g, W, H);
                DibujarMensaje(g, W, H);
            }
            DibujarBotones(g, W, H);
            if (MostrarVictoria || MostrarMenu)
            {
                zonas.Clear();
                if (MostrarVictoria)
                    DibujarPantallaVictoria(g, W, H);
                else
                    DibujarPantallaMenu(g, W, H);
            }
        }

        void DibujarPantallaVictoria(Graphics g, float W, float H)
        {
            using (var b = new SolidBrush(Color.FromArgb(165, 65, 45, 60)))
                g.FillRectangle(b, 0, 0, W, H);
            float pw = Math.Min(620f, W * 0.70f);
            float ph = Math.Min(430f, H * 0.72f);
            var panel = new RectangleF((W - pw) / 2, (H - ph) / 2, pw, ph);
            using (var sombra = Redondo(new RectangleF(panel.X + 6, panel.Y + 8, panel.Width, panel.Height), 28f))
            using (var bs = new SolidBrush(Color.FromArgb(45, Cafe)))
            {
                g.FillPath(bs, sombra);
            }
            using (var p = Redondo(panel, 28f))
            {
                using (var b = new SolidBrush(Color.FromArgb(255, 250, 245)))
                    g.FillPath(b, p);

                using (var pen = new Pen(RosaFuerte, 4))
                    g.DrawPath(pen, p);
            }
            using (var titulo = new Font("Segoe UI", 30, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                Texto(g, "Fin de la partida", titulo, Cafe, new RectangleF(panel.X, panel.Y + 28, pw, 48));
            }
            using (var nombre = new Font("Segoe UI", 27, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                Texto(g, NombreGanador, nombre, RosaFuerte, new RectangleF(panel.X, panel.Y + 115, pw, 42));
            }

            using (var subtitulo = new Font("Segoe UI", 17, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                Texto(g, "Estadísticas", subtitulo, Cafe, new RectangleF(panel.X, panel.Y + 165, pw, 28));
            }
            using (var fuente = new Font("Segoe UI", 16, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                for (int i = 0; i < Partida.getJugadores().Count; i++)
                {
                    Jugador jugador = Partida.getJugador(i);
                    int cartas = jugador.getCartas().Count;
                    int victorias = Puntos[i];
                    string resultado = jugador.getNombre() + "  |  " + cartas + " cartas" + "  |  " + victorias + " victorias";
                    Color fondo = jugador.getNombre() == NombreGanador ? Mantequilla : Color.FromArgb(245, 230, 235);

                    var fila = new RectangleF(panel.X + 35, panel.Y + 200 + i * 43, pw - 70, 36);

                    Capsula(g, fila, fondo, Rosa, 1);
                    Texto(g, resultado, fuente, Cafe, fila);
                }
            }
            float bw = (pw - 85) / 2;
            float bh = 48f;
            float by = panel.Bottom - 75;

            DibujarBotonPanel(g, new RectangleF(panel.X + 30, by, bw, bh), "Volver a jugar", Coral, () => VolverAJugarClick?.Invoke());

            DibujarBotonPanel(g, new RectangleF(panel.X + 55 + bw, by, bw, bh), "Menú", Lavanda, () => IrMenuClick?.Invoke());
        }
        void DibujarBotonPanel(Graphics g, RectangleF r, string texto, Color color, Action accion)
        {
            bool hover = r.Contains(mouse);
            Color fondo = hover ? ControlPaint.Light(color) : color;
            using (var p = Redondo(r, 20))
            {
                using (var b = new SolidBrush(fondo))
                    g.FillPath(b, p);

                using (var pen = new Pen(Color.White, 3))
                    g.DrawPath(pen, p);
            }

            using (var f = new Font("Segoe UI", 16, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                Texto(g, texto, f, Cafe, r);
            }
            zonas.Add(new Zona{ R = r, Accion = accion });
        }
        void DibujarPantallaMenu(Graphics g, float W, float H)
        {
            using (var b = new SolidBrush(Color.FromArgb(180, 65, 45, 60)))
                g.FillRectangle(b, 0, 0, W, H);

            float pw = 400;
            float ph = 280;

            var panel = new RectangleF((W - pw) / 2, (H - ph) / 2, pw, ph);

            Capsula(g, panel, Crema, RosaFuerte, 4);

            using (var f = new Font("Segoe UI", 45, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                Texto(g, "UNO", f, Coral, new RectangleF(panel.X, panel.Y + 25, pw, 75));
            }

            DibujarBotonPanel(g, new RectangleF(panel.X + 75, panel.Y + 125, 250, 50), "Jugar", Menta, () => VolverAJugarClick?.Invoke());
            DibujarBotonPanel(g, new RectangleF(panel.X + 75, panel.Y + 190, 250, 50), "Salir", Lavanda, () => SalirClick?.Invoke());
        }

        Jugador JugadorEnAsiento(int seat)
        {
            return Partida.getJugador(seat);
        }

        int IndiceEnAsiento(int seat)
        {
            return seat;
        }


        void DibujarFondo(Graphics g, float W, float H)
        {
            using (var br = new LinearGradientBrush(new RectangleF(0, 0, W, H), Crema, Rosa, 60f))
                g.FillRectangle(br, 0, 0, W, H);

            var rnd = new Random(11);
            Color[] pal = { Color.White, Rosa, Mantequilla, Menta, Lavanda, Coral };
            for (int i = 0; i < 36; i++)
            {
                float x = (float)rnd.NextDouble() * W;
                float y = (float)rnd.NextDouble() * H;
                float s = 14 + rnd.Next(0, 34);
                Color c = Color.FromArgb(80, pal[rnd.Next(pal.Length)]);
                int tipo = rnd.Next(3);
                using (var b = new SolidBrush(c))
                {
                    if (tipo == 0) g.FillEllipse(b, x, y, s, s);
                    else if (tipo == 1)
                        using (var p = Corazon(new RectangleF(x, y, s, s * 0.9f))) g.FillPath(b, p);
                    else
                        using (var p = Brillo(x, y, s * 0.6f)) g.FillPath(b, p);
                }
            }
        }

        void DibujarTitulo(Graphics g, float W, float H)
        {
            float fs = Math.Max(30f, H * 0.075f);
            Color[] cols = { RosaFuerte, Color.FromArgb(255, 205, 120), Color.FromArgb(150, 215, 175) };
            string letras = "UNO";
            using (var f = new Font("Segoe UI", fs, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                float x = 22;
                for (int i = 0; i < 3; i++)
                {
                    string l = letras.Substring(i, 1);
                    SizeF sz = g.MeasureString(l, f);
                    using (var sh = new SolidBrush(Color.FromArgb(70, Cafe)))
                        g.DrawString(l, f, sh, x + 3, 13);
                    using (var b = new SolidBrush(cols[i]))
                        g.DrawString(l, f, b, x, 10);
                    x += sz.Width * 0.78f;
                }
            }
            //using (var p = Corazon(new RectangleF(22 + fs * 2.4f, 18, fs * 0.4f, fs * 0.36f)))
            ///using (var b = new SolidBrush(RosaFuerte))
                //g.FillPath(b, p);
        }

        void DibujarMesa(Graphics g, float W, float H, float cw, float ch)
        {
            float cx = W / 2, cy = H * 0.45f;
            float mw = W * 0.46f, mh = H * 0.46f;
            var mesa = new RectangleF(cx - mw / 2, cy - mh / 2, mw, mh);

            using (var b = new SolidBrush(Color.FromArgb(110, 255, 255, 255)))
                g.FillEllipse(b, mesa);
            using (var p = new Pen(Color.FromArgb(190, 255, 255, 255), 4) { DashStyle = DashStyle.Dot })
                g.DrawEllipse(p, mesa);
            DibujarIndicadorDireccion(g, mesa, SentidoHorario);
            string turno = "Turno de " + Partida.getJugadorActual().getNombre();
            using (var f = new Font("Segoe UI", Math.Max(14f, H * 0.032f), FontStyle.Bold, GraphicsUnit.Pixel))
            {
                SizeF sz = g.MeasureString(turno, f);
                var r = new RectangleF(cx - sz.Width / 2 - 22, 18, sz.Width + 44, sz.Height + 14);
                Capsula(g, r, Mantequilla, Coral, 3);
                Texto(g, turno, f, Cafe, r);
            }

            var mazo = new RectangleF(cx - cw - 14, cy - ch / 2, cw, ch);
            float ps = Math.Max(55f, H * 0.09f);
            Color colorPasar = PasarDisponible ? Lavanda : Color.FromArgb(175, 175, 175);
            Boton( g, new RectangleF(mazo.Left - ps - 22, mazo.Top + (mazo.Height - ps) / 2, ps, ps), colorPasar, "Pasar", PasarDisponible ? (Action)(() => PasarClick?.Invoke()) : null, PasarDisponible ? Cafe : Color.LightGray, ps * 0.24f, false);
            for (int i = 2; i >= 1; i--)
                DibujarReverso(g, new RectangleF(mazo.X - i * 3, mazo.Y - i * 3, cw, ch));
            DibujarReverso(g, mazo);
            zonas.Add(new Zona { R = mazo, Accion = () => { if (RobarClick != null) RobarClick(); } });
            using (var f = new Font("Segoe UI", Math.Max(11f, H * 0.022f), FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, "Robar (" + Partida.getMazo().getMazoSize() + ")", f, Cafe,
                      new RectangleF(mazo.X - 20, mazo.Bottom + 6, cw + 40, 22)); 

            var desc = new RectangleF(cx + 14, cy - ch / 2, cw, ch);
            Carta ultima = Partida.getJuego().getUltimaCarta();
            if (ultima != null) DibujarCara(g, desc, ultima);
            else
                using (var p = Redondo(desc, cw * 0.1f))
                using (var pen = new Pen(Color.FromArgb(200, 255, 255, 255), 3) { DashStyle = DashStyle.Dash })
                    g.DrawPath(pen, p);

            /*float hs = Math.Max(40f, H * 0.08f);
            var rc = new RectangleF(desc.Right + 26, cy - hs / 2 - 10, hs, hs * 0.9f);
            using (var p = Corazon(rc))
            {
                using (var b = new SolidBrush(ColorDe(Partida.getColorActual()))) g.FillPath(b, p);
                using (var pen = new Pen(Color.White, 4)) g.DrawPath(pen, p);
            }
            using (var f = new Font("Segoe UI", Math.Max(11f, H * 0.02f), FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, "Color", f, Cafe, new RectangleF(rc.X - 10, rc.Bottom + 10, hs + 20, 20));
            */
            float hs = ps;
            float separacion = 22f;
            var rc = new RectangleF(desc.Right + separacion, desc.Top + (desc.Height - hs) / 2, hs, hs);
            using (var b = new SolidBrush(ColorDe(Partida.getColorActual())))
                g.FillEllipse(b, rc);
            using (var pen = new Pen(Color.White, 4))
                g.DrawEllipse(pen, rc);
            /*using (var f = new Font("Segoe UI", Math.Max(11f, H * 0.02f), FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, "Color", f, Cafe, new RectangleF(rc.X - 10, rc.Bottom + 10, hs + 20, 20)); */
        }


        void DibujarMano(Graphics g, int seat, float W, float H, float cw, float ch)
        {
            List<Carta> mano = JugadorEnAsiento(seat).getCartas();
            int n = mano.Count;
            if (n == 0) return;

            bool turnoActivo = seat == Partida.getTurnoActual();
            float elevacion = 22f;

            for (int i = 0; i < n; i++)
            {
                Carta carta = mano[i];

                bool jugable = turnoActivo && !MostrarSelectorColor && Partida.sePuedeJugar(carta);

                bool hov = turnoActivo && !MostrarSelectorColor && hoverI == i;

                RectangleF bounds;
                float ang = 0;

                if (seat == 0)
                {
                    float step = n > 1 ? Math.Min(cw * 0.8f, (W * 0.6f - cw) / (n - 1)) : 0;

                    float total = cw + step * (n - 1);
                    float x0 = (W - total) / 2;

                    bounds = new RectangleF( x0 + i * step, H - ch - 26, cw, ch );

                    if (jugable)
                        bounds.Y -= elevacion;

                    if (hov)
                        bounds.Y -= 12f;
                }
                else
                {
                    float step = n > 1 ? Math.Min(cw * 0.5f, (H * 0.55f - cw) / (n - 1)) : 0;

                    float total = cw + step * (n - 1);
                    float y0 = H * 0.5f - total / 2 + 30;

                    float cx = seat == 1 ? 16 + ch / 2 : W - 16 - ch / 2;

                    float cy = y0 + cw / 2 + i * step;

                    bounds = new RectangleF( cx - ch / 2, cy - cw / 2, ch, cw);

                    ang = seat == 1 ? 90 : -90;

                    if (jugable)
                    {
                        if (seat == 1)
                            bounds.X += elevacion;
                        else
                            bounds.X -= elevacion;
                    }
                    if (hov)
                    {
                        if (seat == 1)
                            bounds.X += 12f;
                        else
                            bounds.X -= 12f;
                    }
                }
                Rotar(g, bounds, ang, r => { DibujarCara(g, r, carta); });
                if (turnoActivo && !MostrarSelectorColor)
                {
                    int ii = i;
                    zonas.Add(new Zona { R = bounds, Indice = ii, Accion = () => { if (CartaClick != null) CartaClick(carta); } });
                }
            }
        }

        void DibujarPastillas(Graphics g, float W, float H, float cw, float ch)
        {
            float pw = Math.Max(150f, W * 0.15f), ph = Math.Max(48f, H * 0.075f);
            RectangleF[] rs =
            {
                new RectangleF((W - pw) / 2, H - ch - 26 - ph - 14, pw, ph),
                new RectangleF(12, H * 0.17f, pw, ph),
                new RectangleF(W - 12 - pw, H * 0.17f, pw, ph)
            };
            using (var f1 = new Font("Segoe UI", ph * 0.34f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var f2 = new Font("Segoe UI", ph * 0.26f, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                for (int seat = 0; seat < 3; seat++)
                {
                    Jugador jug = JugadorEnAsiento(seat);
                    int idx = IndiceEnAsiento(seat);
                    bool activo = idx == Partida.getTurnoActual();
                    Capsula(g, rs[seat], activo ? Mantequilla : Color.FromArgb(215, 255, 255, 255),
                            activo ? Coral : RosaFuerte, activo ? 4 : 2);
                    var top = new RectangleF(rs[seat].X, rs[seat].Y + 2, rs[seat].Width, rs[seat].Height * 0.52f);
                    var bot = new RectangleF(rs[seat].X, rs[seat].Y + rs[seat].Height * 0.48f, rs[seat].Width, rs[seat].Height * 0.5f);
                    int pts = idx < Puntos.Length ? Puntos[idx] : 0;
                    Texto(g, jug.getNombre(), f1, Cafe, top);
                    Texto(g, jug.getCartas().Count + " cartas   ★ " + pts, f2, Cafe, bot);
                }
            }
        }

        void DibujarSelectorColor(Graphics g, float W, float H)
        {
            float cx = W / 2, cy = H * 0.45f;
            float d = Math.Max(54f, H * 0.11f);
            float gap = d * 0.35f;
            float total = d * 4 + gap * 3;
            var panel = new RectangleF(cx - total / 2 - 30, cy - d / 2 - 60, total + 60, d + 100);

            using (var p = Redondo(panel, 28))
            {
                using (var b = new SolidBrush(Color.FromArgb(235, 255, 250, 245))) g.FillPath(b, p);
                using (var pen = new Pen(RosaFuerte, 4)) g.DrawPath(pen, p);
            }
            using (var f = new Font("Segoe UI", Math.Max(14f, H * 0.03f), FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, "Elige un color", f, Cafe, new RectangleF(panel.X, panel.Y + 8, panel.Width, 40));

            string[] nombres = { "Rojo", "Amarillo", "Verde", "Azul" };
            for (int i = 0; i < 4; i++)
            {
                string nombre = nombres[i];
                var r = new RectangleF(cx - total / 2 + i * (d + gap), cy - d / 2 + 8, d, d);
                bool h = r.Contains(mouse);
                var rr = h ? RectangleF.Inflate(r, 4, 4) : r;
                using (var b = new SolidBrush(Color.FromArgb(50, Cafe)))
                    g.FillEllipse(b, rr.X + 2, rr.Y + 4, rr.Width, rr.Height);
                using (var b = new SolidBrush(ColorDe(nombre))) g.FillEllipse(b, rr);
                using (var pen = new Pen(Color.White, 4)) g.DrawEllipse(pen, rr);
                zonas.Add(new Zona { R = r, Accion = () => { if (ColorElegido != null) ColorElegido(nombre); } });
            }
        }

        void DibujarMensaje(Graphics g, float W, float H)
        {
            if (string.IsNullOrEmpty(Mensaje)) return;
            using (var f = new Font("Segoe UI", Math.Max(14f, H * 0.03f), FontStyle.Bold, GraphicsUnit.Pixel))
            {
                SizeF sz = g.MeasureString(Mensaje, f);
                float x = 20f;
                float y = H * 0.14f;
                var r = new RectangleF(W / 2 - sz.Width / 2 - 24, H * 0.14f, sz.Width + 48, sz.Height + 16);
                //var r = new RectangleF(x, y, sz.Width + 48, sz.Height + 16);
                Capsula(g, r, Color.FromArgb(245, 255, 255, 255), RosaFuerte, 3);
                Texto(g, Mensaje, f, Cafe, r);
            }
        }
        void DibujarIndicadorDireccion(Graphics g, RectangleF mesa, bool horario)
        {
            RectangleF r = RectangleF.Inflate(mesa, -40f, -25f);

            using (var pen = new Pen(Coral, 16f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                if (horario)
                {
                    DibujarArcoConFlecha(g, pen, r, 205f, 95f);
                    DibujarArcoConFlecha(g, pen, r, 25f, 95f);
                }
                else
                {
                    DibujarArcoConFlecha(g, pen, r, 300f, -95f);
                    DibujarArcoConFlecha(g, pen, r, 120f, -95f);
                }
            }
        }

        void DibujarArcoConFlecha(Graphics g, Pen pen, RectangleF r, float startAngle, float sweepAngle)
        {
            g.DrawArc(pen, r, startAngle, sweepAngle);

            float endAngle = startAngle + sweepAngle;
            PointF punta = PuntoEnElipse(r, endAngle);

            PointF tangente = TangenteEnElipse(r, endAngle, sweepAngle);
            float len = (float)Math.Sqrt(tangente.X * tangente.X + tangente.Y * tangente.Y);
            if (len == 0) return;

            tangente = new PointF(tangente.X / len, tangente.Y / len);

            PointF atras = new PointF(-tangente.X, -tangente.Y);
            PointF normal = new PointF(-tangente.Y, tangente.X);

            float tam = 30f;

            PointF p1 = new PointF(
                punta.X + atras.X * tam + normal.X * (tam * 0.65f),
                punta.Y + atras.Y * tam + normal.Y * (tam * 0.65f)
            );

            PointF p2 = new PointF(
                punta.X + atras.X * tam - normal.X * (tam * 0.65f),
                punta.Y + atras.Y * tam - normal.Y * (tam * 0.65f)
            );

            using (var b = new SolidBrush(Coral))
            {
                g.FillPolygon(b, new[] { punta, p1, p2 });
            }
        }

        PointF PuntoEnElipse(RectangleF r, float anguloGrados)
        {
            double a = r.Width / 2.0;
            double b = r.Height / 2.0;
            double cx = r.X + a;
            double cy = r.Y + b;
            double t = anguloGrados * Math.PI / 180.0;

            return new PointF(
                (float)(cx + a * Math.Cos(t)),
                (float)(cy + b * Math.Sin(t))
            );
        }
        PointF TangenteEnElipse(RectangleF r, float anguloGrados, float sweepAngle)
        {
            double a = r.Width / 2.0;
            double b = r.Height / 2.0;
            double t = anguloGrados * Math.PI / 180.0;
            float dx = (float)(-a * Math.Sin(t));
            float dy = (float)(b * Math.Cos(t));
            if (sweepAngle < 0)
            {
                dx = -dx;
                dy = -dy;
            }

            return new PointF(dx, dy);
        }
        void DibujarBotones(Graphics g, float W, float H)
        {
            float bs = Math.Max(40f, H * 0.065f);

            Boton(g, new RectangleF(W - bs - 16, 14, bs, bs), Color.FromArgb(255, 160, 175), "✕",
                  () => { if (SalirClick != null) SalirClick(); }, Color.White, bs * 0.5f, false);

            /*Boton(g, new RectangleF(W - bs * 2 - 28, 14, bs, bs), Lavanda, "♪",
                  () => { Silencio = !Silencio; Invalidate(); },
                  Color.White, bs * 0.55f, Silencio); */

            float us = Math.Max(80f, H * 0.14f);
            bool unoDisponible = Partida != null && Partida.getJugadorActual().getCartas().Count == 2;
            Color colorBoton = unoDisponible ? Coral : Color.FromArgb(175, 175, 175);
            Color colorTexto = unoDisponible ? Color.White : Color.FromArgb(225, 225, 225);
            Boton(g, new RectangleF(W - us - 20, H - us - 20, us, us), colorBoton, "UNO!", unoDisponible ? (Action)(() => { if (UnoClick != null) UnoClick(); }) : null, colorTexto, us * 0.26f, false);
            /*float ps = Math.Max(65f, H * 0.10f);
            Color colorPasar = PasarDisponible ? Lavanda : Color.FromArgb(175, 175, 175);
            Boton(g, new RectangleF( W - ps - 20, H - us - ps - 40, ps, ps), colorPasar, "Pasar", PasarDisponible ? (Action)(() => PasarClick?.Invoke()) : null, PasarDisponible ? Cafe : Color.LightGray, ps * 0.24f, false); */
        }

        void Boton(Graphics g, RectangleF r, Color fill, string txt, Action accion,
                   Color colTxt, float fs, bool tachado)
        {
            bool h = accion != null && r.Contains(mouse);
            RectangleF rr = h ? RectangleF.Inflate(r, 3, 3) : r;

            using (var b = new SolidBrush(Color.FromArgb(50, Cafe)))
                g.FillEllipse(b, rr.X + 2, rr.Y + 4, rr.Width, rr.Height);
            using (var b = new SolidBrush(fill)) g.FillEllipse(b, rr);
            using (var p = new Pen(Color.White, 3)) g.DrawEllipse(p, rr);
            using (var f = new Font("Segoe UI", fs, FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, txt, f, colTxt, rr);
            if (tachado)
                using (var p = new Pen(Color.White, 4) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                    g.DrawLine(p, rr.X + rr.Width * 0.22f, rr.Y + rr.Height * 0.2f,
                                  rr.Right - rr.Width * 0.22f, rr.Bottom - rr.Height * 0.2f);
            if(accion != null)
                zonas.Add(new Zona { R = r, Accion = accion });
        }

        void Rotar(Graphics g, RectangleF bounds, float ang, Action<RectangleF> dibuja)
        {
            if (ang == 0) { dibuja(bounds); return; }
            GraphicsState s = g.Save();
            float cx = bounds.X + bounds.Width / 2, cy = bounds.Y + bounds.Height / 2;
            g.TranslateTransform(cx, cy);
            g.RotateTransform(ang);
            dibuja(new RectangleF(-bounds.Height / 2, -bounds.Width / 2, bounds.Height, bounds.Width));
            g.Restore(s);
        }

        void Sombra(Graphics g, RectangleF r)
        {
            var s = new RectangleF(r.X + 2, r.Y + 4, r.Width, r.Height);
            using (var p = Redondo(s, r.Width * 0.12f))
            using (var b = new SolidBrush(Color.FromArgb(55, 150, 90, 110)))
                g.FillPath(b, p);
        }

        void DibujarCara(Graphics g, RectangleF r, Carta c)
        {
            Sombra(g, r);
            Image img = ImagenesUno.Obtener(c);
            if (img != null)
            {
                g.DrawImage(img, r);
                return;
            }

            float rad = r.Width * 0.12f;
            bool comodin = c.getValor() >= 13;
            Color col = comodin ? ColorDe("Comodin") : ColorDe(c.getColor());
            using (var p = Redondo(r, rad))
            {
                using (var b = new SolidBrush(col)) g.FillPath(b, p);
                using (var pen = new Pen(Color.White, 3.5f)) g.DrawPath(pen, p);
            }

            var ov = new RectangleF(r.X + r.Width * 0.14f, r.Y + r.Height * 0.2f, r.Width * 0.72f, r.Height * 0.6f);
            if (comodin)
            {
                string[] q = { "Rojo", "Amarillo", "Verde", "Azul" };
                for (int i = 0; i < 4; i++)
                    using (var b = new SolidBrush(ColorDe(q[i]))) g.FillPie(b, ov.X, ov.Y, ov.Width, ov.Height, i * 90, 90);
                using (var pen = new Pen(Color.White, 3)) g.DrawEllipse(pen, ov);
            }
            else
            {
                using (var b = new SolidBrush(Color.FromArgb(240, 255, 255, 255))) g.FillEllipse(b, ov);
            }

            string t = TextoValor(c.getValor());
            Color oscuro = comodin ? Cafe : Oscurecer(col, 0.6f);
            using (var f = new Font("Segoe UI", r.Height * (t.Length > 1 ? 0.24f : 0.32f), FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, t, f, oscuro, ov);

            using (var f = new Font("Segoe UI", r.Height * 0.12f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                Texto(g, t, f, Color.White, new RectangleF(r.X + 2, r.Y + 3, r.Width * 0.38f, r.Height * 0.18f));
                Texto(g, t, f, Color.White, new RectangleF(r.Right - r.Width * 0.38f - 2, r.Bottom - r.Height * 0.18f - 3, r.Width * 0.38f, r.Height * 0.18f));
            }
        }
        void DibujarReverso(Graphics g, RectangleF r)
        {
            Sombra(g, r);
            Image img = ImagenesUno.ObtenerReverso();
            if (img != null)
            {
                g.DrawImage(img, r);
                return;
            }
        }
        public static Color ColorDe(string c)
        {
            switch (c)
            {
                case "Rojo": return Color.FromArgb(255, 154, 162);
                case "Amarillo": return Color.FromArgb(255, 224, 140);
                case "Verde": return Color.FromArgb(170, 228, 190);
                case "Azul": return Color.FromArgb(160, 204, 248);
                default: return Color.FromArgb(205, 185, 235);
            }
        }

        static Color Oscurecer(Color c, float f)
        {
            return Color.FromArgb((int)(c.R * f), (int)(c.G * f), (int)(c.B * f));
        }

        static string TextoValor(int v)
        {
            switch (v)
            {
                case 10: return "+2";
                case 11: return "⇄";
                case 12: return "⊘";
                case 13: return "★";
                case 14: return "+4";
                default: return v.ToString();
            }
        }

        void Texto(Graphics g, string t, Font f, Color c, RectangleF r)
        {
            using (var b = new SolidBrush(c)) g.DrawString(t, f, b, r, Centro);
        }

        void Capsula(Graphics g, RectangleF r, Color fill, Color borde, float grosor)
        {
            using (var p = Redondo(r, r.Height / 2))
            {
                using (var s = new SolidBrush(Color.FromArgb(40, Cafe)))
                using (var ps = Redondo(new RectangleF(r.X + 2, r.Y + 3, r.Width, r.Height), r.Height / 2))
                    g.FillPath(s, ps);
                using (var b = new SolidBrush(fill)) g.FillPath(b, p);
                using (var pen = new Pen(borde, grosor)) g.DrawPath(pen, p);
            }
        }

        static GraphicsPath Redondo(RectangleF r, float rad)
        {
            float d = rad * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        static GraphicsPath Corazon(RectangleF r)
        {
            float w = r.Width, h = r.Height;
            var p = new GraphicsPath();
            p.AddBezier(r.X + w / 2, r.Y + h,
                        r.X - w * 0.25f, r.Y + h * 0.45f,
                        r.X + w * 0.2f, r.Y - h * 0.2f,
                        r.X + w / 2, r.Y + h * 0.28f);
            p.AddBezier(r.X + w / 2, r.Y + h * 0.28f,
                        r.X + w * 0.8f, r.Y - h * 0.2f,
                        r.X + w * 1.25f, r.Y + h * 0.45f,
                        r.X + w / 2, r.Y + h);
            p.CloseFigure();
            return p;
        }

        static GraphicsPath Brillo(float cx, float cy, float r)
        {
            var pts = new PointF[8];
            for (int i = 0; i < 8; i++)
            {
                double a = Math.PI / 4 * i - Math.PI / 2;
                float rr = i % 2 == 0 ? r : r * 0.28f;
                pts[i] = new PointF(cx + (float)Math.Cos(a) * rr, cy + (float)Math.Sin(a) * rr);
            }
            var p = new GraphicsPath();
            p.AddPolygon(pts);
            return p;
        }
    }


    public partial class Form2 : Form
    {
        Tablero mesa;
        Partida partida;
        bool unoDeclarado = false;
        Carta cartaRobadaPendiente = null;
        Jugador jugadorSinUno = null;

        public Form2()
        {
            InitializeComponent();

            Text = "UNO";
            ClientSize = new Size(1100, 720);
            MinimumSize = new Size(900, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(255, 230, 225);
            DoubleBuffered = true;

            mesa = new Tablero { Dock = DockStyle.Fill };
            Controls.Add(mesa);

            mesa.SalirClick += () => Close();
            mesa.CartaClick += AlHacerClicEnCarta;
            mesa.RobarClick += AlRobar;
            mesa.UnoClick += AlPresionarUno;
            mesa.ColorElegido += AlElegirColor;
            mesa.PasarClick += AlPasar;
            mesa.VolverAJugarClick += NuevaPartida;
            mesa.IrMenuClick += () =>
            {
                mesa.MostrarVictoria = false;
                mesa.MostrarMenu = false;
                mesa.Invalidate();
            };
            NuevaPartida();
        }

        void NuevaPartida()
        {
            partida = new Partida();
            partida.repartir();
            partida.iniciaDescarte();
            unoDeclarado = false;

            mesa.Partida = partida;
            mesa.SentidoHorario = true;
            mesa.MostrarSelectorColor = false;
            cartaRobadaPendiente = null;
            mesa.PasarDisponible = false;
            mesa.MostrarMenu = false;
            mesa.MostrarVictoria = false;
            mesa.NombreGanador = "";
            mesa.Invalidate();
        }

        void AlHacerClicEnCarta(Carta carta)
        {
            if(cartaRobadaPendiente != null && carta != cartaRobadaPendiente)
            {
                mesa.MostrarMensaje("Después de robar solo puedes jugar la carta robada");
                return;
            }
            if (mesa.MostrarSelectorColor) return;

            if (!partida.sePuedeJugar(carta))
            {
                mesa.MostrarMensaje("Esa carta no se puede jugar");
                return;
            }

            Jugador quienJuega = partida.getJugadorActual();
            partida.jugarCarta(carta);
            cartaRobadaPendiente = null;
            mesa.PasarDisponible = false;

            if (carta.getValor() == 11) mesa.SentidoHorario = !mesa.SentidoHorario;

            if (partida.hayGanador())
            {
                TerminarPartida();
                return;
            }

            if (quienJuega.uno() && !unoDeclarado)
            {
                jugadorSinUno = quienJuega;
            } else
            {
                jugadorSinUno = null;
            }
            mesa.AcusarUnoDisponible = jugadorSinUno != null;
            unoDeclarado = false;

            if (partida.necesitaElegirColor())
                mesa.MostrarSelectorColor = true;

            mesa.Invalidate();
        }

        void AlElegirColor(string color)
        {
            mesa.MostrarSelectorColor = false;
            partida.elegirColor(color);
            mesa.Invalidate();
        }

        void AlRobar()
        {
            if (mesa.MostrarSelectorColor) return;
            if(cartaRobadaPendiente != null)
            {
                mesa.MostrarMensaje("Ya robaste una carta");
                return;
            }
            Carta robada = partida.robarCarta();
            if (robada == null) return;
            if (partida.sePuedeJugar(robada)) {
                cartaRobadaPendiente = robada;
                mesa.PasarDisponible = true;
                mesa.MostrarMensaje("¡Puedes jugar la carta que robaste!");
            }
            else
            {
                cartaRobadaPendiente = null;
                mesa.PasarDisponible = false;
                partida.siguienteTurno();
                mesa.MostrarMensaje("No se puede jugar, pasa el turno");
            }
            unoDeclarado = false;
            mesa.Invalidate();
        }

        void AlPresionarUno()
        {
            int cartas = partida.getJugadorActual().getCartas().Count;
            if (cartas == 2)
            {
                unoDeclarado = true;
                mesa.MostrarMensaje("¡UNO!");
            }
            else
            {
                mesa.MostrarMensaje("Aún tienes muchas cartas");
            }
        }
        void AlPasar()
        {
            if (cartaRobadaPendiente == null || mesa.MostrarSelectorColor)
                return;
            cartaRobadaPendiente = null;
            unoDeclarado = false;

            partida.siguienteTurno();

            mesa.PasarDisponible = false;
            mesa.MostrarMensaje("Turno pasado");
            mesa.Invalidate();
        }

        void TerminarPartida()
        {
            Jugador ganador = partida.getGanador();
            if (ganador == null) return;
            int idx = partida.getJugadores().IndexOf(ganador);
            if (idx >= 0) mesa.Puntos[idx]++;
            mesa.NombreGanador = ganador.getNombre();
            mesa.MostrarSelectorColor = false;
            mesa.MostrarVictoria = true;
            mesa.MostrarMenu = false;

            mesa.Invalidate();
        }
    }
}
