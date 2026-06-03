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

        private string selectionCouleurs = "";
        private string selectionFormes = "";

        private void BorderCouleurs_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border borderClique = (Border)sender;

            if (borderClique.BorderBrush == Brushes.Transparent)
            {
                DéselectionnerAutresCouleurs();
                borderClique.BorderBrush = Brushes.White;
                selectionCouleurs = (string)borderClique.Tag;
            }
            else
            {
                borderClique.BorderBrush = Brushes.Transparent;
                selectionCouleurs = "";
            }

            if (selectionCouleurs != "" && selectionFormes != "")
            {
                MessageBox.Show(selectionCouleurs + " " + selectionFormes);
                this.Close();
            }
        }

        private void BorderFormes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border borderClique = (Border)sender;

            if (borderClique.BorderBrush == Brushes.Transparent)
            {
                DéselectionnerAutresFormes();
                borderClique.BorderBrush = Brushes.White;
                selectionFormes = (string)borderClique.Tag;
            }
            else
            {
                borderClique.BorderBrush = Brushes.Transparent;
                selectionFormes = "";
            }

            if (selectionCouleurs != "" && selectionFormes != "")
            {
                MessageBox.Show(selectionCouleurs + " " + selectionFormes);
                this.Close();
            }
        }

        private void DéselectionnerAutresCouleurs()
        {
            for (int i = 1; i <= 6; i++)
            {
                Border border = (Border)FindName("BorderCouleur" + i.ToString());
                border.BorderBrush = Brushes.Transparent;
            }
        }

        private void DéselectionnerAutresFormes()
        {
            for (int i = 1; i <= 6; i++)
            {
                Border border = (Border)FindName("BorderForme" + i.ToString());
                border.BorderBrush = Brushes.Transparent;
            }
        }
    }
}
