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
    /// Logique d'interaction pour FenetreJeu.xaml
    /// </summary>
    public partial class FenetreJeu : Window
    {
        public Joueur Joueur1 { get; set; }
        public Joueur Joueur2 { get; set; }
        public Configuration Config { get; set; }
        public Partie Partie { get; set; }
        public FenetreJeu(Joueur J1, Joueur J2, Configuration config)
        {
            InitializeComponent();
            Joueur1 = J1;
            Joueur2 = J2;
            Config = config;
            Partie = new Partie(Joueur1, Joueur2, Config);

            DessinerGrille();
        }

        private void BtnQuitter_Click(object sender, RoutedEventArgs e)
        {
            FenetreAccueil fenetreAccueil = new FenetreAccueil();
            fenetreAccueil.Show();
            this.Close();
        }

        private void DessinerGrille()
        {
            int taille = 40;

            if (Partie.Grille.Lignes >= 8)
            {
                Row8.Height = new GridLength(1, GridUnitType.Star);
                Row9.Height = new GridLength(1, GridUnitType.Star);
                Column8.Width = new GridLength(1, GridUnitType.Star);
                Column9.Width = new GridLength(1, GridUnitType.Star);
                taille = 32;
                if (Partie.Grille.Lignes == 10)
                {
                    Row10.Height = new GridLength(1, GridUnitType.Star);
                    Row11.Height = new GridLength(1, GridUnitType.Star);
                    Column10.Width = new GridLength(1, GridUnitType.Star);
                    Column11.Width = new GridLength(1, GridUnitType.Star);
                    Column12.Width = new GridLength(1, GridUnitType.Star);
                    taille = 24;
                }
            }

            for (int i = 0; i < Partie.Grille.Lignes; i++)
            {
                var couleur = new BrushConverter();
                for (int j = 0; j < Partie.Grille.Colonnes; j++)
                {
                    Border caseGrille = new Border
                    {
                        Background = (Brush)couleur.ConvertFromString("#cbc7b7")!,
                        Width = taille,
                        Height = taille,
                        Margin = new Thickness(2),
                        CornerRadius = new CornerRadius(5)
                    };

                    Ellipse jeton = new Ellipse
                    {
                        Fill = Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    caseGrille.Child = jeton;
                    Grid.SetRow(caseGrille, i+1);
                    Grid.SetColumn(caseGrille, j);
                    GridTableJeu.Children.Add(caseGrille);
                }
            }
        }
    }
}