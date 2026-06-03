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
        })
    }
}