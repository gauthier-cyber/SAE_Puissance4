namespace Puissance4.Systeme
{
    public class Partie
    {
        public string PremierCoup { get; set; }
        public string CoupDecisif { get; set; }
        public int Duree { get; set; } // en secondes
        public int NombreCoups { get; set; }
        public Joueur Joueur1 { get; set; }
        public Joueur Joueur2 { get; set; }
        public Configuration Configuration { get; set; }
        public Joueur ?Gagnant { get; set; }
        public Grille Grille { get; set; }
        public Joueur JoueurCourant { get; set; }

        public Partie(Joueur J1, Joueur J2, Configuration config)
        {
            PremierCoup = "";
            CoupDecisif = "";
            Duree = 0;
            NombreCoups = 0;
            Joueur1 = J1;
            Joueur2 = J2;
            Configuration = config;
            Gagnant = null;
            Grille = new Grille(config.TailleGrille);
            JoueurCourant = J1;
        }
    }
}