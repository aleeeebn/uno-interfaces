using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }
}
