using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static partial class ImagenesCartas
    {
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
                    {
                        g.DrawString("UNO", f, Brushes.Yellow, 10, 48);
                    }
                }
                cache["dorso"] = bmp;
            }
            return cache["dorso"];
        }
    }
}