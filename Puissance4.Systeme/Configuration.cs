namespace Puissance4.Systeme
{
    public class Configuration
    {
        public (int, int) TailleGrille { get; set; }
        public int NbJetonAAligner { get; set; }
        public string CouleurJoueur1 { get; set; }
        public string CouleurJoueur2 { get; set; }
        public string FormeJoueur { get; set; }
        public int TempsReflexion { get; set; } = 0; // 0 = pas de limite de temps

        // Nouveaux reglages d'accessibilite :
        public int TailleTexte { get; set; } = 20;         // taille de police choisie au curseur
        public bool ContrasteMarque { get; set; } = false; // true si l'option contraste est activee

        public Configuration((int, int) tailleGrille, int nbJetonAAligner, string couleurJoueur1,
                             string couleurJoueur2, string formeJoueur, int tempsReflexion,
                             int tailleTexte, bool contrasteMarque)
        {
            TailleGrille = tailleGrille;
            NbJetonAAligner = nbJetonAAligner;
            CouleurJoueur1 = couleurJoueur1;
            CouleurJoueur2 = couleurJoueur2;
            FormeJoueur = formeJoueur;
            TempsReflexion = tempsReflexion;
            TailleTexte = tailleTexte;
            ContrasteMarque = contrasteMarque;
        }
    }
}