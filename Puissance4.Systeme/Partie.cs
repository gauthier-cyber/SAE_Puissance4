using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Reflection.Metadata;

namespace Puissance4.Systeme
{
    // Cette classe représente une partie de Puissance 4 en cours.
    // Elle relie le plateau, les deux joueurs, le tour par tour,
    // l'historique des actions et les statistiques.
    // C'est surtout cette classe que le front-end va utiliser.
    public class Partie : INotifyPropertyChanged
    {
        private Plateau _plateau;
        private Parametres _parametres;
        private Joueur _joueur1;
        private Joueur _joueur2;

        // Le joueur dont c'est le tour (1 ou 2).
        private int _joueurCourant;

        // Indique si la partie est terminée.
        private bool _partieTerminee;

        // Le vainqueur : 1, 2 ou 0 (match nul ou partie pas finie).
        private int _vainqueur;

        // L'historique des actions, par exemple "[Joueur 1] joue en E".
        // On utilise une List<string> car c'est simple et vu en cours.
        private List<string> _historique;

        // Les statistiques de cette partie (pour l'écran de performances).
        private StatistiquesPartie _statistiques;

        // Les lettres du clavier utilisées pour nommer les colonnes (A Z E R T Y U ...),
        // comme sur la maquette. On les utilise pour écrire l'historique.
        private string[] _lettresColonnes = { "A", "Z", "E", "R", "T", "Y", "U", "I", "O", "P", "Q", "S" };

        // Constructeur : on crée une partie à partir des paramètres et des deux joueurs.
        public Partie(Parametres parametres, Joueur joueur1, Joueur joueur2)
        {
            _parametres = parametres;
            _joueur1 = joueur1;
            _joueur2 = joueur2;

            // On crée le plateau à la bonne taille.
            _plateau = new Plateau(parametres.NombreLignes, parametres.NombreColonnes, parametres.JetonsAAligner);

            _joueurCourant = 1;     // par convention, le joueur 1 commence
            _partieTerminee = false;
            _vainqueur = 0;
            _historique = new List<string>();
            _statistiques = new StatistiquesPartie();

            // On note dans l'historique que les joueurs ont rejoint la partie.
            _historique.Add(_joueur1.Nom + " a rejoint");
            _historique.Add(_joueur2.Nom + " a rejoint");
        }

        // ---- Propriétés en lecture pour le front ----

        public Plateau Plateau
        {
            get { return _plateau; }
        }

        public Parametres Parametres
        {
            get { return _parametres; }
        }

        public Joueur Joueur1
        {
            get { return _joueur1; }
        }

        public Joueur Joueur2
        {
            get { return _joueur2; }
        }

        public int JoueurCourant
        {
            get { return _joueurCourant; }
        }

        public bool PartieTerminee
        {
            get { return _partieTerminee; }
        }

        public int Vainqueur
        {
            get { return _vainqueur; }
        }

        public StatistiquesPartie Statistiques
        {
            get { return _statistiques; }
        }

        // Renvoie l'objet Joueur dont c'est le tour (pratique pour le front).
        public Joueur JoueurCourantObjet()
        {
            if (_joueurCourant == 1)
            {
                return _joueur1;
            }
            return _joueur2;
        }

        // Renvoie un texte du genre "C'est au tour de [Joueur 1]" pour le titre.
        public string TexteTourEnCours()
        {
            return "C'est au tour de " + JoueurCourantObjet().Nom;
        }

        // Le coeur de la partie : on essaie de jouer dans une colonne.
        // Renvoie vrai si le coup a été joué, faux si le coup est impossible
        // (colonne pleine ou partie déjà terminée).
        public bool Jouer(int colonne)
        {
            // Si la partie est finie, on ne joue plus.
            if (_partieTerminee == true)
            {
                return false;
            }

            // On demande au plateau de poser le jeton (avec la gravité).
            int ligne = _plateau.JouerColonne(colonne, _joueurCourant);

            // Si le plateau renvoie -1, le coup était impossible.
            if (ligne == -1)
            {
                return false;
            }

            // On enregistre le coup dans les statistiques.
            _statistiques.EnregistrerCoup(_joueurCourant, colonne);

            // On ajoute une ligne dans l'historique : "[Joueur 1] joue en E".
            string lettre = LettreDeColonne(colonne);
            _historique.Add(JoueurCourantObjet().Nom + " joue en " + lettre);

            // On vérifie si le joueur courant vient de gagner.
            if (_plateau.VerifierVictoire(_joueurCourant) == true)
            {
                _partieTerminee = true;
                _vainqueur = _joueurCourant;
                _statistiques.EnregistrerVictoire(_joueurCourant, colonne);
                _historique.Add(JoueurCourantObjet().Nom + " gagne la partie");

                // On met à jour les statistiques globales des deux joueurs.
                if (_vainqueur == 1)
                {
                    _joueur1.AjouterVictoire(_statistiques.CoupsJoueur1);
                    _joueur2.AjouterDefaite();
                }
                else
                {
                    _joueur2.AjouterVictoire(_statistiques.CoupsJoueur2);
                    _joueur1.AjouterDefaite();
                }

                OnPropertyChanged("PartieTerminee");
                OnPropertyChanged("Vainqueur");
                return true;
            }

            // Sinon, on vérifie si la grille est pleine (match nul).
            if (_plateau.GrillePleine() == true)
            {
                _partieTerminee = true;
                _vainqueur = 0;
                _statistiques.EnregistrerMatchNul();
                _historique.Add("Match nul, la grille est pleine");
                OnPropertyChanged("PartieTerminee");
                return true;
            }

            // La partie continue : on passe au joueur suivant.
            ChangerDeJoueur();
            return true;
        }

        // Passe la main à l'autre joueur et prévient l'interface.
        public void ChangerDeJoueur()
        {
            if (_joueurCourant == 1)
            {
                _joueurCourant = 2;
            }
            else
            {
                _joueurCourant = 1;
            }
            OnPropertyChanged("JoueurCourant");
        }

        // Cette méthode est appelée par le front quand le temps de réflexion est écoulé.
        // Comme demandé dans la maquette, le tour passe simplement à l'autre joueur.
        // (Le front affichera l'alerte visuelle de son côté.)
        public void TempsEcoule()
        {
            if (_partieTerminee == false)
            {
                _historique.Add(JoueurCourantObjet().Nom + " a depasse le temps");
                ChangerDeJoueur();
            }
        }

        // ---- Gestion de l'historique pour le front ----

        // Renvoie le nombre de lignes dans l'historique.
        public int NombreLignesHistorique()
        {
            return _historique.Count;
        }

        // Renvoie une ligne précise de l'historique.
        public string LireLigneHistorique(int i)
        {
            return _historique[i];
        }

        // Renvoie la dernière action jouée (utile pour l'afficher en grand).
        public string DerniereAction()
        {
            if (_historique.Count == 0)
            {
                return "";
            }
            return _historique[_historique.Count - 1];
        }

        // Donne la lettre de clavier qui correspond à une colonne (0 -> A, 1 -> Z, ...).
        public string LettreDeColonne(int colonne)
        {
            // On vérifie qu'on a bien une lettre prévue pour cette colonne.
            if (colonne >= 0 && colonne < _lettresColonnes.Length)
            {
                return _lettresColonnes[colonne];
            }
            // Sinon on renvoie juste le numéro de colonne en texte.
            return (colonne + 1).ToString();
        }

        // Fait l'inverse : à partir d'une lettre tapée au clavier, retrouve la colonne.
        // Renvoie -1 si la lettre n'est pas une colonne valide.
        public int ColonneDeLettre(string lettre)
        {
            for (int i = 0; i < _lettresColonnes.Length; i++)
            {
                // On compare sans tenir compte des majuscules/minuscules.
                if (_lettresColonnes[i].ToUpper() == lettre.ToUpper())
                {
                    // On vérifie aussi que cette colonne existe dans la grille actuelle.
                    if (i < _plateau.NombreColonnes)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        // Partie technique de INotifyPropertyChanged (recopiée du cours).
        protected void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}