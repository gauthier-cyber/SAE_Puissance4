using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Puissance4.Systeme;

namespace Puissance4.Interface
{
    /// <summary>
    /// Logique d'interaction pour PopUpStatistique.xaml
    /// </summary>
    public partial class PopUpStatistique : Window
    {
        Partie Partie { get; set; }
        Challenge? Challenge { get; set; }

        // Ancien constructeur : partie simple, sans challenge.
        public PopUpStatistique(Partie partie)
        {
            InitializeComponent();
            Partie = partie;
            Challenge = null;

            RemplirStatistiques();
        }

        // Nouveau constructeur : partie en mode challenge.
        // On recoit en plus le challenge pour afficher le score.
        public PopUpStatistique(Partie partie, Challenge challenge)
        {
            InitializeComponent();
            Partie = partie;
            Challenge = challenge;

            RemplirStatistiques();
            AfficherScoreChallenge();
        }

        // On remplit toutes les statistiques communes (gagnant, coups, etc.).
        // C'est exactement ce que tu faisais avant, juste range dans une methode.
        private void RemplirStatistiques()
        {
            TxtGagnant.Text = Partie.Gagnant!.Nom;
            TxtPremierCoup.Text = Partie.PremierCoup!.Substring(0, Partie.PremierCoup.Length - 1) + " en " + Partie.PremierCoup[^1];
            TxtCoupDecisif.Text = Partie.CoupDecisif!.Substring(0, Partie.CoupDecisif.Length - 1) + " en " + Partie.CoupDecisif[^1];
            int minutes = (int)(Partie.Duree! / 60);
            int secondes = (int)(Partie.Duree! % 60);
            TxtDuree.Text = minutes.ToString() + "m" + secondes.ToString("D2");
            TxtCoups.Text = Partie.NombreCoups.ToString();

            TxtGagnant.Foreground = (Brush)new BrushConverter().ConvertFromString((Partie.Gagnant == Partie.Joueur1) ? Partie.Configuration.CouleurJoueur1 : Partie.Configuration.CouleurJoueur2)!;
            TxtPremierCoup.Foreground = (Brush)new BrushConverter().ConvertFromString((Partie.PremierCoup.Contains(Partie.Joueur1.Nom)) ? Partie.Configuration.CouleurJoueur1 : Partie.Configuration.CouleurJoueur2)!;
            TxtCoupDecisif.Foreground = (Brush)new BrushConverter().ConvertFromString((Partie.CoupDecisif.Contains(Partie.Joueur1.Nom)) ? Partie.Configuration.CouleurJoueur1 : Partie.Configuration.CouleurJoueur2)!;
        }

        // On affiche la ligne du score uniquement en mode challenge.
        private void AfficherScoreChallenge()
        {
            if (Challenge != null)
            {
                // On rend visibles les deux TextBlock caches dans le XAML.
                TxtBlockLibelleScore.Visibility = Visibility.Visible;
                TxtScore.Visibility = Visibility.Visible;

                // On ecrit le score sous la forme "2 - 1" (Joueur1 - Joueur2).
                TxtScore.Text = Challenge.ScoreJoueur1.ToString() + " - " + Challenge.ScoreJoueur2.ToString();
            }
        }
    }
}