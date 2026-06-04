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
    /// Logique d'interaction pour FenetreVictoire.xaml
    /// </summary>
    public partial class FenetreVictoire : Window
    {
        public FenetreVictoire(Partie partie, Challenge challenge)
        {
            InitializeComponent();
            Partie Partie = partie;
            Challenge Challenge = challenge;
        }
    }
}
