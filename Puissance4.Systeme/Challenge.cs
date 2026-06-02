using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.Generic;
using System.ComponentModel;

namespace Puissance4.Systeme
{
    // Cette classe gère le mode Challenge : une série de parties avec un score global.
    // Sur la maquette, on voit "Partie 1, Partie 2..." avec le total des victoires.
    public class Challenge : INotifyPropertyChanged
    {
        // Le numéro de la partie en cours (1, 2, 3...).
        private int _numeroPartieEnCours;

        // Le score total de chaque joueur sur tout le challenge.
        private int _scoreJoueur1;
        private int _scoreJoueur2;

        // L'historique des résultats : pour chaque partie, qui a gagné (1, 2 ou 0).
        // On stocke ça dans deux listes parallèles, simples à parcourir.
        private List<int> _victoiresJoueur1ParPartie; // 1 si J1 a gagné cette partie, sinon 0
        private List<int> _victoiresJoueur2ParPartie; // 1 si J2 a gagné cette partie, sinon 0

        // Constructeur : on démarre au début, scores à zéro.
        public Challenge()
        {
            _numeroPartieEnCours = 1;
            _scoreJoueur1 = 0;
            _scoreJoueur2 = 0;
            _victoiresJoueur1ParPartie = new List<int>();
            _victoiresJoueur2ParPartie = new List<int>();
        }

        // ---- Propriétés en lecture pour le front ----

        public int NumeroPartieEnCours
        {
            get { return _numeroPartieEnCours; }
        }

        public int ScoreJoueur1
        {
            get { return _scoreJoueur1; }
        }

        public int ScoreJoueur2
        {
            get { return _scoreJoueur2; }
        }

        // Enregistre le résultat d'une partie terminée.
        // vainqueur = 1, 2 ou 0 (match nul).
        public void EnregistrerResultat(int vainqueur)
        {
            if (vainqueur == 1)
            {
                _scoreJoueur1 = _scoreJoueur1 + 1;
                _victoiresJoueur1ParPartie.Add(1);
                _victoiresJoueur2ParPartie.Add(0);
            }
            else if (vainqueur == 2)
            {
                _scoreJoueur2 = _scoreJoueur2 + 1;
                _victoiresJoueur1ParPartie.Add(0);
                _victoiresJoueur2ParPartie.Add(1);
            }
            else
            {
                // Match nul : personne ne marque, mais on garde une trace.
                _victoiresJoueur1ParPartie.Add(0);
                _victoiresJoueur2ParPartie.Add(0);
            }

            OnPropertyChanged("ScoreJoueur1");
            OnPropertyChanged("ScoreJoueur2");
        }

        // Le clic sur "Relancer une partie" : on passe à la partie suivante.
        public void RelancerUnePartie()
        {
            _numeroPartieEnCours = _numeroPartieEnCours + 1;
            OnPropertyChanged("NumeroPartieEnCours");
        }

        // Le clic sur "Finir le Challenge" : on renvoie le numéro du vainqueur final.
        // Renvoie 1 ou 2, ou 0 en cas d'égalité.
        public int FinirLeChallenge()
        {
            if (_scoreJoueur1 > _scoreJoueur2)
            {
                return 1;
            }
            if (_scoreJoueur2 > _scoreJoueur1)
            {
                return 2;
            }
            return 0; // égalité
        }

        // ---- Accès à l'historique des parties pour le tableau de la maquette ----

        // Renvoie le nombre de parties déjà jouées dans le challenge.
        public int NombrePartiesJouees()
        {
            return _victoiresJoueur1ParPartie.Count;
        }

        // Renvoie le résultat du joueur 1 à la partie numéro i (0 ou 1).
        public int ResultatJoueur1(int i)
        {
            return _victoiresJoueur1ParPartie[i];
        }

        // Renvoie le résultat du joueur 2 à la partie numéro i (0 ou 1).
        public int ResultatJoueur2(int i)
        {
            return _victoiresJoueur2ParPartie[i];
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