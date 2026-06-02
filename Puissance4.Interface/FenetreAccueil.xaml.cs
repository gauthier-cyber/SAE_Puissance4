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

namespace Puissance4.Interface
{
    /// <summary>
    /// Interaction logic for FenetreAccueil.xaml
    /// </summary>
    public partial class FenetreAccueil : Window
    {
        public FenetreAccueil()
        {
            InitializeComponent();
            DésactiverBtnIA();
        }

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            // Si le bouton a déjà été cliqué, retirer les bordures blanches, faire apparaître le bouton BtnContinueGame et cacher le border BorderNewGame
            // Sinon, faire apparaître des bordures blanches au boutton cliqué, faire apparaître le border BorderNewGame et cacher le bouton BtnContinueGame
            string tag = (string)BtnNewGame.Tag;
            if (tag == "EstCliqué")
            {
                BtnNewGame.BorderThickness = new Thickness(0);
                BtnContinueGame.Visibility = Visibility.Visible;
                BorderNewGame.Visibility = Visibility.Hidden;
                BtnNewGame.Tag = "";
            }
            else
            {
                BtnNewGame.BorderThickness = new Thickness(5);
                BorderNewGame.Visibility = Visibility.Visible;
                BtnContinueGame.Visibility = Visibility.Hidden;
                BtnNewGame.Tag = "EstCliqué";
            }
        }

        private void BtnContinueGame_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la PopUpSauvegarde
            PopUpSauvegarde popUp = new PopUpSauvegarde();
            popUp.ShowDialog();
        }

        private void BtnContinueGame_GotFocus(object sender, RoutedEventArgs e)
        {
            // Faire apparaître des bordures blanches au boutton cliqué
            BtnContinueGame.BorderThickness = new Thickness(5);
        }

        private void BtnContinueGame_LostFocus(object sender, RoutedEventArgs e)
        {
            // Faire disparaître les bordures blanches au boutton décliqué
            BtnContinueGame.BorderThickness = new Thickness(0);
        }

        private void BtnJouerA2_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre de réglages et fermer la fenêtre d'accueil
            FenetreReglage fenetreReglage = new FenetreReglage();
            fenetreReglage.Show();
            this.Close();
        }

        private void BtnJouerSeul_Click(object sender, RoutedEventArgs e)
        {
            // Faire apparaître des bordures blanches et activer les boutons BtnIAIdiot et BtnIAIntelligent
            BtnJouerSeul.BorderThickness = new Thickness(5);
            ActiverBtnIA();
        }

        private void BtnIA_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre de réglages et fermer la fenêtre d'accueil
            FenetreReglage fenetreReglage = new FenetreReglage();
            fenetreReglage.Show();
            this.Close();
        }

        private void DésactiverBtnIA()
        {
            // Désactiver les boutons BtnIAIdiot et BtnIAIntelligent
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
            // Activer les boutons BtnIAIdiot et BtnIAIntelligent
            BtnIAIdiot.IsEnabled = true;
            BtnIAIntelligent.IsEnabled = true;

            SolidColorBrush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c9c9c9"));
            BtnIAIdiot.Background = color;
            BtnIAIntelligent.Background = color;
        }
    }
}