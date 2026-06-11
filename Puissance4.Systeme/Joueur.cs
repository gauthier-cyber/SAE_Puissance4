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

        // un vrai joueur est humain par défaut
        public Joueur(string nom)
        {
            Nom = nom;
            NiveauVirtuel = NiveauVirtuel.Humain;
        }

        // ici on précise le niveau, utile pour créer une IA
        public Joueur(string nom, NiveauVirtuel niveau)
        {
            Nom = nom;
            NiveauVirtuel = niveau;
        }
    }
}