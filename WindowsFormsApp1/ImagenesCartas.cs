using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static partial class ImagenesCartas
    {
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
            string nombre = NombreArchivo(c);
            if (!cache.ContainsKey(nombre))
            {
                string ruta = Path.Combine(Application.StartupPath, "assets", nombre + ".png");
                cache[nombre] = CargarYEscalarImagen(ruta);
            }
            return cache[nombre];
        }
    }
}