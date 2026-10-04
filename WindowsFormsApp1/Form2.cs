using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }
}
