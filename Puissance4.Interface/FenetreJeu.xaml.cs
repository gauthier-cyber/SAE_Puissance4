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
using System.Windows.Threading;
using System.ComponentModel;

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

        private DispatcherTimer? timerTempsReflexion;
        private int tempsRestant = 0;

        private bool ContrasteMarque;
        private int TailleTexte;

        public FenetreJeu(Joueur J1, Joueur J2, Configuration config, bool modeChallenge, bool contrasteMarque, int tailleTexte)
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
            ContrasteMarque = contrasteMarque;
            TailleTexte = tailleTexte;
            Main(ContrasteMarque, TailleTexte);
        }

        // on essaie de tout nettoyer quand l'objet est supprimé
        ~FenetreJeu()
        {
            try
            {
                ArreterTimer();
                if (timerTempsReflexion != null)
                    timerTempsReflexion = null;
            }
            catch { }
        }

        // on nettoie tout de suite quand on ferme la fenêtre
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                ArreterTimer();
                this.KeyDown -= Window_KeyDown;
                // on vide les éléments visuels
                if (GridTableJeu != null)
                    GridTableJeu.Children.Clear();
                if (ListBoxHistorique != null)
                    ListBoxHistorique.Items.Clear();
            }
            catch { }

            base.OnClosing(e);
        }

        public FenetreJeu(Joueur J1, Joueur J2, Configuration config, Challenge challenge, bool contrasteMarque, int tailleTexte)
        {
            InitializeComponent();
            Joueur1 = J1;
            Joueur2 = J2;
            Config = config;
            Partie = new Partie(Joueur1, Joueur2, Config);
            Challenge = challenge;

            Main(contrasteMarque, tailleTexte);
        }

        public void Main(bool contrasteMarque, int tailleTexte)
        {
            ContrasteMarque = contrasteMarque;
            TailleTexte = tailleTexte;

            // on change la taille du texte selon le réglage choisi
            double baseFontSize = SystemFonts.MessageFontSize;
            double multiplier = (TailleTexte >= 6) ? 1.6 : (TailleTexte <= -6) ? 0.8 : 1.0;
            double newFontSize = Math.Max(8, Math.Round(baseFontSize * multiplier));
            this.FontSize = newFontSize;

            // on agrandit la fenêtre pour que le texte ne dépasse pas des cadres
            if (TailleTexte >= 6)
            {
                this.Width = 1000;
                this.Height = 720;
            }
            else if (TailleTexte <= -6)
            {
                this.Width = 760;
                this.Height = 520;
            }
            else
            {
                this.Width = 800;
                this.Height = 550;
            }

            // on met à jour la taille des textes déjà placés dans le XAML
            try
            {
                // le texte du joueur au tour est dans un TextBlock parent
                if (RunTxtBlockAuTourDe != null)
                {
                    RunTxtBlockAuTourDe.FontSize = this.FontSize;
                    var parentTb = RunTxtBlockAuTourDe.Parent as TextBlock;
                    if (parentTb != null) parentTb.FontSize = this.FontSize;
                }

                // les noms des joueurs et leurs scores
                if (TxtBlockJoueur1 != null) TxtBlockJoueur1.FontSize = this.FontSize;
                if (TxtBlockJoueur2 != null) TxtBlockJoueur2.FontSize = this.FontSize;
                if (TxtBlockScoreJoueur1 != null) TxtBlockScoreJoueur1.FontSize = this.FontSize;
                if (TxtBlockScoreJoueur2 != null) TxtBlockScoreJoueur2.FontSize = this.FontSize;

                // les boutons
                if (BtnFinirChallenge != null) BtnFinirChallenge.FontSize = Math.Max(10, this.FontSize * 0.9);
                if (BtnRelancerPartie != null) BtnRelancerPartie.FontSize = Math.Max(10, this.FontSize * 0.9);
                if (BtnQuitter != null) BtnQuitter.FontSize = Math.Max(10, this.FontSize * 0.9);

                // le temps restant et l'historique
                if (TxtBlockTempsRestant != null) TxtBlockTempsRestant.FontSize = Math.Max(10, this.FontSize);
                if (ListBoxHistorique != null) ListBoxHistorique.FontSize = Math.Max(10, this.FontSize);

                // on agrandit le cadre du score et l'historique pour avoir de la place
                if (BorderScore != null) BorderScore.Height = Math.Max(80, this.FontSize * 6);
                if (ListBoxHistorique != null) ListBoxHistorique.Height = Math.Max(120, this.FontSize * 12);

                // on ajuste aussi les lettres au dessus des colonnes (la première ligne)
                foreach (UIElement enfant in GridTableJeu.Children)
                {
                    if (enfant is TextBlock tb)
                    {
                        int row = Grid.GetRow(tb);
                        if (row == 0)
                        {
                            tb.FontSize = Math.Max(10, this.FontSize * 0.8);
                        }
                    }
                }
            }
            catch { }

            DessinerGrille();

            couleurJ1 = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur1)!;
            couleurJ2 = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur2)!;

            RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
            RunTxtBlockAuTourDe.Foreground = couleurJ1;

            TxtBlockJoueur1.Text = Partie.Joueur1.Nom;
            TxtBlockJoueur1.Foreground = couleurJ1;
            TxtBlockJoueur2.Text = Partie.Joueur2.Nom;
            TxtBlockJoueur2.Foreground = couleurJ2;

            // on écrit dans l'historique que les deux joueurs ont rejoint
            AjouterHistoriqueRejoindre(Partie.Joueur1.Nom);
            AjouterHistoriqueRejoindre(Partie.Joueur2.Nom);

            DebutPartie = DateTime.Now;

            this.KeyDown += Window_KeyDown;
            DemarrerTimerSiBesoin();

            if (Challenge != null)
            {
                TxtBlockScoreJoueur1.Text = Challenge.ScoreJoueur1.ToString();
                TxtBlockScoreJoueur2.Text = Challenge.ScoreJoueur2.ToString();
            }

            // on applique les réglages de contraste et de taille de texte
            if (contrasteMarque)
            {
                // fond blanc
                this.Background = Brushes.White;
                // police Verdana partout
                var verdana = new FontFamily("Verdana");
                this.FontFamily = verdana;
                // texte en noir partout
                this.Foreground = Brushes.Black;
                BtnFinirChallenge.Foreground = Brushes.Black;
                BtnRelancerPartie.Foreground = Brushes.Black;
                BtnQuitter.Foreground = Brushes.Black;
            }
        }

        private void DemarrerTimerSiBesoin()
        {
            if (timerTempsReflexion != null)
            {
                timerTempsReflexion.Stop();
            }

            if (Partie.Configuration.TempsReflexion > 0)
            {
                tempsRestant = Partie.Configuration.TempsReflexion;
                TxtBlockTempsRestant.Visibility = Visibility.Visible;
                TxtBlockTempsRestant.Text = $"Temps restant : {tempsRestant}s";

                if (timerTempsReflexion == null)
                {
                    timerTempsReflexion = new DispatcherTimer();
                    timerTempsReflexion.Interval = TimeSpan.FromSeconds(1);
                    timerTempsReflexion.Tick += TimerTempsReflexion_Tick;
                }

                timerTempsReflexion.Start();
            }
            else
            {
                TxtBlockTempsRestant.Visibility = Visibility.Collapsed;
            }
        }

        private void ArreterTimer()
        {
            if (timerTempsReflexion != null)
                timerTempsReflexion.Stop();
            TxtBlockTempsRestant.Visibility = Visibility.Collapsed;
        }

        private void TimerTempsReflexion_Tick(object? sender, EventArgs e)
        {
            // si la partie est déjà finie on arrête le timer et on ne fait rien
            if (Partie != null && Partie.Gagnant != null)
            {
                ArreterTimer();
                return;
            }

            tempsRestant -= 1;
            if (tempsRestant < 0) tempsRestant = 0;
            TxtBlockTempsRestant.Text = $"Temps restant : {tempsRestant}s";

            if (tempsRestant == 0)
            {
                ArreterTimer();
                if (Partie == null || Partie.Gagnant != null) return;
                JouerAutoSiTempsEcoule();
            }
        }

        private void JouerAutoSiTempsEcoule()
        {
            if (Partie.JoueurCourant.NiveauVirtuel == NiveauVirtuel.Intelligent)
            {
                JouerIntelligent();
                return;
            }

            Random rand = new Random();
            for (int attempt = 0; attempt < Partie.Grille.Colonnes; attempt++)
            {
                int colonne = rand.Next(Partie.Grille.Colonnes);
                for (int ligne = Partie.Grille.Lignes - 1; ligne >= 0; ligne--)
                {
                    bool caseOccupee = false;
                    foreach (UIElement enfant in GridTableJeu.Children)
                    {
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
                                        Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur1);
                                        Partie.JoueurCourant = Partie.Joueur2;
                                        RunTxtBlockAuTourDe.Text = Partie.Joueur2.Nom;
                                        RunTxtBlockAuTourDe.Foreground = couleurJ2;
                                    }
                                    else
                                    {
                                        jeton.Fill = couleurJ2;
                                        Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur2);
                                        Partie.JoueurCourant = Partie.Joueur1;
                                        RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                                        RunTxtBlockAuTourDe.Foreground = couleurJ1;
                                    }

                                    nbCoups += 1;

                                    if (Partie.Grille.VérifierAlignements(Partie.Configuration.NbJetonAAligner) != EtatCase.Vide)
                                    {
                                        Joueur joueur;
                                        if (Partie.JoueurCourant == Partie.Joueur1)
                                            joueur = Partie.Joueur2;
                                        else
                                            joueur = Partie.Joueur1;

                                        DateTime Fin = DateTime.Now;
                                        TimeSpan intervalle = Fin - DebutPartie;
                                        DureePartie = intervalle.TotalSeconds;

                                        alignement = true;
                                        // on arrête le timer juste avant de finir la partie
                                        ArreterTimer();
                                        PartieFini(joueur);
                                    }

                                    DemarrerTimerSiBesoin();
                                }
                                break;
                            }
                        }
                        return;
                    }
                }
            }
        }

        private void BtnQuitter_Click(object sender, RoutedEventArgs e)
        {
            FenetreAccueil fenetreAccueil = new FenetreAccueil(ContrasteMarque, TailleTexte);
            fenetreAccueil.Show();
            this.Close();
        }

        private void BtnRelancerPartie_Click(object sender, RoutedEventArgs e)
        {
            FenetreJeu fenetreJeu = new FenetreJeu(Joueur1, Joueur2, Config, Challenge!, ContrasteMarque, TailleTexte);
            fenetreJeu.Show();
            this.Close();
        }

        private void BtnFinirChallenge_Click(object sender, RoutedEventArgs e)
        {
            FenetreVictoire fenetreVictoire = new FenetreVictoire(Partie, Challenge!, ContrasteMarque, TailleTexte);
            fenetreVictoire.Show();
            this.Close();
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            int colonne = -1;

            // on relie chaque touche à un numéro de colonne
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

            // on joue seulement si la touche est l'une des nôtres et que la colonne existe
            if (colonne != -1 && colonne <= Partie.Grille.Colonnes && !alignement)
            {
                // on cherche la ligne libre la plus basse en partant du bas
                for (int ligne = Partie.Grille.Lignes - 1; ligne >= 0; ligne--)
                {
                    // on regarde si la case a déjà quelque chose dessus
                    bool caseOccupee = false;
                    foreach (UIElement enfant in GridTableJeu.Children)
                    {
                        // ligne + 1 car la première ligne sert aux lettres des colonnes
                        if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                        {
                            // on vérifie si la case a déjà un jeton visible
                            if (enfant is Border b && b.Child != null && ((Shape)b.Child).Fill != Brushes.Transparent)
                            {
                                caseOccupee = true;
                                break;
                            }
                        }
                    }

                    // dès qu'on trouve la première case libre en bas
                    if (!caseOccupee)
                    {
                        // on récupère la case pour colorier son jeton
                        foreach (UIElement enfant in GridTableJeu.Children)
                        {
                            if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                            {
                                if (enfant is Border b && b.Child is Shape jeton)
                                {
                                    if (Partie.JoueurCourant == Partie.Joueur1)
                                    {
                                        jeton.Fill = couleurJ1; // le jeton prend la couleur du joueur 1
                                        RunTxtBlockAuTourDe.Text = Partie.Joueur2.Nom;
                                        RunTxtBlockAuTourDe.Foreground = couleurJ2;

                                        // on prévient la grille du nouveau jeton
                                        Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur1);
                                        AjouterHistorique(Partie.Joueur1.Nom, colonne);

                                        if (Partie.Grille.VérifierAlignements(Partie.Configuration.NbJetonAAligner) == EtatCase.Vide)
                                        {
                                            if (Partie.Joueur2.NiveauVirtuel == NiveauVirtuel.Humain)
                                            {
                                                // on passe au joueur 2
                                                Partie.JoueurCourant = Partie.Joueur2;
                                                DemarrerTimerSiBesoin();
                                            }
                                            else if (Partie.Joueur2.NiveauVirtuel == NiveauVirtuel.Intelligent)
                                            {
                                                // on bloque les touches pendant que l'IA "réfléchit"
                                                alignement = true;
                                                // on stoppe le timer pour que l'IA ne récupère pas le temps restant
                                                ArreterTimer();
                                                await Task.Delay(2000);
                                                // on relance un timer tout neuf pour l'IA si besoin
                                                DemarrerTimerSiBesoin();
                                                alignement = false;

                                                JouerIntelligent();
                                            }
                                            else
                                            {
                                                alignement = true;
                                                // on stoppe le timer pour que l'IA ne récupère pas le temps restant
                                                ArreterTimer();
                                                await Task.Delay(2000);
                                                // on relance un timer tout neuf pour l'IA si besoin
                                                DemarrerTimerSiBesoin();
                                                alignement = false;

                                                JouerIdiot();
                                            }
                                        }
                                        else
                                            Partie.JoueurCourant = Partie.Joueur2;
                                    }
                                    else
                                    {
                                        jeton.Fill = couleurJ2; // le jeton prend la couleur du joueur 2

                                        // on repasse au joueur 1
                                        Partie.JoueurCourant = Partie.Joueur1;
                                        RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                                        RunTxtBlockAuTourDe.Foreground = couleurJ1;

                                        DemarrerTimerSiBesoin();

                                        // on prévient la grille du nouveau jeton
                                        Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur2);
                                        AjouterHistorique(Partie.Joueur2.Nom, colonne);
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
                                    DemarrerTimerSiBesoin();
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
                DemarrerTimerSiBesoin();
            }
        }

        private void JouerIdiot()
        {
            bool tourIA = true;
            while (tourIA)
            {
                Random rand = new Random();
                int colonne = rand.Next(Partie.Grille.Colonnes);
                // on cherche la ligne libre la plus basse en partant du bas
                for (int ligne = Partie.Grille.Lignes - 1; ligne >= 0; ligne--)
                {
                    // on regarde si la case a déjà quelque chose dessus
                    bool caseOccupee = false;
                    foreach (UIElement enfant in GridTableJeu.Children)
                    {
                        // ligne + 1 car la première ligne sert aux lettres des colonnes
                        if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                        {
                            // on vérifie si la case a déjà un jeton visible
                            if (enfant is Border b && b.Child != null && ((Shape)b.Child).Fill != Brushes.Transparent)
                            {
                                caseOccupee = true;
                                break;
                            }
                        }
                    }

                    // dès qu'on trouve la première case libre en bas
                    if (!caseOccupee)
                    {
                        // on récupère la case pour colorier son jeton
                        foreach (UIElement enfant in GridTableJeu.Children)
                        {
                            if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                            {
                                if (enfant is Border b && b.Child is Shape jeton)
                                {
                                    jeton.Fill = couleurJ2; // le jeton prend la couleur du joueur 2
                                    tourIA = false;

                                    // on repasse au joueur 1
                                    RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                                    RunTxtBlockAuTourDe.Foreground = couleurJ1;

                                    // on prévient la grille du nouveau jeton
                                    Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur2);

                                    nbCoups += 1;
                                    AjouterHistorique(Partie.Joueur2.Nom, colonne);
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        // l'IA intelligente joue son coup.
        // c'est presque pareil que JouerIdiot, sauf que la colonne n'est pas
        // tirée au hasard : on demande à la classe IA de choisir le meilleur
        // coup avec l'algorithme Minimax (alpha-beta) de la SAE 2.2.
        private void JouerIntelligent()
        {
            IA ia = new IA(NiveauVirtuel.Intelligent, Partie.Configuration.NbJetonAAligner);
            int colonne = ia.ChoisirColonne(Partie.Grille);
            if (colonne == -1) return;

            for (int ligne = Partie.Grille.Lignes - 1; ligne >= 0; ligne--)
            {
                bool caseOccupee = false;
                foreach (UIElement enfant in GridTableJeu.Children)
                {
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
                                jeton.Fill = couleurJ2; // le jeton prend la couleur du joueur 2 (l'IA)
                                RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                                RunTxtBlockAuTourDe.Foreground = couleurJ1;
                                Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur2);
                                nbCoups += 1;
                                AjouterHistorique(Partie.Joueur2.Nom, colonne);
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void AjouterHistorique(string nomJoueur, int colonne)
        {
            try
            {
                string lettre = ColonneToLettre(colonne);
                // on crée la ligne d'historique : [Nom] en [Lettre], avec le nom coloré
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                TextBlock txtNom = new TextBlock { Text = nomJoueur + " ", FontSize = this.FontSize };
                // on choisit la couleur du joueur
                if (nomJoueur == Partie.Joueur1.Nom)
                    txtNom.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur1)!;
                else
                    txtNom.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur2)!;

                TextBlock txtEn = new TextBlock { Text = "en ", FontSize = this.FontSize };
                TextBlock txtCol = new TextBlock { Text = lettre, FontSize = this.FontSize };

                if (ContrasteMarque)
                {
                    txtEn.Foreground = Brushes.Black;
                    txtCol.Foreground = Brushes.Black;
                }
                else
                {
                    txtEn.Foreground = Brushes.White;
                    txtCol.Foreground = Brushes.White;
                }

                panel.Children.Add(txtNom);
                panel.Children.Add(txtEn);
                panel.Children.Add(txtCol);

                // on met le tout dans un ListBoxItem pour bien l'afficher dans la liste
                ListBoxItem item = new ListBoxItem { Content = panel, Padding = new Thickness(4), Background = Brushes.Transparent, BorderThickness = new Thickness(0) };
                ListBoxHistorique.Items.Insert(0, item);
            }
            catch { }
        }

        private string ColonneToLettre(int colonne)
        {
            string[] mapping = new string[] { "A", "Z", "E", "R", "T", "Y", "U", "I", "O", "P", "Q", "S" };
            if (colonne >= 0 && colonne < mapping.Length) return mapping[colonne];
            return colonne.ToString();
        }

        private void AjouterHistoriqueGagne(Joueur gagnant)
        {
            try
            {
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                TextBlock txtNom = new TextBlock { Text = gagnant.Nom + " ", FontSize = this.FontSize };
                if (gagnant == Partie.Joueur1)
                    txtNom.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur1)!;
                else
                    txtNom.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur2)!;

                TextBlock txtMsg = new TextBlock { Text = "a gagné", FontSize = this.FontSize };
                if (ContrasteMarque)
                    txtMsg.Foreground = Brushes.Black;
                else
                    txtMsg.Foreground = Brushes.White;
                panel.Children.Add(txtNom);
                panel.Children.Add(txtMsg);
                ListBoxItem item = new ListBoxItem { Content = panel, Padding = new Thickness(4), Background = Brushes.Transparent, BorderThickness = new Thickness(0) };
                ListBoxHistorique.Items.Insert(0, item);
            }
            catch { }
        }

        private void AjouterHistoriqueRejoindre(string nomJoueur)
        {
            try
            {
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                TextBlock txtNom = new TextBlock { Text = nomJoueur + " ", FontSize = this.FontSize };
                if (nomJoueur == Partie.Joueur1.Nom)
                    txtNom.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur1)!;
                else
                    txtNom.Foreground = (Brush)new BrushConverter().ConvertFromString(Partie.Configuration.CouleurJoueur2)!;

                TextBlock txtMsg = new TextBlock { Text = "a rejoint", FontSize = this.FontSize };
                if (ContrasteMarque)
                    txtMsg.Foreground = Brushes.Black;
                else
                    txtMsg.Foreground = Brushes.White;
                panel.Children.Add(txtNom);
                panel.Children.Add(txtMsg);
                ListBoxItem item = new ListBoxItem { Content = panel, Padding = new Thickness(4), Background = Brushes.Transparent, BorderThickness = new Thickness(0) };
                ListBoxHistorique.Items.Insert(0, item);
            }
            catch { }
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

                    Grid.SetRow(caseGrille, i + 1);
                    Grid.SetColumn(caseGrille, j);
                    // on permet aussi de jouer en cliquant sur la case (on retrouve la colonne avec Grid.GetColumn)
                    caseGrille.MouseLeftButtonDown += CaseGrille_MouseLeftButtonDown;
                    GridTableJeu.Children.Add(caseGrille);
                }
            }
        }

        // ce qui se passe quand on clique sur une case (on trouve la colonne et on joue comme avec les touches)
        private async void CaseGrille_MouseLeftButtonDown(object? sender, MouseButtonEventArgs e)
        {
            if (alignement) return;
            if (sender is not UIElement element) return;

            int colonne = Grid.GetColumn(element);
            if (colonne < 0 || colonne >= Partie.Grille.Colonnes) return;

            // on cherche la ligne libre la plus basse en partant du bas
            for (int ligne = Partie.Grille.Lignes - 1; ligne >= 0; ligne--)
            {
                // on regarde si la case a déjà quelque chose dessus
                bool caseOccupee = false;
                foreach (UIElement enfant in GridTableJeu.Children)
                {
                    if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                    {
                        if (enfant is Border b && b.Child != null && ((Shape)b.Child).Fill != Brushes.Transparent)
                        {
                            caseOccupee = true;
                            break;
                        }
                    }
                }

                // dès qu'on trouve la première case libre en bas
                if (!caseOccupee)
                {
                    // on récupère la case pour colorier son jeton
                    foreach (UIElement enfant in GridTableJeu.Children)
                    {
                        if (Grid.GetRow(enfant) == (ligne + 1) && Grid.GetColumn(enfant) == colonne)
                        {
                            if (enfant is Border b && b.Child is Shape jeton)
                            {
                                if (Partie.JoueurCourant == Partie.Joueur1)
                                {
                                    jeton.Fill = couleurJ1; // le jeton prend la couleur du joueur 1
                                    RunTxtBlockAuTourDe.Text = Partie.Joueur2.Nom;
                                    RunTxtBlockAuTourDe.Foreground = couleurJ2;

                                    // on prévient la grille du nouveau jeton
                                    Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur1);
                                    AjouterHistorique(Partie.Joueur1.Nom, colonne);

                                    if (Partie.Grille.VérifierAlignements(Partie.Configuration.NbJetonAAligner) == EtatCase.Vide)
                                    {
                                        if (Partie.Joueur2.NiveauVirtuel == NiveauVirtuel.Humain)
                                        {
                                            // on passe au joueur 2
                                            Partie.JoueurCourant = Partie.Joueur2;
                                            DemarrerTimerSiBesoin();
                                        }
                                        else if (Partie.Joueur2.NiveauVirtuel == NiveauVirtuel.Intelligent)
                                        {
                                            // on bloque les touches pendant que l'IA "réfléchit"
                                            alignement = true;
                                            // on stoppe le timer pour que l'IA ne récupère pas le temps restant
                                            ArreterTimer();
                                            await Task.Delay(2000);
                                            // on relance un timer tout neuf pour l'IA si besoin
                                            DemarrerTimerSiBesoin();
                                            alignement = false;

                                            JouerIntelligent();
                                        }
                                        else
                                        {
                                            alignement = true;
                                            // on stoppe le timer pour que l'IA ne récupère pas le temps restant
                                            ArreterTimer();
                                            await Task.Delay(2000);
                                            // on relance un timer tout neuf pour l'IA si besoin
                                            DemarrerTimerSiBesoin();
                                            alignement = false;

                                            JouerIdiot();
                                        }
                                    }
                                    else
                                        Partie.JoueurCourant = Partie.Joueur2;
                                }
                                else
                                {
                                    jeton.Fill = couleurJ2; // le jeton prend la couleur du joueur 2

                                    // on repasse au joueur 1
                                    Partie.JoueurCourant = Partie.Joueur1;
                                    RunTxtBlockAuTourDe.Text = Partie.Joueur1.Nom;
                                    RunTxtBlockAuTourDe.Foreground = couleurJ1;

                                    DemarrerTimerSiBesoin();

                                    // on prévient la grille du nouveau jeton
                                    Partie.Grille.ChangerValeurCase(ligne, colonne, EtatCase.Joueur2);
                                    AjouterHistorique(Partie.Joueur2.Nom, colonne);
                                }

                                nbCoups += 1;

                                if (Partie.Grille.VérifierAlignements(Partie.Configuration.NbJetonAAligner) != EtatCase.Vide)
                                {
                                    Joueur joueur;
                                    if (Partie.JoueurCourant == Partie.Joueur1)
                                        joueur = Partie.Joueur2;
                                    else
                                        joueur = Partie.Joueur1;

                                    coupDecisif = joueur.Nom + ColonneToLettre(colonne);

                                    DateTime Fin = DateTime.Now;
                                    TimeSpan intervalle = Fin - DebutPartie;
                                    DureePartie = intervalle.TotalSeconds;

                                    alignement = true;
                                    PartieFini(joueur);
                                }
                                DemarrerTimerSiBesoin();
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            if (premierCoup == null)
            {
                premierCoup = Partie.Joueur1.Nom + ColonneToLettre(colonne);
            }
            DemarrerTimerSiBesoin();
        }

        private void PartieFini(Joueur Gagnant)
        {
            if (Challenge == null)
            {
                Partie.FinirPartie(premierCoup!, coupDecisif!, DureePartie, nbCoups, Gagnant);
                AjouterHistoriqueGagne(Gagnant);
                FenetreVictoire fenetreVictoire = new FenetreVictoire(Partie, ContrasteMarque, TailleTexte);
                fenetreVictoire.Show();
                this.Close();
            }
            else
            {
                // fin de partie en mode challenge : on arrête le timer pour éviter d'autres actions
                ArreterTimer();
                Partie.FinirPartie(premierCoup!, coupDecisif!, DureePartie, nbCoups, Gagnant);
                AjouterHistoriqueGagne(Gagnant);
                if (Gagnant == Partie.Joueur1)
                {
                    Challenge.AjouterPointJoueur(1);
                    TxtBlockScoreJoueur1.Text = Challenge.ScoreJoueur1.ToString();
                }
                else
                {
                    Challenge.AjouterPointJoueur(2);
                    TxtBlockScoreJoueur2.Text = Challenge.ScoreJoueur1.ToString();
                }


                BtnFinirChallenge.Visibility = Visibility.Visible;
                BtnRelancerPartie.Visibility = Visibility.Visible;
            }
        }
    }
}