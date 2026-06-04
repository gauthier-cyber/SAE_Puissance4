namespace Puissance4.Systeme
{
    public enum NiveauVirtuel
    {
        Humain,
        Idiot,
        Intelligent
    }

    public class Joueur
    {
        public string Nom { get; set; }
        public NiveauVirtuel NiveauVirtuel { get; set; }

        public Joueur(string nom)
        {
            Nom = nom;
            NiveauVirtuel = NiveauVirtuel.Humain;
        }

        public Joueur(string nom, NiveauVirtuel niveau)
        {
            Nom = nom;
            NiveauVirtuel = niveau;
        }
    }
}