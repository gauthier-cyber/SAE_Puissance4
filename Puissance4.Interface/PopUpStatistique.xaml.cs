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
        public PopUpStatistique(Partie partie)
        {
            InitializeComponent();
            Partie = partie;

            // on affiche le gagnant et les coups (le dernier caractère est la lettre de la colonne)
            TxtGagnant.Text = Partie.Gagnant!.Nom;
            TxtPremierCoup.Text = Partie.PremierCoup!.Substring(0, Partie.PremierCoup.Length - 1) + " en " + Partie.PremierCoup[^1];
            TxtCoupDecisif.Text = Partie.CoupDecisif!.Substring(0, Partie.CoupDecisif.Length - 1) + " en " + Partie.CoupDecisif[^1];
            // on transforme la durée en minutes et secondes
            int minutes = (int)(Partie.Duree! / 60);
            int secondes = (int)(Partie.Duree! % 60);
            TxtDuree.Text = minutes.ToString() + "m" + secondes.ToString("D2");
            TxtCoups.Text = Partie.NombreCoups.ToString();

            // on met chaque texte à la couleur du joueur concerné
            TxtGagnant.Foreground = (Brush)new BrushConverter().ConvertFromString((Partie.Gagnant == Partie.Joueur1) ? Partie.Configuration.CouleurJoueur1 : Partie.Configuration.CouleurJoueur2)!;
            TxtPremierCoup.Foreground = (Brush)new BrushConverter().ConvertFromString((Partie.PremierCoup.Contains(Partie.Joueur1.Nom)) ? Partie.Configuration.CouleurJoueur1 : Partie.Configuration.CouleurJoueur2)!;
            TxtCoupDecisif.Foreground = (Brush)new BrushConverter().ConvertFromString((Partie.CoupDecisif.Contains(Partie.Joueur1.Nom)) ? Partie.Configuration.CouleurJoueur1 : Partie.Configuration.CouleurJoueur2)!;
        }

        public PopUpStatistique(Partie partie, bool contrasteMarque)
        {
            InitializeComponent();
            Partie = partie;

            // on affiche le gagnant et les coups (le dernier caractère est la lettre de la colonne)
            TxtGagnant.Text = Partie.Gagnant!.Nom;
            TxtPremierCoup.Text = Partie.PremierCoup!.Substring(0, Partie.PremierCoup.Length - 1) + " en " + Partie.PremierCoup[^1];
            TxtCoupDecisif.Text = Partie.CoupDecisif!.Substring(0, Partie.CoupDecisif.Length - 1) + " en " + Partie.CoupDecisif[^1];
            // on transforme la durée en minutes et secondes
            int minutes = (int)(Partie.Duree! / 60);
            int secondes = (int)(Partie.Duree! % 60);
            TxtDuree.Text = minutes.ToString() + "m" + secondes.ToString("D2");
            TxtCoups.Text = Partie.NombreCoups.ToString();

            // on met chaque texte à la couleur du joueur concerné
            TxtGagnant.Foreground = (Brush)new BrushConverter().ConvertFromString((Partie.Gagnant == Partie.Joueur1) ? Partie.Configuration.CouleurJoueur1 : Partie.Configuration.CouleurJoueur2)!;
            TxtPremierCoup.Foreground = (Brush)new BrushConverter().ConvertFromString((Partie.PremierCoup.Contains(Partie.Joueur1.Nom)) ? Partie.Configuration.CouleurJoueur1 : Partie.Configuration.CouleurJoueur2)!;
            TxtCoupDecisif.Foreground = (Brush)new BrushConverter().ConvertFromString((Partie.CoupDecisif.Contains(Partie.Joueur1.Nom)) ? Partie.Configuration.CouleurJoueur1 : Partie.Configuration.CouleurJoueur2)!;

            // on applique le mode contraste fort si besoin
            if (contrasteMarque)
            {
                this.Background = Brushes.White;
                this.FontFamily = new FontFamily("Verdana");
                this.Foreground = Brushes.Black;
            }
        }
    }
}