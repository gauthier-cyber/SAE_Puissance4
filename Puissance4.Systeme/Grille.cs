using System;

namespace Puissance4.Systeme
{
    // La classe Grille represente la grille de jeu du Puissance 4.
    // C'est le coeur du moteur : elle gere le tableau, la gravite des jetons
    // et la verification des alignements pour savoir s'il y a un gagnant.
    // Attention : aucune partie graphique ici, on manipule juste des nombres.
    public class Grille
    {
        // La grille est un tableau a 2 dimensions d'entiers.
        // 0 = case vide, 1 = jeton du joueur 1, 2 = jeton du joueur 2.
        public int[,] Grille { get; private set; }

        // Nombre de lignes et de colonnes de la grille.
        public int NbLignes { get; private set; }
        public int NbColonnes { get; private set; }

        // Nombre de jetons a aligner pour gagner (ex: 4).
        public int NbJetonsAAligner { get; private set; }


        // Constructeur : on cree la grille a la bonne taille.
        // Toutes les cases sont mises a 0 (vide) automatiquement par C#.
        public Grille(int nbLignes, int nbColonnes, int nbJetonsAAligner)
        {
            NbLignes = nbLignes;
            NbColonnes = nbColonnes;
            NbJetonsAAligner = nbJetonsAAligner;

            Grille = new int[nbLignes, nbColonnes];
        }


        // Vide completement la grille pour recommencer une partie.
        public void Reinitialiser()
        {
            for (int ligne = 0; ligne < NbLignes; ligne++)
            {
                for (int colonne = 0; colonne < NbColonnes; colonne++)
                {
                    Grille[ligne, colonne] = 0;
                }
            }
        }


        // Verifie si on peut encore jouer dans une colonne.
        // On regarde simplement si la case tout en haut de la colonne est vide.
        public bool ColonneJouable(int colonne)
        {
            // On verifie d'abord que la colonne existe vraiment.
            if (colonne < 0 || colonne >= NbColonnes)
                return false;

            // La case du haut est a la ligne 0. Si elle est vide, la colonne n'est pas pleine.
            if (Grille[0, colonne] == 0)
                return true;
            else
                return false;
        }


        // Pose un jeton dans une colonne en respectant la gravite.
        // Le jeton tombe sur la ligne disponible la plus basse.
        // On renvoie le numero de la ligne ou le jeton s'est pose,
        // ou -1 si la colonne est pleine ou n'existe pas.
        public int PoserJeton(int colonne, int numeroJoueur)
        {
            // Si on ne peut pas jouer ici, on renvoie -1 pour signaler l'erreur.
            if (ColonneJouable(colonne) == false)
                return -1;

            // On part du bas de la grille (derniere ligne) et on remonte
            // jusqu'a trouver la premiere case vide.
            for (int ligne = NbLignes - 1; ligne >= 0; ligne--)
            {
                if (Grille[ligne, colonne] == 0)
                {
                    // On pose le jeton ici et on renvoie la ligne trouvee.
                    Grille[ligne, colonne] = numeroJoueur;
                    return ligne;
                }
            }

            // Normalement on n'arrive jamais ici, mais par securite on renvoie -1.
            return -1;
        }


        // Indique si la grille est entierement pleine.
        // Sert a detecter un match nul.
        public bool EstPleine()
        {
            // Il suffit de regarder la ligne du haut.
            // Si toutes ses cases sont occupees, alors la grille est pleine.
            for (int colonne = 0; colonne < NbColonnes; colonne++)
            {
                if (Grille[0, colonne] == 0)
                    return false;
            }
            return true;
        }


        // Verifie si le joueur donne vient de gagner.
        // On teste les 4 directions possibles a partir de chaque case.
        // On renvoie vrai des qu'on trouve un alignement suffisant.
        public bool ALigne(int numeroJoueur)
        {
            // On parcourt toutes les cases de la grille.
            for (int ligne = 0; ligne < NbLignes; ligne++)
            {
                for (int colonne = 0; colonne < NbColonnes; colonne++)
                {
                    // On ne teste que les cases qui appartiennent au joueur.
                    if (Grille[ligne, colonne] == numeroJoueur)
                    {
                        // Direction horizontale (vers la droite) : ligne fixe, colonne +1.
                        if (CompterDansDirection(ligne, colonne, 0, 1, numeroJoueur) >= NbJetonsAAligner)
                            return true;

                        // Direction verticale (vers le bas) : ligne +1, colonne fixe.
                        if (CompterDansDirection(ligne, colonne, 1, 0, numeroJoueur) >= NbJetonsAAligner)
                            return true;

                        // Diagonale qui descend vers la droite : ligne +1, colonne +1.
                        if (CompterDansDirection(ligne, colonne, 1, 1, numeroJoueur) >= NbJetonsAAligner)
                            return true;

                        // Diagonale qui descend vers la gauche : ligne +1, colonne -1.
                        if (CompterDansDirection(ligne, colonne, 1, -1, numeroJoueur) >= NbJetonsAAligner)
                            return true;
                    }
                }
            }

            // Aucun alignement trouve.
            return false;
        }


        // Compte combien de jetons du meme joueur sont alignes a partir d'une case,
        // dans une direction donnee.
        // depLigne et depColonne indiquent le deplacement a chaque pas
        // (par exemple 0 et 1 pour aller vers la droite).
        // Cette methode est privee car elle sert juste a aider ALigne.
        private int CompterDansDirection(int ligne, int colonne, int depLigne, int depColonne, int numeroJoueur)
        {
            int compte = 0;

            // Position de depart.
            int ligneCourante = ligne;
            int colonneCourante = colonne;

            // Tant qu'on reste dans la grille ET que la case appartient au joueur,
            // on avance dans la direction choisie et on compte.
            while (ligneCourante >= 0 && ligneCourante < NbLignes &&
                   colonneCourante >= 0 && colonneCourante < NbColonnes &&
                   Grille[ligneCourante, colonneCourante] == numeroJoueur)
            {
                compte++;
                ligneCourante = ligneCourante + depLigne;
                colonneCourante = colonneCourante + depColonne;
            }

            return compte;
        }


        // Renvoie la liste des colonnes encore jouables.
        // Pratique pour l'IA "Idiot" qui devra choisir au hasard parmi ces colonnes.
        // On renvoie un tableau d'entiers contenant les numeros de colonnes valides.
        public int[] ColonnesJouables()
        {
            // On compte d'abord combien de colonnes sont jouables.
            int nbJouables = 0;
            for (int colonne = 0; colonne < NbColonnes; colonne++)
            {
                if (ColonneJouable(colonne))
                    nbJouables++;
            }

            // On cree un tableau a la bonne taille et on le remplit.
            int[] resultat = new int[nbJouables];
            int index = 0;
            for (int colonne = 0; colonne < NbColonnes; colonne++)
            {
                if (ColonneJouable(colonne))
                {
                    resultat[index] = colonne;
                    index++;
                }
            }

            return resultat;
        }
    }
}