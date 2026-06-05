using System;
using System.Collections.Generic;

namespace Puissance4.Systeme
{
    // l'ia choisit juste une colonne et renvoie son numero
    // elle ne s'occupe pas de l'affichage
    // niveau idiot = colonne au hasard, niveau intelligent = minimax alpha-beta
    // l'ia joue les jetons Joueur2, l'humain joue les jetons Joueur1
    public class IA
    {
        private NiveauVirtuel niveau;     // idiot ou intelligent
        private Random generateur;        // pour le hasard de l'ia idiote
        private int profondeurMax;        // nb de coups que l'ia anticipe
        private int nbJetonsPourGagner;   // combien de jetons a aligner pour gagner

        public IA(NiveauVirtuel niveau, int nbJetonsPourGagner)
        {
            this.niveau = niveau;
            this.generateur = new Random();
            this.nbJetonsPourGagner = nbJetonsPourGagner;
            this.profondeurMax = 4;
        }

        // methode appelee par la fenetre de jeu, renvoie la colonne choisie
        public int ChoisirColonne(Grille grille)
        {
            if (niveau == NiveauVirtuel.Idiot)
            {
                return ChoisirColonneIdiot(grille);
            }
            else
            {
                return ChoisirColonneIntelligent(grille);
            }
        }

        // ia idiote : une colonne au hasard parmi celles encore jouables
        private int ChoisirColonneIdiot(Grille grille)
        {
            List<int> colonnesJouables = ColonnesJouables(grille);

            // grille pleine, on renvoie -1
            if (colonnesJouables.Count == 0)
            {
                return -1;
            }

            int index = generateur.Next(colonnesJouables.Count);
            return colonnesJouables[index];
        }

        // ia intelligente : on teste chaque colonne et on garde celle qui a le meilleur score
        private int ChoisirColonneIntelligent(Grille grille)
        {
            List<int> colonnesJouables = ColonnesJouables(grille);

            if (colonnesJouables.Count == 0)
            {
                return -1;
            }

            // on part du plus mauvais score possible
            int meilleurScore = int.MinValue;
            int meilleureColonne = colonnesJouables[0];

            // alpha = -infini, beta = +infini au debut
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            for (int i = 0; i < colonnesJouables.Count; i++)
            {
                int colonne = colonnesJouables[i];

                // on copie la grille pour tester sans toucher a la vraie partie
                Grille copie = CopierGrille(grille);
                JouerCoup(copie, colonne, EtatCase.Joueur2);

                // apres notre coup c'est a l'adversaire de jouer, donc estMax = false
                int score = AlphaBeta(copie, profondeurMax - 1, alpha, beta, false);

                if (score > meilleurScore)
                {
                    meilleurScore = score;
                    meilleureColonne = colonne;
                }

                if (meilleurScore > alpha)
                {
                    alpha = meilleurScore;
                }
            }

            return meilleureColonne;
        }

        // minimax avec elagage alpha-beta (vu en sae 2.2)
        // estMax = true quand c'est a l'ia de jouer, false pour l'adversaire
        private int AlphaBeta(Grille grille, int profondeur, int alpha, int beta, bool estMax)
        {
            EtatCase gagnant = grille.VérifierAlignements(nbJetonsPourGagner);

            // l'ia gagne, gros score positif (+profondeur pour gagner vite)
            if (gagnant == EtatCase.Joueur2)
            {
                return 100000 + profondeur;
            }

            // l'adversaire gagne, gros score negatif
            if (gagnant == EtatCase.Joueur1)
            {
                return -100000 - profondeur;
            }

            // on s'arrete si plus de profondeur ou plus de coup possible
            List<int> colonnesJouables = ColonnesJouables(grille);
            if (profondeur == 0 || colonnesJouables.Count == 0)
            {
                return Evaluer(grille);
            }

            if (estMax)
            {
                // c'est l'ia, elle veut le plus grand score
                int meilleurScore = int.MinValue;

                for (int i = 0; i < colonnesJouables.Count; i++)
                {
                    int colonne = colonnesJouables[i];

                    Grille copie = CopierGrille(grille);
                    JouerCoup(copie, colonne, EtatCase.Joueur2);

                    int score = AlphaBeta(copie, profondeur - 1, alpha, beta, false);

                    if (score > meilleurScore)
                    {
                        meilleurScore = score;
                    }
                    if (meilleurScore > alpha)
                    {
                        alpha = meilleurScore;
                    }

                    // coupure : pas la peine de continuer
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return meilleurScore;
            }
            else
            {
                // c'est l'adversaire, il veut le plus petit score
                int meilleurScore = int.MaxValue;

                for (int i = 0; i < colonnesJouables.Count; i++)
                {
                    int colonne = colonnesJouables[i];

                    Grille copie = CopierGrille(grille);
                    JouerCoup(copie, colonne, EtatCase.Joueur1);

                    int score = AlphaBeta(copie, profondeur - 1, alpha, beta, true);

                    if (score < meilleurScore)
                    {
                        meilleurScore = score;
                    }
                    if (meilleurScore < beta)
                    {
                        beta = meilleurScore;
                    }

                    // coupure : pas la peine de continuer
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return meilleurScore;
            }
        }

        // donne un score a la position : score de l'ia moins score de l'adversaire
        // un groupe de 2 vaut 1, un groupe de 3 vaut 2, un groupe de 4 vaut 4... (poids 2^(t-2))
        private int Evaluer(Grille grille)
        {
            int scoreIA = 0;
            int scoreAdversaire = 0;

            // on regarde les groupes de taille 2 jusqu'a K
            for (int t = 2; t <= nbJetonsPourGagner; t++)
            {
                // poids = 2^(t-2), calcule avec une boucle au lieu de Math.Pow
                int poids = 1;
                for (int p = 0; p < t - 2; p++)
                {
                    poids = poids * 2;
                }

                int nbIA = CompterAlignements(grille, EtatCase.Joueur2, t);
                int nbAdversaire = CompterAlignements(grille, EtatCase.Joueur1, t);

                scoreIA = scoreIA + poids * nbIA;
                scoreAdversaire = scoreAdversaire + poids * nbAdversaire;
            }

            return scoreIA - scoreAdversaire;
        }

        // compte les groupes de "taille" jetons alignes pour un joueur
        // on regarde les 4 directions, comme dans VérifierAlignements de Grille.cs
        private int CompterAlignements(Grille grille, EtatCase joueur, int taille)
        {
            int compteur = 0;

            for (int i = 0; i < grille.Lignes; i++)
            {
                for (int j = 0; j < grille.Colonnes; j++)
                {
                    // on part de la case (i,j), si elle n'est pas au bon joueur on passe
                    if (grille.Tableau[i][j] != joueur)
                    {
                        continue;
                    }

                    // horizontale (vers la droite)
                    if (j + taille <= grille.Colonnes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < taille; k++)
                        {
                            if (grille.Tableau[i][j + k] != joueur)
                            {
                                aligne = false;
                                break;
                            }
                        }
                        if (aligne)
                        {
                            compteur++;
                        }
                    }

                    // verticale (vers le bas)
                    if (i + taille <= grille.Lignes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < taille; k++)
                        {
                            if (grille.Tableau[i + k][j] != joueur)
                            {
                                aligne = false;
                                break;
                            }
                        }
                        if (aligne)
                        {
                            compteur++;
                        }
                    }

                    // diagonale qui descend (bas-droite)
                    if (i + taille <= grille.Lignes && j + taille <= grille.Colonnes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < taille; k++)
                        {
                            if (grille.Tableau[i + k][j + k] != joueur)
                            {
                                aligne = false;
                                break;
                            }
                        }
                        if (aligne)
                        {
                            compteur++;
                        }
                    }

                    // diagonale qui monte (haut-droite)
                    if (i - taille + 1 >= 0 && j + taille <= grille.Colonnes)
                    {
                        bool aligne = true;
                        for (int k = 1; k < taille; k++)
                        {
                            if (grille.Tableau[i - k][j + k] != joueur)
                            {
                                aligne = false;
                                break;
                            }
                        }
                        if (aligne)
                        {
                            compteur++;
                        }
                    }
                }
            }

            return compteur;
        }

        // liste des colonnes pas encore pleines (la case du haut est vide)
        private List<int> ColonnesJouables(Grille grille)
        {
            List<int> colonnes = new List<int>();
            for (int j = 0; j < grille.Colonnes; j++)
            {
                if (grille.Tableau[0][j] == EtatCase.Vide)
                {
                    colonnes.Add(j);
                }
            }
            return colonnes;
        }

        // fait tomber un jeton dans une colonne (gravite)
        // renvoie la ligne ou il s'est pose, ou -1 si la colonne est pleine
        private int JouerCoup(Grille grille, int colonne, EtatCase joueur)
        {
            for (int ligne = grille.Lignes - 1; ligne >= 0; ligne--)
            {
                if (grille.Tableau[ligne][colonne] == EtatCase.Vide)
                {
                    grille.ChangerValeurCase(ligne, colonne, joueur);
                    return ligne;
                }
            }
            return -1; // colonne pleine
        }

        // copie la grille pour tester des coups sans abimer la partie
        private Grille CopierGrille(Grille grille)
        {
            Grille copie = new Grille(grille.Lignes, grille.Colonnes);
            for (int i = 0; i < grille.Lignes; i++)
            {
                for (int j = 0; j < grille.Colonnes; j++)
                {
                    copie.Tableau[i][j] = grille.Tableau[i][j];
                }
            }
            return copie;
        }
    }
}