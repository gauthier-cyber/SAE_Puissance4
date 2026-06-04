namespace Puissance4.Systeme
{
    public class Partie
    {
        public string ?PremierCoup { get; set; }
        public string ?CoupDecisif { get; set; }
        public double ?Duree { get; set; } // en secondes
        public int ?NombreCoups { get; set; }
        public Joueur Joueur1 { get; set; }
        public Joueur Joueur2 { get; set; }
        public Configuration Configuration { get; set; }
        public Joueur ?Gagnant { get; set; }
        public Grille Grille { get; set; }
        public Joueur JoueurCourant { get; set; }

        public Partie(Joueur J1, Joueur J2, Configuration config)
        {
            Joueur1 = J1;
            Joueur2 = J2;
            Configuration = config;
            Grille = new Grille(config.TailleGrille);
            JoueurCourant = J1;
        }

        public void FinirPartie(string premier_coup, string coup_decisif, double duree, int nb_coups, Joueur gagnant)
        {
            PremierCoup = premier_coup;
            CoupDecisif = coup_decisif;
            Duree = duree;
            NombreCoups = nb_coups;
            Gagnant = gagnant;
        }
    }
}