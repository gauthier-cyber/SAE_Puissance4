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
    /// Logique d'interaction pour PopUpSauvegarde.xaml
    /// </summary>
    public partial class PopUpSauvegarde : Window
    {
        public PopUpSauvegarde()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Faire apparaître un message d'erreur "Il n'y a pas de sauvegarde disponible"
            MessageBox.Show("Il n'y a pas de sauvegarde disponible");
        }
    }
}
