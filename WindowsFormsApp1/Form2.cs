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

        // Busca la carpeta assets junto al .exe o subiendo por las carpetas (bin/Debug -> proyecto)
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

        // Nombres tal cual están en tu carpeta assets
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
                            // Se reduce una sola vez para que el dibujo sea rápido
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
            cache[nombre] = img; // null = se dibuja carta de respaldo
            return img;
        }
    }
}
