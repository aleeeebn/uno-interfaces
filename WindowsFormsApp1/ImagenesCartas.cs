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

        private static Image CargarYEscalarImagen(string ruta)
        {
            using (Image original = Image.FromFile(ruta))
            {
                Bitmap bmp = new Bitmap(Ancho, Alto);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(original, 0, 0, Ancho, Alto);
                }
                return bmp;
            }
        }
    }
}