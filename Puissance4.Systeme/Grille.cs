namespace Puissance4.Systeme
{
    public enum EtatCase
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
            // on remplit toute la grille avec des cases vides
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
            // on remplit toute la grille avec des cases vides
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

        // on place le jeton d'un joueur sur une case précise
        public void ChangerValeurCase(int ligne, int colonne, EtatCase etat)
        {
            Tableau[ligne][colonne] = etat;
        }

        public EtatCase VérifierAlignements(int nbJetons)
        {
            // on passe sur chaque case de la grille
            for (int i = 0; i < Lignes; i++)
            {
                for (int j = 0; j < Colonnes; j++)
                {
                    EtatCase joueurActuel = Tableau[i][j];

                    // si la case est vide on ne regarde rien à partir d'ici
                    if (joueurActuel == EtatCase.Vide)
                        continue;

                    // 1. on regarde vers la droite (horizontal)
                    if (j + nbJetons <= Colonnes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < nbJetons; k++)
                        {
                            if (Tableau[i][j + k] != joueurActuel) { aligne = false; break; }
                        }
                        if (aligne) return joueurActuel;
                    }

                    // 2. on regarde vers le bas (vertical)
                    if (i + nbJetons <= Lignes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < nbJetons; k++)
                        {
                            if (Tableau[i + k][j] != joueurActuel) { aligne = false; break; }
                        }
                        if (aligne) return joueurActuel;
                    }

                    // 3. on regarde la diagonale qui descend vers la droite
                    if (i + nbJetons <= Lignes && j + nbJetons <= Colonnes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < nbJetons; k++)
                        {
                            if (Tableau[i + k][j + k] != joueurActuel) { aligne = false; break; }
                        }
                        if (aligne) return joueurActuel;
                    }

                    // 4. on regarde la diagonale qui monte vers la droite
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

            // on n'a trouvé aucun alignement
            return EtatCase.Vide;
        }
    }
}