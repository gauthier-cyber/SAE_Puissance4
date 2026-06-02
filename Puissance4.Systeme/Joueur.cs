using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;

namespace Puissance4.Systeme
{
    // Cette classe représente un joueur (humain ou IA plus tard).
    // Elle implémente INotifyPropertyChanged (vu en cours) pour que
    // l'interface se mette à jour automatiquement quand une valeur change.
    public class Joueur : INotifyPropertyChanged
    {
        // Les attributs privés (commencent par _ comme dans le cours)
        private string _nom;
        private int _numero;       // 1 pour le joueur 1, 2 pour le joueur 2
        private string _couleur;   // par exemple "Jaune" ou "Rouge"
        private bool _estIA;       // vrai si ce joueur est géré par l'ordinateur

        // Les statistiques globales du joueur (utiles pour l'écran de performances)
        private int _victoires;
        private int _defaites;
        private int _totalCoupsJoues;   // total de jetons posés sur toutes ses victoires

        // Constructeur : on crée un joueur en donnant son nom et son numéro.
        public Joueur(string nom, int numero)
        {
            _nom = nom;
            _numero = numero;
            _couleur = "";
            _estIA = false;
            _victoires = 0;
            _defaites = 0;
            _totalCoupsJoues = 0;
        }

        // Propriété Nom : si on change le nom, on prévient l'interface.
        public string Nom
        {
            get { return _nom; }
            set
            {
                _nom = value;
                OnPropertyChanged("Nom");
            }
        }

        // Propriété Numero (1 ou 2). En lecture seule car le numéro ne change pas.
        public int Numero
        {
            get { return _numero; }
        }

        // Propriété Couleur du jeton.
        public string Couleur
        {
            get { return _couleur; }
            set
            {
                _couleur = value;
                OnPropertyChanged("Couleur");
            }
        }

        // Propriété EstIA : indique si ce joueur est l'ordinateur.
        public bool EstIA
        {
            get { return _estIA; }
            set
            {
                _estIA = value;
                OnPropertyChanged("EstIA");
            }
        }

        // Nombre de victoires du joueur.
        public int Victoires
        {
            get { return _victoires; }
            set
            {
                _victoires = value;
                OnPropertyChanged("Victoires");
                // Le ratio dépend des victoires, donc on prévient aussi pour lui.
                OnPropertyChanged("Ratio");
            }
        }

        // Nombre de défaites du joueur.
        public int Defaites
        {
            get { return _defaites; }
            set
            {
                _defaites = value;
                OnPropertyChanged("Defaites");
                OnPropertyChanged("Ratio");
            }
        }

        // Total des coups joués lors de ses victoires (pour la moyenne).
        public int TotalCoupsJoues
        {
            get { return _totalCoupsJoues; }
            set
            {
                _totalCoupsJoues = value;
                OnPropertyChanged("TotalCoupsJoues");
                OnPropertyChanged("MoyenneCoups");
            }
        }

        // Le ratio de victoires en pourcentage (entre 0 et 100).
        // C'est une propriété calculée, donc juste un get.
        public int Ratio
        {
            get
            {
                int totalParties = _victoires + _defaites;
                if (totalParties == 0)
                {
                    return 0; // on évite la division par zéro
                }
                // On calcule le pourcentage de victoires.
                return (_victoires * 100) / totalParties;
            }
        }

        // La moyenne du nombre de coups par victoire.
        public int MoyenneCoups
        {
            get
            {
                if (_victoires == 0)
                {
                    return 0;
                }
                return _totalCoupsJoues / _victoires;
            }
        }

        // Méthode appelée quand le joueur gagne une partie.
        // nbCoups = le nombre de jetons qu'il a posés dans cette partie.
        public void AjouterVictoire(int nbCoups)
        {
            Victoires = _victoires + 1;
            TotalCoupsJoues = _totalCoupsJoues + nbCoups;
        }

        // Méthode appelée quand le joueur perd une partie.
        public void AjouterDefaite()
        {
            Defaites = _defaites + 1;
        }

        // Partie technique de INotifyPropertyChanged (recopiée du cours).
        // Elle prévient l'interface qu'une propriété a changé.
        protected void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }

        // L'événement qui sera écouté par l'interface.
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}