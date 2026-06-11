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

namespace Puissance4.Interface
{
    /// <summary>
    /// Logique d'interaction pour PopUpCouleurs.xaml
    /// </summary>
    public partial class PopUpCouleurs : Window
    {
        public PopUpCouleurs()
        {
            InitializeComponent();
        }

        public PopUpCouleurs(bool contrasteMarque)
        {
            InitializeComponent();
            if (contrasteMarque)
            {
                this.Background = Brushes.White;
                this.FontFamily = new FontFamily("Verdana");
                this.Foreground = Brushes.Black;
            }
        }

        public string SelectionCouleurs { get; private set; } = "";
        public string SelectionFormes { get; private set; } = "";

        private void BorderCouleurs_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border borderClique = (Border)sender;

            // si la couleur n'est pas encore choisie on la sélectionne, sinon on l'enlève
            if (borderClique.BorderBrush == Brushes.Transparent)
            {
                DéselectionnerAutresCouleurs();
                borderClique.BorderBrush = Brushes.White;
                SelectionCouleurs = (string)borderClique.Tag;
            }
            else
            {
                borderClique.BorderBrush = Brushes.Transparent;
                SelectionCouleurs = "";
            }

            // quand une couleur et une forme sont choisies on ferme la pop-up
            if (SelectionCouleurs != "" && SelectionFormes != "")
            {
                this.Close();
            }
        }

        private void BorderFormes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border borderClique = (Border)sender;

            // si la forme n'est pas encore choisie on la sélectionne, sinon on l'enlève
            if (borderClique.BorderBrush == Brushes.Transparent)
            {
                DéselectionnerAutresFormes();
                borderClique.BorderBrush = Brushes.White;
                SelectionFormes = (string)borderClique.Tag;
            }
            else
            {
                borderClique.BorderBrush = Brushes.Transparent;
                SelectionFormes = "";
            }

            // quand une couleur et une forme sont choisies on ferme la pop-up
            if (SelectionCouleurs != "" && SelectionFormes != "")
            {
                this.Close();
            }
        }

        private void DéselectionnerAutresCouleurs()
        {
            // on enlève la bordure blanche de toutes les autres couleurs
            for (int i = 1; i <= 6; i++)
            {
                Border border = (Border)FindName("BorderCouleur" + i.ToString());
                border.BorderBrush = Brushes.Transparent;
            }
        }

        private void DéselectionnerAutresFormes()
        {
            // on enlève la bordure blanche de toutes les autres formes
            for (int i = 1; i <= 6; i++)
            {
                Border border = (Border)FindName("BorderForme" + i.ToString());
                border.BorderBrush = Brushes.Transparent;
            }
        }
    }
}