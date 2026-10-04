using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }

    internal class Tablero : Control
    {
        class Zona
        {
            public RectangleF R;
            public Action Accion;
            public int Indice = -1;
        }

        // ---------- Datos ----------
        public Partida Partida;
        public int[] Puntos = new int[3];          // partidas ganadas (se puede alimentar de la BD)
        public bool SentidoHorario = true;
        public bool MostrarSelectorColor = false;
        public bool Silencio = false;
        public string Mensaje = "";

        // ---------- Eventos ----------
        public event Action<Carta> CartaClick;
        public event Action RobarClick;
        public event Action UnoClick;
        public event Action SalirClick;
        public event Action<string> ColorElegido;  // "Rojo", "Azul", "Amarillo", "Verde"

        // ---------- Paleta pastel ----------
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

        // ───────────── Interacción ─────────────
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

        // ───────────── Dibujo principal ─────────────
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
        }

        // Asiento 0 = abajo (jugador en turno), 1 = izquierda, 2 = derecha
        Jugador JugadorEnAsiento(int seat)
        {
            int n = Partida.getJugadores().Count;
            return Partida.getJugador((Partida.getTurnoActual() + seat) % n);
        }

        int IndiceEnAsiento(int seat)
        {
            int n = Partida.getJugadores().Count;
            return (Partida.getTurnoActual() + seat) % n;
        }

        // ───────────── Fondo ─────────────
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
            using (var p = Corazon(new RectangleF(22 + fs * 2.4f, 18, fs * 0.4f, fs * 0.36f)))
            using (var b = new SolidBrush(RosaFuerte))
                g.FillPath(b, p);
        }

        // ───────────── Mesa (centro) ─────────────
        void DibujarMesa(Graphics g, float W, float H, float cw, float ch)
        {
            float cx = W / 2, cy = H * 0.45f;
            float mw = W * 0.46f, mh = H * 0.46f;
            var mesa = new RectangleF(cx - mw / 2, cy - mh / 2, mw, mh);

            using (var b = new SolidBrush(Color.FromArgb(110, 255, 255, 255)))
                g.FillEllipse(b, mesa);
            using (var p = new Pen(Color.FromArgb(190, 255, 255, 255), 4) { DashStyle = DashStyle.Dot })
                g.DrawEllipse(p, mesa);

            // Banner de turno
            string turno = "Turno de " + Partida.getJugadorActual().getNombre() + " ♥";
            using (var f = new Font("Segoe UI", Math.Max(14f, H * 0.032f), FontStyle.Bold, GraphicsUnit.Pixel))
            {
                SizeF sz = g.MeasureString(turno, f);
                var r = new RectangleF(cx - sz.Width / 2 - 22, 18, sz.Width + 44, sz.Height + 14);
                Capsula(g, r, Mantequilla, Coral, 3);
                Texto(g, turno, f, Cafe, r);
            }

            // Mazo para robar
            var mazo = new RectangleF(cx - cw - 14, cy - ch / 2, cw, ch);
            for (int i = 2; i >= 1; i--)
                DibujarReverso(g, new RectangleF(mazo.X - i * 3, mazo.Y - i * 3, cw, ch));
            DibujarReverso(g, mazo);
            zonas.Add(new Zona { R = mazo, Accion = () => { if (RobarClick != null) RobarClick(); } });
            using (var f = new Font("Segoe UI", Math.Max(11f, H * 0.022f), FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, "Robar (" + Partida.getMazo().getMazoSize() + ")", f, Cafe,
                      new RectangleF(mazo.X - 20, mazo.Bottom + 6, cw + 40, 22));

            // Carta en juego
            var desc = new RectangleF(cx + 14, cy - ch / 2, cw, ch);
            Carta ultima = Partida.getJuego().getUltimaCarta();
            if (ultima != null) DibujarCara(g, desc, ultima);
            else
                using (var p = Redondo(desc, cw * 0.1f))
                using (var pen = new Pen(Color.FromArgb(200, 255, 255, 255), 3) { DashStyle = DashStyle.Dash })
                    g.DrawPath(pen, p);

            // Color actual (corazón)
            float hs = Math.Max(40f, H * 0.08f);
            var rc = new RectangleF(desc.Right + 26, cy - hs / 2 - 10, hs, hs * 0.9f);
            using (var p = Corazon(rc))
            {
                using (var b = new SolidBrush(ColorDe(Partida.getColorActual()))) g.FillPath(b, p);
                using (var pen = new Pen(Color.White, 4)) g.DrawPath(pen, p);
            }
            using (var f = new Font("Segoe UI", Math.Max(11f, H * 0.02f), FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, "Color", f, Cafe, new RectangleF(rc.X - 10, rc.Bottom + 10, hs + 20, 20));

            // Sentido
            float ds = Math.Max(40f, H * 0.075f);
            var rd = new RectangleF(mazo.X - ds - 40, cy - ds / 2 - 10, ds, ds);
            g.FillEllipse(Brushes.White, rd);
            using (var pen = new Pen(Menta, 4)) g.DrawEllipse(pen, rd);
            using (var f = new Font("Segoe UI", ds * 0.6f, FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, SentidoHorario ? "↻" : "↺", f, Color.FromArgb(120, 190, 160), rd);
        }

        // ───────────── Manos ─────────────
        void DibujarMano(Graphics g, int seat, float W, float H, float cw, float ch)
        {
            List<Carta> mano = JugadorEnAsiento(seat).getCartas();
            int n = mano.Count;
            if (n == 0) return;

            for (int i = 0; i < n; i++)
            {
                RectangleF bounds;
                float ang = 0;
                PointF off = new PointF(0, 0);
                bool hov = (seat == 0 && !MostrarSelectorColor && hoverI == i);

                if (seat == 0)
                {
                    float step = n > 1 ? Math.Min(cw * 0.8f, (W * 0.6f - cw) / (n - 1)) : 0;
                    float total = cw + step * (n - 1);
                    float x0 = (W - total) / 2;
                    bounds = new RectangleF(x0 + i * step, H - ch - 26, cw, ch);
                    if (hov) off = new PointF(0, -22);
                }
                else
                {
                    float step = n > 1 ? Math.Min(cw * 0.5f, (H * 0.55f - cw) / (n - 1)) : 0;
                    float total = cw + step * (n - 1);
                    float y0 = H * 0.5f - total / 2 + 30;
                    float cx = seat == 1 ? 16 + ch / 2 : W - 16 - ch / 2;
                    float cy = y0 + cw / 2 + i * step;
                    bounds = new RectangleF(cx - ch / 2, cy - cw / 2, ch, cw);
                    ang = seat == 1 ? 90 : -90;
                }

                var dib = new RectangleF(bounds.X + off.X, bounds.Y + off.Y, bounds.Width, bounds.Height);
                Carta carta = mano[i];
                bool visible = true; // el proyecto pide ver las cartas de todos los jugadores
                Rotar(g, dib, ang, r =>
                {
                    if (visible) DibujarCara(g, r, carta);
                    else DibujarReverso(g, r);
                });

                if (seat == 0) // solo se puede jugar con la mano del jugador en turno
                {
                    int ii = i;
                    zonas.Add(new Zona
                    {
                        R = bounds,
                        Indice = ii,
                        Accion = () => { if (CartaClick != null) CartaClick(carta); }
                    });
                }
            }
        }

        // ───────────── Etiquetas de jugador ─────────────
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
                    bool activo = seat == 0;
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

        // ───────────── Selector de color (comodines) ─────────────
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
                Texto(g, "Elige un color ♥", f, Cafe, new RectangleF(panel.X, panel.Y + 8, panel.Width, 40));

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
                var r = new RectangleF(W / 2 - sz.Width / 2 - 24, H * 0.45f + H * 0.25f - 6, sz.Width + 48, sz.Height + 16);
                Capsula(g, r, Color.FromArgb(245, 255, 255, 255), RosaFuerte, 3);
                Texto(g, Mensaje, f, Cafe, r);
            }
        }

        // ───────────── Botones ─────────────
        void DibujarBotones(Graphics g, float W, float H)
        {
            float bs = Math.Max(40f, H * 0.065f);

            Boton(g, new RectangleF(W - bs - 16, 14, bs, bs), Color.FromArgb(255, 160, 175), "✕",
                  () => { if (SalirClick != null) SalirClick(); }, Color.White, bs * 0.5f, false);

            Boton(g, new RectangleF(W - bs * 2 - 28, 14, bs, bs), Lavanda, "♪",
                  () => { Silencio = !Silencio; Invalidate(); },
                  Color.White, bs * 0.55f, Silencio);

            float us = Math.Max(80f, H * 0.14f);
            Boton(g, new RectangleF(W - us - 20, H - us - 20, us, us), Coral, "UNO!",
                  () => { if (UnoClick != null) UnoClick(); }, Color.White, us * 0.26f, false);
        }

        void Boton(Graphics g, RectangleF r, Color fill, string txt, Action accion,
                   Color colTxt, float fs, bool tachado)
        {
            bool h = r.Contains(mouse);
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

            zonas.Add(new Zona { R = r, Accion = accion });
        }

        // ───────────── Cartas ─────────────
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

            // 1) Tu imagen de la carpeta assets (ya trae bordes redondeados y transparencia)
            Image img = ImagenesUno.Obtener(c);
            if (img != null)
            {
                g.DrawImage(img, r);
                return;
            }

            // 2) Respaldo: carta pastel dibujada por código (si falta alguna imagen)
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

        // Dorso de carta (no hay imagen de dorso en assets, así que se dibuja por código)
        void DibujarReverso(Graphics g, RectangleF r)
        {
            Sombra(g, r);
            float rad = r.Width * 0.12f;

            using (var p = Redondo(r, rad))
            {
                using (var b = new LinearGradientBrush(r, Lavanda, Color.FromArgb(255, 190, 215), 70f))
                    g.FillPath(b, p);
                using (var pen = new Pen(Color.White, 3.5f)) g.DrawPath(pen, p);
            }
            var ov = new RectangleF(r.X + r.Width * 0.14f, r.Y + r.Height * 0.22f, r.Width * 0.72f, r.Height * 0.56f);
            using (var b = new SolidBrush(Color.FromArgb(235, Crema))) g.FillEllipse(b, ov);
            using (var f = new Font("Segoe UI", r.Height * 0.22f, FontStyle.Bold, GraphicsUnit.Pixel))
                Texto(g, "UNO", f, RosaFuerte, ov);
            using (var p = Corazon(new RectangleF(r.X + r.Width * 0.12f, r.Y + r.Height * 0.06f, r.Width * 0.16f, r.Width * 0.14f)))
            using (var b = new SolidBrush(Color.White))
                g.FillPath(b, p);
        }

        // ───────────── Utilidades ─────────────
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


}
