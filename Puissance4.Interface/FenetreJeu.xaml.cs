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
using System.Threading.Tasks;

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
        public Challenge? Challenge { get; set; }

        public Brush? couleurJ1;
        public Brush? couleurJ2;

        public string? premierCoup;
        public string? coupDecisif;
        public DateTime DebutPartie;
        public double DureePartie;
        public int nbCoups = 0;

        private bool alignement = false;

        // Minuteur pour le temps de reflexion par coup.
        private System.Windows.Threading.DispatcherTimer? minuteur;
        private int tempsRestant;

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

            // On applique les reglages d'accessibilite.
            AppliquerTailleTexte();
            AppliquerContraste();

            // Si un temps de reflexion a ete choisi (> 0), on lance le minuteur.
            if (Partie.Configuration.TempsReflexion > 0)
            {
                DemarrerMinuteur();
            }
        }

        // On applique la taille de texte choisie aux textes principaux de la fenetre.
        private void AppliquerTailleTexte()
        {
            int taille = Partie.Configuration.TailleTexte;

            RunTxtBlockAuTourDe.FontSize = taille;
            TxtBlockJoueur1.FontSize = taille;
            TxtBlockJoueur2.FontSize = taille;
            TxtBlockScoreJoueur1.FontSize = taille;
            TxtBlockScoreJoueur2.FontSize = taille;
        }

        // Si le contraste marque est active, on met un fond noir et un texte blanc
        // pour que tout soit bien plus lisible.
        private void AppliquerContraste()
        {
            if (Partie.Configuration.ContrasteMarque)
            {
                // Fond noir pur (on utilise le Grid racine nomme dans le XAML).
                GridPrincipal.Background = Brushes.Black;

                // Texte en blanc pur pour le maximum de contraste.
                TxtBlockJoueur1.Foreground = Brushes.White;
                TxtBlockJoueur2.Foreground = Brushes.White;
                TxtBlockScoreJoueur1.Foreground = Brushes.White;
                TxtBlockScoreJoueur2.Foreground = Brushes.White;
            }
        }

        // On prepare et on lance le compte a rebours.
        // Un DispatcherTimer est un objet qui "sonne" tout seul a intervalle regulier.
        // Ici : il sonne toutes les secondes, et a chaque fois il appelle Minuteur_Tick.
        // C'est le meme principe qu'un bouton : un evenement declenche une methode.
        // La difference, c'est que l'evenement n'est pas un clic, c'est le temps qui passe.
        private void DemarrerMinuteur()
        {
            TxtBlockMinuteur.Visibility = Visibility.Visible;

            tempsRestant = Partie.Configuration.TempsReflexion;
            TxtBlockMinuteur.Text = "Temps : " + tempsRestant + "s";

            minuteur = new System.Windows.Threading.DispatcherTimer();
            minuteur.Interval = TimeSpan.FromSeconds(1); // sonne toutes les secondes
            minuteur.Tick += Minuteur_Tick;              // a chaque sonnerie, on appelle Minuteur_Tick
            minuteur.Start();
        }

        // Methode appelee automatiquement par le minuteur, chaque seconde.
        private void Minuteur_Tick(object? sender, EventArgs e)
        {
            tempsRestant = tempsRestant - 1;
            TxtBlockMinuteur.Text = "Temps : " + tempsRestant + "s";

            if (tempsRestant <= 0)
            {
                PasserLeTour(); // le temps est ecoule : le joueur perd son tour
            }
        }

        // On remet le compte a rebours au maximum (apres chaque coup joue).
        private void ReinitialiserMinuteur()
        {
            if (minuteur != null)
            {
                tempsRestant = Partie.Configuration.TempsReflexion;
                TxtBlockMinuteur.Text = "Temps : " + tempsRestant + "s";
            }
        }

        // Le temps est ecoule : on change de joueur sans poser de jeton.
        private void PasserLeTour()
        {
            if (Partie.JoueurCourant == Partie.Joueur1)
            {
                Partie.JoueurCourant = Partie.Joueur2;
                RunTxtBlockAuTourDe.Text = Partie.Joueur2.Nom;
                RunTxtBlockAuTourDe.Foreground = couleurJ2;
            }
            else
            {
                Partie.JoueurCourant = Partie.Joueur1;
                RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                RunTxtBlockAuTourDe.Foreground = couleurJ1;
            }

            ReinitialiserMinuteur();
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

        // =========================================================
        // GESTION DU CLAVIER
        // On traduit la touche en numero de colonne, puis on appelle JouerCoup.
        // =========================================================
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            int colonne = -1;

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

            if (colonne != -1)
            {
                JouerCoup(colonne);
            }
        }

        // =========================================================
        // GESTION DE LA SOURIS
        // Au clic sur une case, on recupere la colonne stockee dans son Tag
        // (voir DessinerGrille) et on joue le coup.
        // =========================================================
        private void Case_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border b && b.Tag is int colonne)
            {
                JouerCoup(colonne);
            }
        }

        // =========================================================
        // METHODE COMMUNE : pose un jeton dans la colonne demandee.
        // Le clavier ET la souris l'appellent.
        // =========================================================
        private async void JouerCoup(int colonne)
        {
            // On verifie que la colonne existe et que le jeu n'est pas bloque.
            if (colonne < 0 || colonne >= Partie.Grille.Colonnes || alignement)
            {
                return;
            }

            // On cherche la ligne la plus basse (en partant de la fin).
            for (int ligne = Partie.Grille.Lignes - 1; ligne >= 0; ligne--)
            {
                bool caseOccupee = false;
                foreach (UIElement enfant in GridTableJeu.Children)
                {
                    // ligne + 1 a cause de l'en-tete dans DessinerGrille.
                    if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                    {
                        if (enfant is Border b && b.Child != null && ((Shape)b.Child).Fill != Brushes.Transparent)
                        {
                            caseOccupee = true;
                            break;
                        }
                    }
                }

                if (!caseOccupee)
                {
                    foreach (UIElement enfant in GridTableJeu.Children)
                    {
                        if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                        {
                            if (enfant is Border b && b.Child is Shape jeton)
                            {
                                if (Partie.JoueurCourant == Partie.Joueur1)
                                {
                                    jeton.Fill = couleurJ1;
                                    RunTxtBlockAuTourDe.Text = Partie.Joueur2.Nom;
                                    RunTxtBlockAuTourDe.Foreground = couleurJ2;

                                    Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur1);

                                    if (Partie.Grille.VérifierAlignements(Partie.Configuration.NbJetonAAligner) == EtatCase.Vide)
                                    {
                                        if (Partie.Joueur2.NiveauVirtuel == NiveauVirtuel.Humain)
                                        {
                                            Partie.JoueurCourant = Partie.Joueur2;
                                        }
                                        else if (Partie.Joueur2.NiveauVirtuel == NiveauVirtuel.Intelligent)
                                        {
                                            alignement = true;
                                            await Task.Delay(2000);
                                            alignement = false;

                                            JouerIntelligent();
                                        }
                                        else
                                        {
                                            alignement = true;
                                            await Task.Delay(2000);
                                            alignement = false;

                                            JouerIdiot();
                                        }
                                    }
                                    else
                                        Partie.JoueurCourant = Partie.Joueur2;
                                }
                                else
                                {
                                    jeton.Fill = couleurJ2;

                                    Partie.JoueurCourant = Partie.Joueur1;
                                    RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                                    RunTxtBlockAuTourDe.Foreground = couleurJ1;

                                    Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur2);
                                }

                                nbCoups += 1;

                                // On remet le compte a rebours a zero apres chaque coup joue.
                                ReinitialiserMinuteur();

                                if (Partie.Grille.VérifierAlignements(Partie.Configuration.NbJetonAAligner) != EtatCase.Vide)
                                {
                                    Joueur joueur;
                                    if (Partie.JoueurCourant == Partie.Joueur1)
                                        joueur = Partie.Joueur2;
                                    else
                                        joueur = Partie.Joueur1;

                                    coupDecisif = joueur.Nom + LettreColonne(colonne);

                                    DateTime Fin = DateTime.Now;
                                    TimeSpan intervalle = Fin - DebutPartie;
                                    DureePartie = intervalle.TotalSeconds;

                                    alignement = true;

                                    // La partie est finie : on arrete le minuteur.
                                    if (minuteur != null)
                                        minuteur.Stop();

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
                premierCoup = Partie.Joueur1.Nom + LettreColonne(colonne);
            }
        }

        // Transforme un numero de colonne en lettre du clavier (pour l'affichage des coups).
        // Avant, ce gros bloc de "? :" etait recopie deux fois. La on l'ecrit une seule fois.
        private string LettreColonne(int colonne)
        {
            switch (colonne)
            {
                case 0: return "A";
                case 1: return "Z";
                case 2: return "E";
                case 3: return "R";
                case 4: return "T";
                case 5: return "Y";
                case 6: return "U";
                case 7: return "I";
                case 8: return "O";
                case 9: return "P";
                case 10: return "Q";
                case 11: return "S";
                default: return "";
            }
        }

        private void JouerIdiot()
        {
            bool tourIA = true;
            while (tourIA)
            {
                Random rand = new Random();
                int colonne = rand.Next(Partie.Grille.Colonnes);
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
                        // On récupère la Border de cette case pour colorier son jeton
                        foreach (UIElement enfant in GridTableJeu.Children)
                        {
                            if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                            {
                                if (enfant is Border b && b.Child is Shape jeton)
                                {
                                    jeton.Fill = couleurJ2; // Le jeton devient de la couleur du joueur 2
                                    tourIA = false;

                                    // On change de joueur
                                    RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                                    RunTxtBlockAuTourDe.Foreground = couleurJ1;

                                    // Dans Grille.cs
                                    Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur2);

                                    nbCoups += 1;

                                    // On remet le compte a rebours a zero apres le coup de l'IA.
                                    ReinitialiserMinuteur();
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        // L'IA intelligente joue son coup.
        // C'est presque la meme chose que JouerIdiot, sauf que la colonne n'est
        // pas tiree au hasard : on demande a la classe IA de choisir le meilleur
        // coup grace a l'algorithme Minimax (alpha-beta) de la SAE 2.2.
        private void JouerIntelligent()
        {
            // On cree une IA de niveau Intelligent et on lui donne le nombre de
            // jetons a aligner (ca vient de la configuration de la partie).
            IA ia = new IA(NiveauVirtuel.Intelligent, Partie.Configuration.NbJetonAAligner);

            // L'IA nous renvoie le numero de la colonne ou elle veut jouer.
            int colonne = ia.ChoisirColonne(Partie.Grille);

            // Si l'IA ne peut pas jouer (grille pleine), on arrete.
            if (colonne == -1)
                return;

            // On cherche la ligne la plus basse de libre dans cette colonne.
            for (int ligne = Partie.Grille.Lignes - 1; ligne >= 0; ligne--)
            {
                // On verifie si la case est deja occupee par un jeton visible.
                bool caseOccupee = false;
                foreach (UIElement enfant in GridTableJeu.Children)
                {
                    // ligne + 1 a cause de l'en-tete dans DessinerGrille.
                    if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                    {
                        if (enfant is Border b && b.Child != null && ((Shape)b.Child).Fill != Brushes.Transparent)
                        {
                            caseOccupee = true;
                            break;
                        }
                    }
                }

                // Des qu'on trouve la ligne la plus basse de libre.
                if (!caseOccupee)
                {
                    // On recupere la Border de cette case pour colorier son jeton.
                    foreach (UIElement enfant in GridTableJeu.Children)
                    {
                        if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                        {
                            if (enfant is Border b && b.Child is Shape jeton)
                            {
                                jeton.Fill = couleurJ2; // Le jeton devient de la couleur du joueur 2 (l'IA)

                                // On redonne la main au joueur 1.
                                RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                                RunTxtBlockAuTourDe.Foreground = couleurJ1;

                                // Dans Grille.cs
                                Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur2);

                                nbCoups += 1;

                                // On remet le compte a rebours a zero apres le coup de l'IA.
                                ReinitialiserMinuteur();
                            }
                            break;
                        }
                    }
                    break;
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

                    // On retient le numero de colonne dans la Border pour le
                    // retrouver au clic, et on abonne la case au clic de la souris.
                    caseGrille.Tag = j;
                    caseGrille.MouseLeftButtonDown += Case_MouseLeftButtonDown;

                    Grid.SetRow(caseGrille, i + 1);
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
                {
                    Challenge.AjouterPointJoueur(1);
                    TxtBlockScoreJoueur1.Text = Challenge.ScoreJoueur1.ToString();
                }
                else
                {
                    Challenge.AjouterPointJoueur(2);
                    TxtBlockScoreJoueur2.Text = Challenge.ScoreJoueur2.ToString();
                }


                BtnFinirChallenge.Visibility = Visibility.Visible;
                BtnRelancerPartie.Visibility = Visibility.Visible;
            }
        }
    }
}