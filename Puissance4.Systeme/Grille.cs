using System;
using System.Collections.Generic;
using System.Text;

namespace Puissance4.Systeme
{
    // Cette classe est le coeur du jeu.
    // Elle gère la grille en tableau 2 dimensions, la gravité des jetons
    // et la vérification des alignements pour trouver le gagnant.
    // Elle ne contient AUCUN élément visuel.
    public class Grille
    {
        // La grille de jeu. Chaque case contient :
        //   0 = case vide
        //   1 = jeton du joueur 1
        //   2 = jeton du joueur 2
        // Premier indice = ligne, deuxième indice = colonne.
        private int[,] _grille;

        private int _nombreLignes;
        private int _nombreColonnes;
        private int _jetonsAAligner;

        // On garde en mémoire les cases de la dernière combinaison gagnante.
        // Cela permet au front de mettre ces jetons en évidence (comme sur la maquette).
        // _lignesGagnantes[i] et _colonnesGagnantes[i] = la case i de l'alignement.
        private int[] _lignesGagnantes;
        private int[] _colonnesGagnantes;
        private int _nbCasesGagnantes;

        // Constructeur : on crée une grille vide de la taille demandée.
        public Grille(int nombreLignes, int nombreColonnes, int jetonsAAligner)
        {
            _nombreLignes = nombreLignes;
            _nombreColonnes = nombreColonnes;
            _jetonsAAligner = jetonsAAligner;

            _grille = new int[_nombreLignes, _nombreColonnes];

            // On prépare le tableau qui stockera les cases gagnantes.
            _lignesGagnantes = new int[_jetonsAAligner];
            _colonnesGagnantes = new int[_jetonsAAligner];
            _nbCasesGagnantes = 0;

            ViderGrille();
        }

        // Propriétés en lecture seule pour que le front connaisse la taille.
        public int NombreLignes
        {
            get { return _nombreLignes; }
        }

        public int NombreColonnes
        {
            get { return _nombreColonnes; }
        }

        public int JetonsAAligner
        {
            get { return _jetonsAAligner; }
        }

        // Remet toutes les cases à 0 (grille vide).
        public void ViderGrille()
        {
            for (int ligne = 0; ligne < _nombreLignes; ligne++)
            {
                for (int colonne = 0; colonne < _nombreColonnes; colonne++)
                {
                    _grille[ligne, colonne] = 0;
                }
            }
            _nbCasesGagnantes = 0;
        }

        // Renvoie le contenu d'une case (0, 1 ou 2).
        // Le front l'utilise pour dessiner la grille.
        public int LireCase(int ligne, int colonne)
        {
            return _grille[ligne, colonne];
        }

        // Vérifie si on peut encore jouer dans une colonne.
        // C'est possible si la case du haut (ligne 0) est encore vide.
        public bool ColonneJouable(int colonne)
        {
            // On vérifie d'abord que la colonne existe vraiment.
            if (colonne < 0 || colonne >= _nombreColonnes)
            {
                return false;
            }
            if (_grille[0, colonne] == 0)
            {
                return true;
            }
            return false;
        }

        // Joue un jeton dans une colonne pour un joueur (numeroJoueur = 1 ou 2).
        // Grâce à la gravité, le jeton tombe sur la ligne libre la plus basse.
        // Renvoie le numéro de la ligne où le jeton s'est posé, ou -1 si la colonne est pleine.
        public int JouerColonne(int colonne, int numeroJoueur)
        {
            // Si la colonne n'est pas jouable, on renvoie -1 (coup impossible).
            if (ColonneJouable(colonne) == false)
            {
                return -1;
            }

            // On part du bas de la grille (dernière ligne) et on remonte
            // jusqu'à trouver une case vide.
            for (int ligne = _nombreLignes - 1; ligne >= 0; ligne--)
            {
                if (_grille[ligne, colonne] == 0)
                {
                    _grille[ligne, colonne] = numeroJoueur;
                    return ligne; // on a trouvé la place, on s'arrête
                }
            }

            // Normalement on n'arrive jamais ici, mais on renvoie -1 par sécurité.
            return -1;
        }

        // Vérifie si la grille est complètement pleine (match nul possible).
        public bool GrillePleine()
        {
            // Si au moins une colonne est jouable, la grille n'est pas pleine.
            for (int colonne = 0; colonne < _nombreColonnes; colonne++)
            {
                if (ColonneJouable(colonne) == true)
                {
                    return false;
                }
            }
            return true;
        }

        // Vérifie si le joueur "numeroJoueur" vient de gagner.
        // On regarde les 4 directions : horizontale, verticale et les 2 diagonales.
        // Si on trouve un alignement, on renvoie vrai et on garde les cases gagnantes.
        public bool VerifierVictoire(int numeroJoueur)
        {
            // On teste chaque case de la grille comme point de départ possible.
            for (int ligne = 0; ligne < _nombreLignes; ligne++)
            {
                for (int colonne = 0; colonne < _nombreColonnes; colonne++)
                {
                    // Direction horizontale (vers la droite) : ligne fixe, colonne +1
                    if (VerifierDirection(ligne, colonne, 0, 1, numeroJoueur) == true)
                    {
                        return true;
                    }
                    // Direction verticale (vers le bas) : ligne +1, colonne fixe
                    if (VerifierDirection(ligne, colonne, 1, 0, numeroJoueur) == true)
                    {
                        return true;
                    }
                    // Diagonale qui descend vers la droite : ligne +1, colonne +1
                    if (VerifierDirection(ligne, colonne, 1, 1, numeroJoueur) == true)
                    {
                        return true;
                    }
                    // Diagonale qui monte vers la droite : ligne -1, colonne +1
                    if (VerifierDirection(ligne, colonne, -1, 1, numeroJoueur) == true)
                    {
                        return true;
                    }
                }
            }
            // Aucun alignement trouvé.
            return false;
        }

        // Vérifie s'il y a "_jetonsAAligner" jetons identiques en partant d'une case
        // et en avançant dans une direction donnée (pasLigne, pasColonne).
        // Exemple : pasLigne=0 et pasColonne=1 veut dire "on avance vers la droite".
        private bool VerifierDirection(int ligneDepart, int colonneDepart, int pasLigne, int pasColonne, int numeroJoueur)
        {
            int ligne = ligneDepart;
            int colonne = colonneDepart;

            // On va vérifier "_jetonsAAligner" cases d'affilée.
            for (int compteur = 0; compteur < _jetonsAAligner; compteur++)
            {
                // Si on sort de la grille, l'alignement est impossible.
                if (ligne < 0 || ligne >= _nombreLignes || colonne < 0 || colonne >= _nombreColonnes)
                {
                    return false;
                }
                // Si la case n'appartient pas au joueur, c'est raté.
                if (_grille[ligne, colonne] != numeroJoueur)
                {
                    return false;
                }
                // On avance d'une case dans la direction choisie.
                ligne = ligne + pasLigne;
                colonne = colonne + pasColonne;
            }

            // Si on arrive ici, toutes les cases appartenaient au joueur : c'est gagné.
            // On enregistre les cases gagnantes pour que le front puisse les surligner.
            EnregistrerCasesGagnantes(ligneDepart, colonneDepart, pasLigne, pasColonne);
            return true;
        }

        // Garde en mémoire les cases de l'alignement gagnant.
        private void EnregistrerCasesGagnantes(int ligneDepart, int colonneDepart, int pasLigne, int pasColonne)
        {
            int ligne = ligneDepart;
            int colonne = colonneDepart;
            for (int i = 0; i < _jetonsAAligner; i++)
            {
                _lignesGagnantes[i] = ligne;
                _colonnesGagnantes[i] = colonne;
                ligne = ligne + pasLigne;
                colonne = colonne + pasColonne;
            }
            _nbCasesGagnantes = _jetonsAAligner;
        }

        // Le front peut demander combien de cases gagnantes il y a.
        public int NombreCasesGagnantes()
        {
            return _nbCasesGagnantes;
        }

        // Le front demande la ligne de la i-ème case gagnante.
        public int LireLigneGagnante(int i)
        {
            return _lignesGagnantes[i];
        }

        // Le front demande la colonne de la i-ème case gagnante.
        public int LireColonneGagnante(int i)
        {
            return _colonnesGagnantes[i];
        }
    }
}