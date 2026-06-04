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

                    int largeurForme = taille - 10;
                    switch (Partie.Configuration.FormeJoueur)
                    {
                        case "Rond":
                            Ellipse jetonRond = new Ellipse
                            {
                                Fill = Brushes.White,
                                Width = largeurForme,
                                Height = largeurForme,
                                Margin = new Thickness(5)
                            };

                            caseGrille.Child = jetonRond;
                            break;
                        case "Triangle":
                            Polygon jetonTriangle = new Polygon
                            {
                                Fill = Brushes.White,
                                Points = new PointCollection
                                {
                                    new Point(largeurForme / 2, 0),
                                    new Point(0, largeurForme),
                                    new Point(largeurForme, largeurForme)
                                },
                                Margin = new Thickness(5)
                            };

                            caseGrille.Child = jetonTriangle;
                            break;

                        case "Croix":
                            double tiers = largeurForme / 3;
                            double deuxTiers = 2 * tiers;

                            Polygon jetonCroix = new Polygon
                            {
                                Fill = Brushes.White,
                                Points = new PointCollection
                                {
                                    new Point(largeurForme / 2, 0),
                                    new Point(deuxTiers, tiers),
                                    new Point(largeurForme, largeurForme / 2),
                                    new Point(deuxTiers, deuxTiers),
                                    new Point(largeurForme / 2 , largeurForme),
                                    new Point(tiers, deuxTiers),
                                    new Point(0, largeurForme / 2),
                                    new Point(tiers, tiers)
                                },
                                Margin = new Thickness(5)
                            };

                            caseGrille.Child = jetonCroix;
                            break;

                        case "Etoile":
                            Polygon jetonEtoile = new Polygon
                            {
                                Fill = Brushes.White,
                                Points = new PointCollection
                                {
                                    new Point(largeurForme / 2, 0),
                                    new Point(largeurForme * 0.6, largeurForme * 0.35),
                                    new Point(largeurForme, largeurForme * 0.4),
                                    new Point(largeurForme * 0.7, largeurForme * 0.65),
                                    new Point(largeurForme * 0.8, largeurForme),
                                    new Point(largeurForme / 2, largeurForme * 0.8),
                                    new Point(largeurForme * 0.2, largeurForme),
                                    new Point(largeurForme * 0.3, largeurForme * 0.65),
                                    new Point(0, largeurForme * 0.4),
                                    new Point(largeurForme * 0.4, largeurForme * 0.35)
                                },
                                Margin = new Thickness(5)
                            };

                            caseGrille.Child = jetonEtoile;
                            break;

                        case "Carre":
                            Rectangle jetonCarre = new Rectangle
                            {
                                Fill = Brushes.White,
                                Width = largeurForme,
                                Height = largeurForme,
                                RadiusX = largeurForme / 4,
                                RadiusY = largeurForme / 4,
                                Margin = new Thickness(5)
                            };

                            caseGrille.Child = jetonCarre;
                            break;

                        case "Losange":
                            Polygon jetonLosange = new Polygon
                            {
                                Fill = Brushes.White,
                                Points = new PointCollection
                                {
                                    new Point(largeurForme / 2, 0),
                                    new Point(largeurForme, largeurForme / 2),
                                    new Point(largeurForme / 2, largeurForme),
                                    new Point(0, largeurForme / 2)
                                },
                                Margin = new Thickness(5)
                            };

                            caseGrille.Child = jetonLosange;
                            break;

                        default:
                            break;
                    }

                    Grid.SetRow(caseGrille, i+1);
                    Grid.SetColumn(caseGrille, j);
                    GridTableJeu.Children.Add(caseGrille);
                }
            }
        }
    }
}