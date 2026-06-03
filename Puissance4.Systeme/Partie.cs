namespace Puissance4.Systeme
{
    public class Partie
    {
        public string PremierCoup { get; set; }
        public string CoupDecisif { get; set; }
        public int Duree { get; set; } // en secondes
        public int NombreCoups { get; set; }

        public Partie()
        {
            PremierCoup = "";
            CoupDecisif = "";
            Duree = 0;
            NombreCoups = 0;
        }
    }
}