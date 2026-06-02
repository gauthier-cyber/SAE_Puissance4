using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;

namespace Puissance4.Systeme
{
    // Cette classe contient tous les réglages choisis dans la fenêtre Réglage :
    // mode challenge, taille de la grille, nombre de jetons à aligner,
    // temps de réflexion et niveau de l'IA.
    // Elle implémente aussi INotifyPropertyChanged pour la liaison de données.
    public class Configuration : INotifyPropertyChanged
    {
        // Attributs privés
        private bool _modeChallenge;     // vrai = on enchaîne plusieurs parties
        private bool _modeDeuxJoueurs;   // vrai = "Jouer à 2", faux = "Jouer seul" (contre IA)
        private int _niveauIA;           // 0 = pas d'IA, 1 = Idiot, 2 = Intelligent
        private int _nombreLignes;       // hauteur de la grille
        private int _nombreColonnes;     // largeur de la grille
        private int _jetonsAAligner;     // nombre de jetons à aligner pour gagner (4, 5...)
        private bool _tempsLimite;       // vrai = un compte à rebours est actif
        private int _tempsParCoup;       // temps en secondes pour jouer un coup (ex : 10)

        // Constructeur : on met les valeurs par défaut (grille 6x7, aligner 4).
        public Configuration()
        {
            _modeChallenge = false;
            _modeDeuxJoueurs = true;
            _niveauIA = 0;
            _nombreLignes = 6;
            _nombreColonnes = 7;
            _jetonsAAligner = 4;
            _tempsLimite = false;
            _tempsParCoup = 10;
        }

        // Mode challenge activé ou non.
        public bool ModeChallenge
        {
            get { return _modeChallenge; }
            set
            {
                _modeChallenge = value;
                OnPropertyChanged("ModeChallenge");
            }
        }

        // Mode 2 joueurs (vrai) ou contre l'ordinateur (faux).
        public bool ModeDeuxJoueurs
        {
            get { return _modeDeuxJoueurs; }
            set
            {
                _modeDeuxJoueurs = value;
                OnPropertyChanged("ModeDeuxJoueurs");
            }
        }

        // Niveau de l'IA : 0 = aucune, 1 = Idiot, 2 = Intelligent.
        public int NiveauIA
        {
            get { return _niveauIA; }
            set
            {
                _niveauIA = value;
                OnPropertyChanged("NiveauIA");
            }
        }

        // Nombre de lignes de la grille.
        public int NombreLignes
        {
            get { return _nombreLignes; }
            set
            {
                _nombreLignes = value;
                OnPropertyChanged("NombreLignes");
            }
        }

        // Nombre de colonnes de la grille.
        public int NombreColonnes
        {
            get { return _nombreColonnes; }
            set
            {
                _nombreColonnes = value;
                OnPropertyChanged("NombreColonnes");
            }
        }

        // Nombre de jetons à aligner pour gagner.
        public int JetonsAAligner
        {
            get { return _jetonsAAligner; }
            set
            {
                _jetonsAAligner = value;
                OnPropertyChanged("JetonsAAligner");
            }
        }

        // Indique si le temps de réflexion est limité.
        public bool TempsLimite
        {
            get { return _tempsLimite; }
            set
            {
                _tempsLimite = value;
                OnPropertyChanged("TempsLimite");
            }
        }

        // Temps en secondes accordé pour jouer un coup.
        public int TempsParCoup
        {
            get { return _tempsParCoup; }
            set
            {
                _tempsParCoup = value;
                OnPropertyChanged("TempsParCoup");
            }
        }

        // Petite méthode pratique pour régler la taille de la grille en une fois.
        // Le front peut l'appeler quand on choisit "6 x 7", "8 x 9", etc.
        public void DefinirTaille(int lignes, int colonnes)
        {
            NombreLignes = lignes;
            NombreColonnes = colonnes;
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
