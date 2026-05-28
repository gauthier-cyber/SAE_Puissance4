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
            //TODO: Implémenter la logique pour continuer une partie en cours
        }
    }
}