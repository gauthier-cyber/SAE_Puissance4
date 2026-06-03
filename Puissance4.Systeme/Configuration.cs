using System;

namespace Puissance4.Systeme
{
    // La classe Parametres contient toute la configuration choisie par le joueur
    // dans la fenêtre de réglages de la maquette (Mode Challenge, taille de la grille,
    // nombre de jetons à aligner, temps de réflexion...).
    // On range tout ça dans une seule classe pour la passer facilement à la partie.
    public class Configuration
    {
        // --- Mode de jeu ---

        // Le mode choisi : 2 joueurs ou joueur seul.
        public ModeJeu Mode { get; set; }

        // Le niveau de l'IA si on joue seul.
        public NiveauIA NiveauIA { get; set; }

        // --- Mode Challenge ---

        // Vrai si le mode Challenge est activé (série de plusieurs parties).
        public bool ModeChallenge { get; set; }

        // --- Taille de la grille (ex: 6x7, 8x9, 10x12) ---

        // Nombre de lignes de la grille.
        public int NbLignes { get; set; }

        // Nombre de colonnes de la grille.
        public int NbColonnes { get; set; }

        // Nombre de jetons à aligner pour gagner (ex: 4 ou 5).
        public int NbJetonsAAligner { get; set; }

        // --- Temps de réflexion ---

        // Vrai si on limite le temps par coup (case cochée dans la maquette).
        public bool TempsLimite { get; set; }

        // Le temps de réflexion par coup en secondes (ex: 10).
        public int TempsReflexionSecondes { get; set; }


        // Constructeur : on met des valeurs par défaut qui correspondent
        // à un Puissance 4 classique (grille 6x7, aligner 4 jetons).
        public Configuration()
        {
            Mode = ModeJeu.DeuxJoueurs;
            NiveauIA = NiveauIA.Aucune;
            ModeChallenge = false;

            NbLignes = 6;
            NbColonnes = 7;
            NbJetonsAAligner = 4;

            TempsLimite = false;
            TempsReflexionSecondes = 10;
        }


        // Vérifie que les paramètres choisis sont cohérents.
        // Par exemple, il ne faut pas demander d'aligner plus de jetons
        // qu'il n'y a de cases. On renvoie vrai si tout est correct.
        public bool SontValides()
        {
            // La grille doit avoir une taille minimale.
            if (NbLignes < 4 || NbColonnes < 4)
                return false;

            // On doit aligner au moins 3 jetons.
            if (NbJetonsAAligner < 3)
                return false;

            // On ne peut pas aligner plus de jetons que la plus grande dimension.
            // On cherche d'abord la plus grande des deux dimensions.
            int plusGrandeDimension = NbLignes;
            if (NbColonnes > plusGrandeDimension)
                plusGrandeDimension = NbColonnes;

            if (NbJetonsAAligner > plusGrandeDimension)
                return false;

            // Si le temps est limité, il doit être positif.
            if (TempsLimite && TempsReflexionSecondes <= 0)
                return false;

            // Tout est bon.
            return true;
        }
    }
}