using System;
using System.Collections.Generic;

namespace Puissance4.Systeme
{

    //
    // Cette classe contient toute la logique de l'ordinateur (l'IA).
    // Elle ne touche JAMAIS a l'affichage : elle renvoie juste un numero de
    // colonne (un int) que la fenetre de jeu utilisera pour poser le jeton.
    //
    // Il y a deux niveaux possibles :
    //
    //   - Idiot       : on choisit une colonne AU HASARD parmi les colonnes
    //                   encore jouables (pas pleines).
    //
    //   - Intelligent : on utilise l'algorithme Minimax avec elagage
    //                   alpha-beta (vu en SAE 2.2) pour choisir le meilleur
    //                   coup possible. On regarde plusieurs coups a l'avance.
    //
    // Convention importante (la meme que dans le front-end) :
    //   - L'IA joue toujours les jetons "Joueur2" (EtatCase.Joueur2).
    //   - L'adversaire (l'humain) joue les jetons "Joueur1" (EtatCase.Joueur1).
    public class IA
    {
        // Niveau de l'IA (Idiot ou Intelligent). On reutilise l'enum NiveauVirtuel
        // qui existe deja dans Joueur.cs.
        private NiveauVirtuel niveau;

        // Sert a tirer une colonne au hasard pour l'IA idiote.
        private Random generateur;

        // Profondeur de recherche de l'IA intelligente.
        // C'est le nombre de coups que l'IA regarde a l'avance.
        // Plus c'est grand, plus l'IA est forte mais plus c'est lent.
        // 4 ou 5 est un bon compromis pour rester rapide.
        private int profondeurMax;

        // Nombre de jetons a aligner pour gagner (4 par defaut, mais peut etre 5).
        // On le recupere depuis la configuration de la partie.
        private int nbJetonsPourGagner;

        // Constructeur : on cree une IA en precisant son niveau et le nombre de
        // jetons a aligner (qui vient de la Configuration de la partie).
        public IA(NiveauVirtuel niveau, int nbJetonsPourGagner)
        {
            this.niveau = niveau;
            this.generateur = new Random();
            this.nbJetonsPourGagner = nbJetonsPourGagner;
            this.profondeurMax = 4;
        }

        // ====================================================================
        //  METHODE PRINCIPALE APPELEE PAR LE FRONT-END
        // ====================================================================
        //
        // On lui passe la grille actuelle de la partie, et elle renvoie le
        // numero de la colonne ou l'IA veut jouer.
        // Le front-end n'a plus qu'a poser le jeton dans cette colonne.
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

        // ====================================================================
        //  IA IDIOTE : une colonne au hasard parmi les colonnes jouables
        // ====================================================================
        private int ChoisirColonneIdiot(Grille grille)
        {
            // On recupere la liste des colonnes ou on peut encore jouer.
            List<int> colonnesJouables = ColonnesJouables(grille);

            // Si jamais aucune colonne n'est jouable (grille pleine), on
            // renvoie -1 pour dire "pas de coup possible".
            if (colonnesJouables.Count == 0)
            {
                return -1;
            }

            // On tire un index au hasard dans la liste et on renvoie cette colonne.
            int index = generateur.Next(colonnesJouables.Count);
            return colonnesJouables[index];
        }

        // ====================================================================
        //  IA INTELLIGENTE : on choisit le meilleur coup avec alpha-beta
        // ====================================================================
        //
        // Pour chaque colonne jouable, on simule le coup de l'IA, puis on
        // calcule le score de la position avec Minimax (alpha-beta). On garde
        // la colonne qui donne le meilleur score.
        private int ChoisirColonneIntelligent(Grille grille)
        {
            List<int> colonnesJouables = ColonnesJouables(grille);

            // Grille pleine : aucun coup possible.
            if (colonnesJouables.Count == 0)
            {
                return -1;
            }

            // On part avec un tres mauvais score, qu'on va essayer d'ameliorer.
            int meilleurScore = int.MinValue;
            int meilleureColonne = colonnesJouables[0];

            // Les bornes de l'elagage alpha-beta. Au depart : alpha = -infini,
            // beta = +infini. On utilise les plus petites/grandes valeurs d'un int.
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            // On essaie chaque colonne jouable.
            for (int i = 0; i < colonnesJouables.Count; i++)
            {
                int colonne = colonnesJouables[i];

                // On fait une copie de la grille pour ne pas abimer la vraie partie.
                Grille copie = CopierGrille(grille);

                // On simule le coup de l'IA dans cette colonne.
                JouerCoup(copie, colonne, EtatCase.Joueur2);

                // C'est maintenant a l'adversaire de jouer, donc le prochain
                // niveau est un niveau MIN -> estMax = false.
                int score = AlphaBeta(copie, profondeurMax - 1, alpha, beta, false);

                // Si ce coup est meilleur que ce qu'on avait, on le retient.
                if (score > meilleurScore)
                {
                    meilleurScore = score;
                    meilleureColonne = colonne;
                }

                // On met a jour alpha (le meilleur score garanti pour l'IA).
                if (meilleurScore > alpha)
                {
                    alpha = meilleurScore;
                }
            }

            return meilleureColonne;
        }

        // ====================================================================
        //  ALGORITHME MINIMAX AVEC ELAGAGE ALPHA-BETA
        // ====================================================================
        //
        // Cette methode renvoie le score d'une position en regardant plusieurs
        // coups a l'avance.
        //   - profondeur : combien de coups il reste a explorer.
        //   - alpha : meilleur score deja garanti pour l'IA (MAX).
        //   - beta  : meilleur score deja garanti pour l'adversaire (MIN).
        //   - estMax : true si c'est a l'IA de jouer, false si c'est l'adversaire.
        private int AlphaBeta(Grille grille, int profondeur, int alpha, int beta, bool estMax)
        {
            // On regarde si quelqu'un a gagne sur cette position.
            EtatCase gagnant = grille.VérifierAlignements(nbJetonsPourGagner);

            // Si l'IA a gagne, c'est une tres bonne position : gros score positif.
            // On enleve la profondeur pour preferer les victoires rapides.
            if (gagnant == EtatCase.Joueur2)
            {
                return 100000 + profondeur;
            }

            // Si l'adversaire a gagne, c'est une tres mauvaise position.
            if (gagnant == EtatCase.Joueur1)
            {
                return -100000 - profondeur;
            }

            // Cas d'arret : plus de profondeur a explorer, ou plus de coup possible.
            List<int> colonnesJouables = ColonnesJouables(grille);
            if (profondeur == 0 || colonnesJouables.Count == 0)
            {
                // On evalue la position avec notre heuristique.
                return Evaluer(grille);
            }

            if (estMax)
            {
                // Noeud MAX : c'est l'IA qui joue, elle cherche le plus grand score.
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

                    // Beta-coupure : MIN ne choisira jamais ce noeud, on arrete.
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return meilleurScore;
            }
            else
            {
                // Noeud MIN : c'est l'adversaire qui joue, il cherche le plus petit score.
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

                    // Alpha-coupure : MAX ne choisira jamais ce noeud, on arrete.
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return meilleurScore;
            }
        }

        // ====================================================================
        //  HEURISTIQUE : evalue une position (positif = bon pour l'IA)
        // ====================================================================
        //
        // On calcule le score de l'IA moins le score de l'adversaire.
        // Le score d'un joueur = somme, pour chaque taille de groupe t (de 2 a K),
        // du nombre de groupes de taille t multiplie par un poids 2^(t-2).
        // Ainsi : groupe de 2 -> poids 1, groupe de 3 -> poids 2, groupe de 4 -> poids 4...
        private int Evaluer(Grille grille)
        {
            int scoreIA = 0;
            int scoreAdversaire = 0;

            // On parcourt toutes les tailles de groupe de 2 jusqu'a K (nbJetonsPourGagner).
            for (int t = 2; t <= nbJetonsPourGagner; t++)
            {
                // Poids = 2^(t-2). On le calcule avec une simple boucle pour
                // rester sur des choses vues en cours (pas de Math.Pow).
                int poids = 1;
                for (int p = 0; p < t - 2; p++)
                {
                    poids = poids * 2;
                }

                // On compte les groupes de taille t pour chaque joueur.
                int nbIA = CompterAlignements(grille, EtatCase.Joueur2, t);
                int nbAdversaire = CompterAlignements(grille, EtatCase.Joueur1, t);

                // On ajoute leur contribution au score de chaque joueur.
                scoreIA = scoreIA + poids * nbIA;
                scoreAdversaire = scoreAdversaire + poids * nbAdversaire;
            }

            // Heuristique globale = score IA - score adversaire.
            return scoreIA - scoreAdversaire;
        }

        // ====================================================================
        //  COMPTER LES GROUPES DE JETONS ALIGNES
        // ====================================================================
        //
        // On compte combien de groupes de "taille" jetons consecutifs du joueur
        // donne existent sur la grille, dans les 4 directions :
        // horizontale, verticale, diagonale descendante et diagonale montante.
        // C'est la meme idee que VérifierAlignements dans Grille.cs, mais ici
        // on compte les groupes au lieu de juste detecter une victoire.
        private int CompterAlignements(Grille grille, EtatCase joueur, int taille)
        {
            int compteur = 0;

            for (int i = 0; i < grille.Lignes; i++)
            {
                for (int j = 0; j < grille.Colonnes; j++)
                {
                    // On regarde un groupe qui commence a la case (i, j).
                    // Si la premiere case n'est pas au bon joueur, on passe.
                    if (grille.Tableau[i][j] != joueur)
                    {
                        continue;
                    }

                    // 1. Horizontale (vers la droite)
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

                    // 2. Verticale (vers le bas)
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

                    // 3. Diagonale descendante (vers le bas et la droite)
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

                    // 4. Diagonale montante (vers le haut et la droite)
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

        // ====================================================================
        //  OUTILS UTILITAIRES (utilises par l'IA)
        // ====================================================================

        // Renvoie la liste des colonnes ou on peut encore jouer (pas pleines).
        // Une colonne est jouable si sa case du haut (ligne 0) est vide.
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

        // Simule la chute d'un jeton dans une colonne (gestion de la gravite).
        // Le jeton se pose sur la ligne libre la plus basse de la colonne.
        // Renvoie la ligne ou le jeton s'est pose, ou -1 si la colonne est pleine.
        private int JouerCoup(Grille grille, int colonne, EtatCase joueur)
        {
            // On part du bas de la grille et on remonte jusqu'a trouver une case vide.
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

        // Fait une copie complete de la grille pour pouvoir simuler des coups
        // sans modifier la vraie grille de la partie.
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