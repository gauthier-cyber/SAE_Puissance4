using System;
using System.Reflection.Metadata;

namespace Puissance4.Systeme
{
    // La classe Partie pilote une partie complete de Puissance 4.
    // Elle utilise un Plateau pour la grille, garde les deux joueurs et les parametres,
    // gere le tour par tour et enregistre les infos pour le resume (premier coup,
    // coup decisif, duree, nombre de coups...).
    // Le front-end appelle surtout cette classe.
    public class Partie
    {
        // Le plateau de jeu (la grille).
        public Plateau Plateau { get; private set; }

        // Les deux joueurs de la partie.
        public Joueur Joueur1 { get; private set; }
        public Joueur Joueur2 { get; private set; }

        // Les parametres choisis (taille, temps, mode...).
        public Parametres Parametres { get; private set; }

        // Le joueur a qui c'est le tour de jouer.
        public Joueur JoueurCourant { get; private set; }

        // L'etat actuel de la partie (en cours, victoire J1, etc.).
        public EtatPartie Etat { get; private set; }

        // --- Donnees pour le resume des performances ---

        // Nombre de coups joues depuis le debut de la partie.
        public int NbCoupsJoues { get; private set; }

        // Indique quel joueur a joue le tout premier coup (utile pour "Premier coup : J2").
        public Joueur PremierJoueur { get; private set; }

        // Memorise le coup decisif (celui qui a fait gagner).
        // On garde le numero du joueur et le numero du coup.
        public int CoupDecisifNumeroJoueur { get; private set; }
        public int CoupDecisifNumeroCoup { get; private set; }

        // Pour mesurer la duree de la partie, on retient l'heure de debut.
        private DateTime heureDebut;
        // Et l'heure de fin une fois la partie terminee.
        private DateTime heureFin;


        // Constructeur : on prepare une nouvelle partie a partir des parametres.
        public Partie(Joueur joueur1, Joueur joueur2, Parametres parametres)
        {
            Joueur1 = joueur1;
            Joueur2 = joueur2;
            Parametres = parametres;

            // On cree le plateau a la taille demandee dans les parametres.
            Plateau = new Plateau(parametres.NbLignes, parametres.NbColonnes, parametres.NbJetonsAAligner);

            // On demarre la partie proprement dite.
            Demarrer();
        }


        // (Re)demarre une partie : grille vide, joueur 1 commence, compteurs a zero.
        public void Demarrer()
        {
            Plateau.Reinitialiser();

            // On s'assure que chaque joueur a le bon numero de jeton pour la grille.
            Joueur1.NumeroJeton = 1;
            Joueur2.NumeroJeton = 2;

            // Par convention le joueur 1 commence.
            JoueurCourant = Joueur1;
            PremierJoueur = Joueur1;

            Etat = EtatPartie.EnCours;
            NbCoupsJoues = 0;

            CoupDecisifNumeroJoueur = 0;
            CoupDecisifNumeroCoup = 0;

            // On note l'heure de depart pour calculer la duree plus tard.
            heureDebut = DateTime.Now;
        }


        // Joue un coup dans la colonne demandee pour le joueur courant.
        // Renvoie la ligne ou le jeton s'est pose, ou -1 si le coup est impossible.
        // Apres un coup valide, la methode met a jour l'etat et change de joueur.
        public int JouerCoup(int colonne)
        {
            // On ne joue pas si la partie est deja finie.
            if (Etat != EtatPartie.EnCours)
                return -1;

            // On essaie de poser le jeton du joueur courant.
            int ligne = Plateau.PoserJeton(colonne, JoueurCourant.NumeroJeton);

            // Si le coup est impossible (colonne pleine), on s'arrete la.
            if (ligne == -1)
                return -1;

            // Le coup est valide, on incremente le compteur de coups.
            NbCoupsJoues++;

            // On regarde si le joueur courant vient de gagner.
            if (Plateau.ALigne(JoueurCourant.NumeroJeton))
            {
                // On enregistre le coup decisif.
                CoupDecisifNumeroJoueur = JoueurCourant.NumeroJeton;
                CoupDecisifNumeroCoup = NbCoupsJoues;

                // On met l'etat selon le gagnant.
                if (JoueurCourant == Joueur1)
                    Etat = EtatPartie.VictoireJ1;
                else
                    Etat = EtatPartie.VictoireJ2;

                // On note l'heure de fin et on met a jour les statistiques.
                Terminer();
                return ligne;
            }

            // Pas de victoire : on verifie si la grille est pleine (match nul).
            if (Plateau.EstPleine())
            {
                Etat = EtatPartie.MatchNul;
                Terminer();
                return ligne;
            }

            // La partie continue : on passe la main a l'autre joueur.
            ChangerDeJoueur();
            return ligne;
        }


        // Change le joueur courant (passe de J1 a J2 ou inversement).
        private void ChangerDeJoueur()
        {
            if (JoueurCourant == Joueur1)
                JoueurCourant = Joueur2;
            else
                JoueurCourant = Joueur1;
        }


        // A appeler quand le temps de reflexion est ecoule pour le coup courant.
        // Comme demande dans la maquette, on passe simplement le tour a l'autre joueur.
        // Le front-end gere le minuteur (Timer) et appelle cette methode a la fin du temps.
        public void TempsEcoule()
        {
            // On ne fait rien si la partie est finie.
            if (Etat != EtatPartie.EnCours)
                return;

            // Le joueur a perdu son tour : on passe au joueur suivant.
            ChangerDeJoueur();
        }


        // Termine la partie : on note l'heure de fin et on met a jour les stats des joueurs.
        private void Terminer()
        {
            heureFin = DateTime.Now;

            // On met a jour les compteurs de victoires/defaites selon l'etat.
            if (Etat == EtatPartie.VictoireJ1)
            {
                Joueur1.NbVictoires++;
                Joueur2.NbDefaites++;

                // Pour la moyenne de coups par victoire, on ajoute les coups de cette partie.
                Joueur1.TotalCoupsJoues = Joueur1.TotalCoupsJoues + NbCoupsJoues;
            }
            else if (Etat == EtatPartie.VictoireJ2)
            {
                Joueur2.NbVictoires++;
                Joueur1.NbDefaites++;

                Joueur2.TotalCoupsJoues = Joueur2.TotalCoupsJoues + NbCoupsJoues;
            }
            // En cas de match nul, on ne change pas victoires/defaites.

            // Dans tous les cas, les deux joueurs ont joué une partie de plus.
            Joueur1.NbPartiesJouees++;
            Joueur2.NbPartiesJouees++;
        }


        // Renvoie la duree de la partie en secondes.
        // Si la partie est encore en cours, on calcule depuis le debut jusqu'a maintenant.
        public int DureeEnSecondes()
        {
            DateTime fin;

            if (Etat == EtatPartie.EnCours)
                fin = DateTime.Now;
            else
                fin = heureFin;

            // On calcule l'ecart entre les deux heures.
            TimeSpan ecart = fin - heureDebut;
            return (int)ecart.TotalSeconds;
        }


        // Renvoie une phrase a afficher pour le tour courant.
        // Exemple : "C'est au tour de Joueur 1".
        // Le front-end peut afficher ce texte directement.
        public string MessageTour()
        {
            return "C'est au tour de " + JoueurCourant.Nom;
        }


        // Indique si la partie est terminee (victoire ou match nul).
        public bool EstTerminee()
        {
            if (Etat == EtatPartie.EnCours)
                return false;
            else
                return true;
        }
    }
}