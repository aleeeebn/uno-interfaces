using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static class ImagenesCartas
    {
        public const int Ancho = 80, Alto = 120;
        private static readonly Dictionary<string, Image> cache = new Dictionary<string, Image>();

        public static string NombreArchivo(Carta c)
        {
            int v = c.getValor();
            if (v == 13) return "cambio_color";
            if (v == 14) return "+4";
            string color = c.getColor();
            string pref = color == "Rojo" ? "rojo" : color == "Azul" ? "azul"
                        : color == "Verde" ? "verde" : "amarilla";
            switch (v)
            {
                case 10: return (color == "Rojo" ? "roja" : pref) + "+2";
                case 11: return pref + "_reversa";
                case 12: return (color == "Amarillo" ? "amarillo" : pref) + "_stop";
                default: return pref + v;
            }
        }

        public static Image Obtener(Carta c)
        {
            if (c == null) return null;

            string nombre = NombreArchivo(c);

            if (!cache.ContainsKey(nombre))
            {
                string ruta = Path.Combine(Application.StartupPath, "assets", nombre + ".png");

                if (File.Exists(ruta))
                {
                    using (Image original = Image.FromFile(ruta))
                    {
                        Bitmap bmp = new Bitmap(Ancho, Alto);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.DrawImage(original, 0, 0, Ancho, Alto);
                        }
                        cache[nombre] = bmp;
                    }
                }
                else
                {
                    Bitmap bmp = new Bitmap(Ancho, Alto);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.DarkSlateGray);
                        using (Font font = new Font("Arial", 9, FontStyle.Bold))
                        using (Brush brush = new SolidBrush(Color.White))
                        {
                            g.DrawString(nombre, font, brush, 5, 20);
                        }
                    }
                    cache[nombre] = bmp;
                }
            }

            return cache[nombre];
        }

        public static Image Dorso()
        {
            if (!cache.ContainsKey("dorso"))
            {
                Bitmap bmp = new Bitmap(Ancho, Alto);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Black);
                    g.FillEllipse(Brushes.Red, 8, 30, Ancho - 16, Alto - 60);
                    using (Font f = new Font("Arial", 16, FontStyle.Bold | FontStyle.Italic))
                        g.DrawString("UNO", f, Brushes.Yellow, 10, 48);
                }
                cache["dorso"] = bmp;
            }
            return cache["dorso"];
        }
    }
}