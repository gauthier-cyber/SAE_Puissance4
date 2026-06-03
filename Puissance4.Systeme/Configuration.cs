using System;

namespace Puissance4.Systeme
{
    public class Configuration
    {
        public (int, int) TailleGrille { get; set; }
        public int NbJetonAAligner { get; set; }
        public string CouleurJoueur1 { get; set; }
        public string CouleurJoueur2 { get; set; }
        public string FormeJoueur1 { get; set; }
        public string FormeJoueur2 { get; set; }
        public int TempsReflexion { get; set; } = 0; // 0 signifie pas de limite de temps

        public Configuration((int, int) tailleGrille, int nbJetonAAligner, string couleurJoueur1, string couleurJoueur2, string formeJoueur1, string formeJoueur2, int tempsReflexion)
        {
            TailleGrille = tailleGrille;
            NbJetonAAligner = nbJetonAAligner;
            CouleurJoueur1 = couleurJoueur1;
            CouleurJoueur2 = couleurJoueur2;
            FormeJoueur1 = formeJoueur1;
            FormeJoueur2 = formeJoueur2;
            TempsReflexion = tempsReflexion;
        }
    }
}