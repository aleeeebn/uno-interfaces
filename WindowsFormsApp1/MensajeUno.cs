namespace WindowsFormsApp1
{
    internal class MensajeUno
    {
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string Color { get; set; }
        public int Indice { get; set; } = -1;
        public int Version { get; set; }
    }

    internal class CartaRed
    {
        public string Color { get; set; }
        public int Valor { get; set; }

        public static CartaRed Desde(Carta carta)
        {
            return carta == null ? null : new CartaRed
            {
                Color = carta.getColor(),
                Valor = carta.getValor()
            };
        }

        public Carta Crear()
        {
            return new Carta(Color, Valor);
        }
    }

    internal class EstadoUno
    {
        public string Tipo { get; set; } = "ESTADO";
        public int Version { get; set; }
        public int MiJugador { get; set; }
        public string[] Nombres { get; set; }
        public int[] Cantidades { get; set; }
        public CartaRed[] Mano { get; set; }
        public CartaRed Descarte { get; set; }
        public string Color { get; set; }
        public int Turno { get; set; }
        public int CantidadMazo { get; set; }
        public bool Horario { get; set; }
        public bool ElegirColor { get; set; }
        public bool PuedePasar { get; set; }
        public bool PuedeAcusar { get; set; }
        public int CartaRobada { get; set; } = -1;
        public int[] Rondas { get; set; }
        public int[] Puntos { get; set; }
        public bool Victoria { get; set; }
        public string Ganador { get; set; }
        public string Titulo { get; set; }
    }
}