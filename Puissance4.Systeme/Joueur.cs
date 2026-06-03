using System;

namespace Puissance4.Systeme
{
    // La classe Joueur représente un joueur du Puissance 4.
    // Un joueur a un nom, une couleur (pour ses jetons) et peut être une IA ou non.
    // On stocke aussi ses statistiques globales (victoires, défaites...) qui servent
    // pour l'écran "Résumé des performances" de la maquette.
    public class Joueur
    {
        // --- Informations de base du joueur ---

        // Le nom affiché (ex: "Joueur 1").
        public string Nom { get; set; }

        // La couleur du jeton (ex: "Jaune" ou "Rouge").
        // On garde un simple texte car le back-end ne doit pas gérer le graphisme.
        public string Couleur { get; set; }

        // Le numéro du jeton de ce joueur dans la grille.
        // On utilise 1 pour le joueur 1 et 2 pour le joueur 2.
        // Le moteur (classe Plateau) range ce numéro dans les cases de la grille.
        public int NumeroJeton { get; set; }

        // Vrai si ce joueur est contrôlé par l'ordinateur.
        public bool EstIA { get; set; }

        // Le niveau de l'IA. Vaut Aucune si EstIA est faux.
        public NiveauIA Niveau { get; set; }

        // --- Statistiques globales (pour le résumé des performances) ---

        // Nombre total de parties gagnées par ce joueur.
        public int NbVictoires { get; set; }

        // Nombre total de parties perdues par ce joueur.
        public int NbDefaites { get; set; }

        // Nombre total de coups joués sur toutes les parties.
        // Sert à calculer la moyenne de coups par partie.
        public int TotalCoupsJoues { get; set; }

        // Nombre de parties terminées (sert aussi pour calculer la moyenne).
        public int NbPartiesJouees { get; set; }


        // Constructeur : on crée un joueur en donnant son nom et sa couleur.
        // Par défaut ce n'est pas une IA.
        public Joueur(string nom, string couleur)
        {
            Nom = nom;
            Couleur = couleur;
            NumeroJeton = 0; // sera mis à 1 ou 2 quand on crée la partie
            EstIA = false;
            Niveau = NiveauIA.Aucune;

            // Au début, toutes les statistiques sont à zéro.
            NbVictoires = 0;
            NbDefaites = 0;
            TotalCoupsJoues = 0;
            NbPartiesJouees = 0;
        }


        // Transforme ce joueur en IA avec un niveau donné.
        public void DefinirCommeIA(NiveauIA niveau)
        {
            EstIA = true;
            Niveau = niveau;
        }


        // Calcule le ratio de victoires en pourcentage (ex: 72).
        // On renvoie un entier pour rester simple.
        public int CalculerRatioVictoires()
        {
            int total = NbVictoires + NbDefaites;

            // On évite la division par zéro si le joueur n'a jamais joué.
            if (total == 0)
                return 0;

            // On calcule le pourcentage. Le (double) sert à ne pas perdre les décimales
            // pendant le calcul, puis on convertit en entier à la fin.
            double ratio = (double)NbVictoires / total * 100;
            return (int)ratio;
        }


        // Calcule la moyenne de coups joués par partie (ex: 14).
        public int CalculerMoyenneCoups()
        {
            // On évite la division par zéro.
            if (NbPartiesJouees == 0)
                return 0;

            return TotalCoupsJoues / NbPartiesJouees;
        }
    }
}