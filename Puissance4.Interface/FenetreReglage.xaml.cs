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
    /// Logique d'interaction pour FenetreReglage.xaml
    /// </summary>
    public partial class FenetreReglage : Window
    {
        public FenetreReglage()
        {
            InitializeComponent();
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
    }
}
