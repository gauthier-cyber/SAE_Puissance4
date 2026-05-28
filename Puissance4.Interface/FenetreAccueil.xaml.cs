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
        }

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implémenter la logique pour démarrer une nouvelle partie
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
    }
}