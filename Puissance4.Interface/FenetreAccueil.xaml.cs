using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Puissance4.Systeme;

namespace Puissance4.Interface
{
    /// <summary>
    /// fenêtre d'accueil
    /// </summary>
    public partial class FenetreAccueil : Window
    {
        public Joueur? Joueur1 { get; set; }
        public Joueur? Joueur2 { get; set; }
        private bool ContrasteMarque;

        public FenetreAccueil()
        {
            InitializeComponent();
            DésactiverBtnIA();
        }

        public FenetreAccueil(bool contrasteMarque)
        {
            InitializeComponent();
            DésactiverBtnIA();
            Main(contrasteMarque);
        }

        public void Main(bool contrasteMarque)
        {
            ContrasteMarque = contrasteMarque;

            if (contrasteMarque)
            {
                this.Background = Brushes.White;
                this.FontFamily = new FontFamily("Verdana");
                this.Foreground = Brushes.Black;
                BtnNouvellePartie.Foreground = Brushes.Black;
                BtnReprendrePartie.Foreground = Brushes.Black;
                BorderNouvellePartie.Background = Brushes.White;
            }
        }

        private void BtnNouvellePartie_Click(object sender, RoutedEventArgs e)
        {
            // si le bouton est déjà cliqué : on enlève la bordure blanche, on remontre BtnReprendrePartie et on cache BorderNouvellePartie
            // sinon : on met une bordure blanche, on montre BorderNouvellePartie et on cache BtnReprendrePartie
            string tag = (string)BtnNouvellePartie.Tag;
            if (tag == "EstCliqué")
            {
                BtnNouvellePartie.BorderThickness = new Thickness(0);
                BtnReprendrePartie.Visibility = Visibility.Visible;
                BorderNouvellePartie.Visibility = Visibility.Hidden;
                BtnNouvellePartie.Tag = "";
            }
            else
            {
                BtnNouvellePartie.BorderThickness = new Thickness(5);
                BorderNouvellePartie.Visibility = Visibility.Visible;
                BtnReprendrePartie.Visibility = Visibility.Hidden;
                BtnNouvellePartie.Tag = "EstCliqué";
            }
        }

        private void BtnReprendrePartie_Click(object sender, RoutedEventArgs e)
        {
            // on ouvre la pop-up des sauvegardes
            PopUpSauvegarde popUp = new PopUpSauvegarde(ContrasteMarque);
            popUp.ShowDialog();
        }

        private void BtnReprendrePartie_GotFocus(object sender, RoutedEventArgs e)
        {
            // on met une bordure blanche quand le bouton est sélectionné
            BtnReprendrePartie.BorderThickness = new Thickness(5);
        }

        private void BtnReprendrePartie_LostFocus(object sender, RoutedEventArgs e)
        {
            // on enlève la bordure blanche quand on quitte le bouton
            BtnReprendrePartie.BorderThickness = new Thickness(0);
        }

        private void BtnJouerA2_Click(object sender, RoutedEventArgs e)
        {
            Joueur1 = new Joueur("Joueur 1");
            Joueur2 = new Joueur("Joueur 2");

            // on ouvre la fenêtre des réglages et on ferme l'accueil
            FenetreReglage fenetreReglage = new FenetreReglage(Joueur1, Joueur2);
            fenetreReglage.Show();
            this.Close();
        }

        private void BtnJouerSeul_Click(object sender, RoutedEventArgs e)
        {
            // on met une bordure blanche et on active les boutons des deux IA
            BtnJouerSeul.BorderThickness = new Thickness(5);
            ActiverBtnIA();
        }

        private void BtnIA_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string joueur;
            NiveauVirtuel niveau;
            if (btn.Name.Contains("Idiot"))
            {
                joueur = "IA Idiot";
                niveau = NiveauVirtuel.Idiot;
            }
            else
            {
                joueur = "IA Intelligent";
                niveau = NiveauVirtuel.Intelligent;
            }

            Joueur1 = new Joueur("Joueur 1");
            Joueur2 = new Joueur(joueur, niveau);

            // on ouvre la fenêtre des réglages et on ferme l'accueil
            FenetreReglage fenetreReglage = new FenetreReglage(Joueur1, Joueur2);
            fenetreReglage.Show();
            this.Close();
        }

        private void DésactiverBtnIA()
        {
            // on désactive les deux boutons IA
            BtnIAIdiot.IsEnabled = false;
            BtnIAIntelligent.IsEnabled = false;

            BtnIAIdiot.BorderThickness = new Thickness(0);
            BtnIAIntelligent.BorderThickness = new Thickness(0);

            SolidColorBrush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5c5a52"));
            BtnIAIdiot.Background = color;
            BtnIAIntelligent.Background = color;
        }

        private void ActiverBtnIA()
        {
            // on active les deux boutons IA
            BtnIAIdiot.IsEnabled = true;
            BtnIAIntelligent.IsEnabled = true;

            SolidColorBrush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c9c9c9"));
            BtnIAIdiot.Background = color;
            BtnIAIntelligent.Background = color;
        }
    }
}