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
        public Challenge ?Challenge { get; set; }

        public Brush ?couleurJ1;
        public Brush ?couleurJ2;

        public string ?premierCoup;
        public string ?coupDecisif;
        public DateTime DebutPartie;
        public double DureePartie;
        public int nbCoups = 0;

        private bool alignement = false;

        public FenetreJeu(Joueur J1, Joueur J2, Configuration config, bool modeChallenge)
        {
            InitializeComponent();
            Joueur1 = J1;
            Joueur2 = J2;
            Config = config;
            Partie = new Partie(Joueur1, Joueur2, Config);
            if (modeChallenge)
            {
                Challenge = new Challenge(0, 0);
            }
            else
            {
                BorderScore.Visibility = Visibility.Hidden;
            }

            Main();
        }

        public FenetreJeu(Joueur J1, Joueur J2, Configuration config, Challenge challenge)
        {
            InitializeComponent();
            Joueur1 = J1;
            Joueur2 = J2;
            Config = config;
            Partie = new Partie(Joueur1, Joueur2, Config);
            Challenge = challenge;

            Main();
        }

        public void Main()
        {
            DessinerGrille();

            couleurJ1 = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur1)!;
            couleurJ2 = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur2)!;

            RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
            RunTxtBlockAuTourDe.Foreground = couleurJ1;

            TxtBlockJoueur1.Text = Partie.Joueur1.Nom;
            TxtBlockJoueur1.Foreground = couleurJ1;
            TxtBlockJoueur2.Text = Partie.Joueur2.Nom;
            TxtBlockJoueur2.Foreground = couleurJ2;

            DebutPartie = DateTime.Now;

            this.KeyDown += Window_KeyDown;

            if (Challenge != null)
            {
                TxtBlockScoreJoueur1.Text = Challenge.ScoreJoueur1.ToString();
                TxtBlockScoreJoueur2.Text = Challenge.ScoreJoueur2.ToString();
            }
        }

        private void BtnQuitter_Click(object sender, RoutedEventArgs e)
        {
            FenetreAccueil fenetreAccueil = new FenetreAccueil();
            fenetreAccueil.Show();
            this.Close();
        }

        private void BtnRelancerPartie_Click(object sender, RoutedEventArgs e)
        {
            FenetreJeu fenetreJeu = new FenetreJeu(Joueur1, Joueur2, Config, Challenge!);
            fenetreJeu.Show();
            this.Close();
        }

        private void BtnFinirChallenge_Click(object sender, RoutedEventArgs e)
        {
            FenetreVictoire fenetreVictoire = new FenetreVictoire(Partie, Challenge!);
            fenetreVictoire.Show();
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            int colonne = -1;

            // Association simple entre la touche et l'index de la colonne
            switch (e.Key)
            {
                case Key.A: colonne = 0; break;
                case Key.Z: colonne = 1; break;
                case Key.E: colonne = 2; break;
                case Key.R: colonne = 3; break;
                case Key.T: colonne = 4; break;
                case Key.Y: colonne = 5; break;
                case Key.U: colonne = 6; break;
                case Key.I: colonne = 7; break;
                case Key.O: colonne = 8; break;
                case Key.P: colonne = 9; break;
                case Key.Q: colonne = 10; break;
                case Key.S: colonne = 11; break;
            }

            // Si la touche pressée fait partie de nos lettres et que la colonne est visible
            if (colonne != -1 && colonne <= Partie.Grille.Lignes && !alignement)
            {
                // On cherche la ligne la plus basse (en partant de la fin)
                for (int ligne = Partie.Grille.Lignes - 1; ligne >= 0; ligne--)
                {
                    // On vérifie s'il y a déjà un visuel à cet emplacement
                    bool caseOccupee = false;
                    foreach (UIElement enfant in GridTableJeu.Children)
                    {
                        // ligne + 1 car dans ton DessinerGrille tu as fait : i + 1 (à cause de l'en-tête)
                        if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                        {
                            // On regarde si la case contient déjà un jeton visible (pas transparent)
                            if (enfant is Border b && b.Child != null && ((Shape)b.Child).Fill != Brushes.Transparent)
                            {
                                caseOccupee = true;
                                break;
                            }
                        }
                    }

                    // Dès qu'on trouve la ligne la plus basse de libre
                    if (!caseOccupee)
                    {
                        // On récupère la Border de cette case pour colorier son jeton en noir
                        foreach (UIElement enfant in GridTableJeu.Children)
                        {
                            if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                            {
                                if (enfant is Border b && b.Child is Shape jeton)
                                {
                                    if (Partie.JoueurCourant == Partie.Joueur1)
                                    {
                                        jeton.Fill = couleurJ1; // Le jeton devient de la couleur du joueur 1

                                        // On change de joueur
                                        Partie.JoueurCourant = Partie.Joueur2; 
                                        RunTxtBlockAuTourDe.Text = Partie.Joueur2.Nom;
                                        RunTxtBlockAuTourDe.Foreground = couleurJ2;

                                        // Dans Grille.cs
                                        Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur1);
                                    }
                                    else
                                    {
                                        jeton.Fill = couleurJ2; // Le jeton devient de la couleur du joueur 2

                                        // On change de joueur
                                        Partie.JoueurCourant = Partie.Joueur1; 
                                        RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                                        RunTxtBlockAuTourDe.Foreground = couleurJ1;

                                        // Dans Grille.cs
                                        Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur2);
                                    }

                                    nbCoups += 1;

                                    if (Partie.Grille.VérifierAlignements(Partie.Configuration.NbJetonAAligner) != EtatCase.Vide)
                                    {
                                        Joueur joueur;
                                        if (Partie.JoueurCourant == Partie.Joueur1)
                                            joueur = Partie.Joueur2;
                                        else
                                            joueur = Partie.Joueur1;

                                        coupDecisif = joueur.Nom +
                                            (colonne == 0 ? "A" :
                                            (colonne == 1 ? "Z" :
                                            (colonne == 2 ? "E" :
                                            (colonne == 3 ? "R" :
                                            (colonne == 4 ? "T" :
                                            (colonne == 5 ? "Y" :
                                            (colonne == 6 ? "U" :
                                            (colonne == 7 ? "I" :
                                            (colonne == 8 ? "O" :
                                            (colonne == 9 ? "P" :
                                            (colonne == 10 ? "Q" :
                                            (colonne == 11 ? "S" :
                                            ""))))))))))));

                                        DateTime Fin = DateTime.Now;
                                        TimeSpan intervalle = Fin - DebutPartie;
                                        DureePartie = intervalle.TotalSeconds;

                                        alignement = true;
                                        PartieFini(joueur);
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                if (premierCoup == null)
                {
                    premierCoup = Partie.Joueur1.Nom +
                        (colonne == 0 ? "A" :
                        (colonne == 1 ? "Z" :
                        (colonne == 2 ? "E" :
                        (colonne == 3 ? "R" :
                        (colonne == 4 ? "T" :
                        (colonne == 5 ? "Y" :
                        (colonne == 6 ? "U" :
                        (colonne == 7 ? "I" :
                        (colonne == 8 ? "O" :
                        (colonne == 9 ? "P" :
                        (colonne == 10 ? "Q" :
                        (colonne == 11 ? "S" :
                        ""))))))))))));
                }
            }
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
                                Fill = Brushes.Transparent,
                                Width = largeurForme,
                                Height = largeurForme,
                                Margin = new Thickness(5)
                            };

                            caseGrille.Child = jetonRond;
                            break;
                        case "Triangle":
                            Polygon jetonTriangle = new Polygon
                            {
                                Fill = Brushes.Transparent,
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
                                Fill = Brushes.Transparent,
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
                                Fill = Brushes.Transparent,
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
                                Fill = Brushes.Transparent,
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
                                Fill = Brushes.Transparent,
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

        private void PartieFini(Joueur Gagnant)
        {
            if (Challenge == null)
            {
                Partie.FinirPartie(premierCoup!, coupDecisif!, DureePartie, nbCoups, Gagnant);
                FenetreVictoire fenetreVictoire = new FenetreVictoire(Partie);
                fenetreVictoire.Show();
                this.Close();
            }
            else
            {
                Partie.FinirPartie(premierCoup!, coupDecisif!, DureePartie, nbCoups, Gagnant);
                if (Gagnant == Partie.Joueur1)
                    Challenge.AjouterPointJoueur(1);
                else
                    Challenge.AjouterPointJoueur(2);

                BtnFinirChallenge.Visibility = Visibility.Visible;
                BtnRelancerPartie.Visibility = Visibility.Visible;
            }
        }
    }
}