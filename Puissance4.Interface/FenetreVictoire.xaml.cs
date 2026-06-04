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
    /// Logique d'interaction pour FenetreVictoire.xaml
    /// </summary>
    public partial class FenetreVictoire : Window
    {
        public Partie Partie { get; set; }
        public Challenge ?Challenge { get; set; }

        public FenetreVictoire(Partie partie, Challenge challenge)
        {
            InitializeComponent();
            Partie = partie;
            Challenge = challenge;

            Main();
            EcrireTableauJeu();
        }

        public FenetreVictoire(Partie partie)
        {
            InitializeComponent();
            Partie = partie;

            Main();
            EcrireTableauJeu();
        }

        public void Main()
        {
            TxtBlockNomJoueur.Text = Partie.Gagnant!.Nom;
            if (Partie.Gagnant == Partie.Joueur1)
                TxtBlockNomJoueur.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur1)!;
            else
                TxtBlockNomJoueur.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur2)!;

            TxtBlockJoueur1.Text = Partie.Joueur1.Nom;
            TxtBlockJoueur1.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur1)!;
            TxtBlockJoueur2.Text = Partie.Joueur2.Nom;
            TxtBlockJoueur2.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur2)!;

            if (Challenge == null)
            {
                BorderChallenge.Visibility = Visibility.Hidden;
            }
        }

        public void EcrireTableauJeu()
        {
            GridTableauJeu.Children.Clear();
            GridTableauJeu.RowDefinitions.Clear();
            GridTableauJeu.ColumnDefinitions.Clear();

            // 2. On crée dynamiquement les définitions de lignes
            for (int i = 0; i < Partie.Grille.Lignes; i++)
            {
                GridTableauJeu.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // 3. On crée dynamiquement les définitions de colonnes
            for (int i = 0; i < Partie.Grille.Colonnes; i++)
            {
                GridTableauJeu.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            double taille = (Partie.Grille.Lignes == 6 ? 23 : (Partie.Grille.Lignes == 8 ? 17 : 12)); 
            for (int ligne = 0; ligne < Partie.Grille.Lignes; ligne++) 
            { 
                for (int colonne = 0; colonne < Partie.Grille.Colonnes; colonne++) 
                {
                    EtatCase caseActuel = Partie.Grille.Tableau[ligne][colonne];
                    Brush couleur = (Brush)new BrushConverter().ConvertFromString("#cbc7b7")!;
                    if (caseActuel == EtatCase.Joueur1)
                    {
                        couleur = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur1.ToString())!;
                    }
                    else if (caseActuel == EtatCase.Joueur2)
                    {
                        couleur = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur2.ToString())!;
                    }

                    Border caseGrille = new Border
                    {
                        Background = (Brush)new BrushConverter().ConvertFromString("#cbc7b7")!,
                        Width = taille,
                        Height = taille,
                        Margin = new Thickness(2),
                        CornerRadius = new CornerRadius(5)
                    };

                    double largeurForme = taille - 4;
                    switch (Partie.Configuration.FormeJoueur)
                    {
                        case "Rond":
                            Ellipse jetonRond = new Ellipse
                            {
                                Fill = couleur,
                                Width = largeurForme,
                                Height = largeurForme,
                                Margin = new Thickness(2)
                            };

                            caseGrille.Child = jetonRond;
                            break;
                        case "Triangle":
                            Polygon jetonTriangle = new Polygon
                            {
                                Fill = couleur,
                                Points = new PointCollection
                                {
                                    new Point(largeurForme / 2, 0),
                                    new Point(0, largeurForme),
                                    new Point(largeurForme, largeurForme)
                                },
                                Margin = new Thickness(2)
                            };

                            caseGrille.Child = jetonTriangle;
                            break;

                        case "Croix":
                            double tiers = largeurForme / 3;
                            double deuxTiers = 2 * tiers;

                            Polygon jetonCroix = new Polygon
                            {
                                Fill = couleur,
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
                                Margin = new Thickness(2)
                            };

                            caseGrille.Child = jetonCroix;
                            break;

                        case "Etoile":
                            Polygon jetonEtoile = new Polygon
                            {
                                Fill = couleur,
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
                                Margin = new Thickness(2)
                            };

                            caseGrille.Child = jetonEtoile;
                            break;

                        case "Carre":
                            Rectangle jetonCarre = new Rectangle
                            {
                                Fill = couleur,
                                Width = largeurForme,
                                Height = largeurForme,
                                RadiusX = largeurForme / 4,
                                RadiusY = largeurForme / 4,
                                Margin = new Thickness(2)
                            };

                            caseGrille.Child = jetonCarre;
                            break;

                        case "Losange":
                            Polygon jetonLosange = new Polygon
                            {
                                Fill = couleur,
                                Points = new PointCollection
                                {
                                    new Point(largeurForme / 2, 0),
                                    new Point(largeurForme, largeurForme / 2),
                                    new Point(largeurForme / 2, largeurForme),
                                    new Point(0, largeurForme / 2)
                                },
                                Margin = new Thickness(2)
                            };

                            caseGrille.Child = jetonLosange;
                            break;

                        default:
                            break;
                    }

                    Grid.SetRow(caseGrille, ligne);
                    Grid.SetColumn(caseGrille, colonne);
                    GridTableauJeu.Children.Add(caseGrille);
                }
            }
        }
    }
}
