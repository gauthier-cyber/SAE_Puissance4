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
    /// Logique d'interaction pour FenetreReglage.xaml
    /// </summary>
    public partial class FenetreReglage : Window
    {
        public Joueur Joueur1 { get; set; }
        public Joueur Joueur2 { get; set; }
        public Configuration ?Config { get; set; }

        private static string ?CouleurJoueur1;
        private static string ?CouleurJoueur2;
        private static string ?FormeJoueur;

        public FenetreReglage(Joueur J1, Joueur J2)
        {
            InitializeComponent();
            Joueur1 = J1;
            Joueur2 = J2;
        }

        private void SliderNbJetonAAligner_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtBlockNbJetonAAligner.Text = ((int)SliderNbJetonAAligner.Value).ToString();
        }

        private void CheckBoxTempsReflexion_Unchecked(object sender, RoutedEventArgs e)
        {
            SliderTempsReflexion.Visibility = Visibility.Hidden;
            TxtBlockTempsReflexion.Visibility = Visibility.Hidden;
        }

        private void CheckBoxTempsReflexion_Checked(object sender, RoutedEventArgs e)
        {
            SliderTempsReflexion.Visibility = Visibility.Visible;
            TxtBlockTempsReflexion.Visibility = Visibility.Visible;
        }

        private void SliderTempsReflexion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtBlockTempsReflexion.Text = ((int)SliderTempsReflexion.Value).ToString() + "s";
        }

        private void SliderTailleTexte_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtBlockTailleTexte.Text = ((int)SliderTailleTexte.Value).ToString();
        }

        private void BorderCouleursFormesJetons_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopUpCouleurs popUpCouleurs = new PopUpCouleurs();
            popUpCouleurs.ShowDialog();

            string couleurs = popUpCouleurs.SelectionCouleurs;
            string formes = popUpCouleurs.SelectionFormes;

            SolidColorBrush couleurGauche = (SolidColorBrush)(new BrushConverter().ConvertFrom(couleurs.Split(',')[0]))!;
            SolidColorBrush couleurDroite = (SolidColorBrush)(new BrushConverter().ConvertFrom(couleurs.Split(',')[1]))!;

            CouleurJoueur1 = couleurs.Split(',')[0];
            CouleurJoueur2 = couleurs.Split(',')[1];
            FormeJoueur = formes;

            switch (formes)
            {
                case "Rond":
                    CacherFormesSauf("Rond");
                    RondGauche.Visibility = Visibility.Visible;
                    RondDroite.Visibility = Visibility.Visible;
                    RondGauche.Fill = couleurGauche;
                    RondDroite.Fill = couleurDroite;
                    break;

                case "Triangle":
                    CacherFormesSauf("Triangle");
                    TriangleGauche.Visibility = Visibility.Visible;
                    TriangleDroite.Visibility = Visibility.Visible;
                    TriangleGauche.Fill = couleurGauche;
                    TriangleDroite.Fill = couleurDroite;
                    break;

                case "Croix":
                    CacherFormesSauf("Croix");
                    CroixGauche.Visibility = Visibility.Visible;
                    CroixDroite.Visibility = Visibility.Visible;
                    CroixGauche.Fill = couleurGauche;
                    CroixDroite.Fill = couleurDroite;
                    break;

                case "Etoile":
                    CacherFormesSauf("Etoile");
                    EtoileGauche.Visibility = Visibility.Visible;
                    EtoileDroite.Visibility = Visibility.Visible;
                    EtoileGauche.Fill = couleurGauche;
                    EtoileDroite.Fill = couleurDroite;
                    break;

                case "Carre":
                    CacherFormesSauf("Carre");
                    CarreGauche.Visibility = Visibility.Visible;
                    CarreDroite.Visibility = Visibility.Visible;
                    CarreGauche.Fill = couleurGauche;
                    CarreDroite.Fill = couleurDroite;
                    break;

                case "Losange":
                    CacherFormesSauf("Losange");
                    LosangeGauche.Visibility = Visibility.Visible;
                    LosangeDroite.Visibility = Visibility.Visible;
                    LosangeGauche.Fill = couleurGauche;
                    LosangeDroite.Fill = couleurDroite;
                    break;

                default:
                    break;
            }
        }

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            (int, int) TailleGrille = (0, 0);
            if (ComboBoxItemTailleGrille6_7.IsSelected)
            {
                TailleGrille = (6, 7);
            }
            else if (ComboBoxItemTailleGrille8_9.IsSelected)
            {
                TailleGrille = (8, 9);

            }
            else if (ComboBoxItemTailleGrille10_12.IsSelected)
            {
                TailleGrille = (10, 12);
            }

            int NbJetonAAligner = (int)SliderNbJetonAAligner.Value;

            int TempsReflexion = 0;
            if (CheckBoxTempsReflexion.IsChecked == false)
            {
                TempsReflexion = (int)SliderTempsReflexion.Value;
            }

            bool modeChallenge;
            if (RadioBtnChallengeOn.IsChecked == true)
            {
                modeChallenge = true;
            }
            else
            {
                modeChallenge = false;
            }

            Config = new Configuration(TailleGrille, NbJetonAAligner, CouleurJoueur1!, CouleurJoueur2!, FormeJoueur!, TempsReflexion);

            FenetreJeu fenetreJeu = new FenetreJeu(Joueur1, Joueur2, Config, modeChallenge);
            fenetreJeu.Show();
            this.Close();
        }

        private void CacherFormesSauf(string forme)
        {
            string[] formes = { "Rond", "Triangle", "Croix", "Etoile", "Carre", "Losange" };
            foreach (string s in formes) {
                if (s != forme)
                {
                    if (s == "Rond")
                    {
                        RondGauche.Visibility = Visibility.Hidden;
                        RondDroite.Visibility = Visibility.Hidden;
                    }
                    else if (s == "Carre")
                    {
                        CarreGauche.Visibility = Visibility.Hidden;
                        CarreDroite.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        Polygon polygonGauche = (Polygon)this.FindName(s + "Gauche");
                        Polygon polygonDroite = (Polygon)this.FindName(s + "Droite");
                        if (polygonGauche != null) polygonGauche.Visibility = Visibility.Hidden;
                        if (polygonDroite != null) polygonDroite.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
    }
}
