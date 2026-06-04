namespace Puissance4.Systeme
{
    public enum  EtatCase
    {
        Vide,
        Joueur1,
        Joueur2
    }

    public class Grille
    {
        public List<List<EtatCase>> Tableau { get; set; }

        public int Lignes { get; set; }
        public int Colonnes { get; set; }

        public Grille(int lignes, int colonnes)
        {
            Lignes = lignes;
            Colonnes = colonnes;
            Tableau = new List<List<EtatCase>>();
            for (int i = 0; i < Lignes; i++)
            {
                var ligne = new List<EtatCase>();
                for (int j = 0; j < Colonnes; j++)
                {
                    ligne.Add(EtatCase.Vide);
                }
                Tableau.Add(ligne);
            }
        }

        public Grille((int, int) Taille)
        {
            Lignes = Taille.Item1;
            Colonnes = Taille.Item2;
            Tableau = new List<List<EtatCase>>();
            for (int i = 0; i < Lignes; i++)
            {
                var ligne = new List<EtatCase>();
                for (int j = 0; j < Colonnes; j++)
                {
                    ligne.Add(EtatCase.Vide);
                }
                Tableau.Add(ligne);
            }
        }

        public EtatCase VérifierAlignements(int nbJetons)
        {
            for (int i = 0; i < Lignes; i++)
            {
                for (int j = 0; j < Colonnes; j++)
                {
                    EtatCase joueurActuel = Tableau[i][j];

                    // Si la case est vide, inutile de vérifier les alignements à partir d'ici
                    if (joueurActuel == EtatCase.Vide)
                        continue;

                    // 1. Vérification Horizontale (vers la droite)
                    if (j + nbJetons <= Colonnes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < nbJetons; k++)
                        {
                            if (Tableau[i][j + k] != joueurActuel) { aligne = false; break; }
                        }
                        if (aligne) return joueurActuel;
                    }

                    // 2. Vérification Verticale (vers le bas)
                    if (i + nbJetons <= Lignes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < nbJetons; k++)
                        {
                            if (Tableau[i + k][j] != joueurActuel) { aligne = false; break; }
                        }
                        if (aligne) return joueurActuel;
                    }

                    // 3. Vérification Diagonale Descendante (vers le bas et la droite ➘)
                    if (i + nbJetons <= Lignes && j + nbJetons <= Colonnes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < nbJetons; k++)
                        {
                            if (Tableau[i + k][j + k] != joueurActuel) { aligne = false; break; }
                        }
                        if (aligne) return joueurActuel;
                    }

                    // 4. Vérification Diagonale Ascendante (vers le haut et la droite ➚)
                    if (i - nbJetons + 1 >= 0 && j + nbJetons <= Colonnes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < nbJetons; k++)
                        {
                            if (Tableau[i - k][j + k] != joueurActuel) { aligne = false; break; }
                        }
                        if (aligne) return joueurActuel;
                    }
                }
            }

            // Aucun alignement trouvé
            return EtatCase.Vide;
        }
    }
}