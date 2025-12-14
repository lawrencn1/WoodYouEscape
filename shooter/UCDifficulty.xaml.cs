using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace shooter
{
    /// <summary>
    /// Interaction logic for UCDifficulty.xaml
    /// </summary>
    public partial class UCDifficulty : UserControl
    {
        public UCDifficulty()
        {
            InitializeComponent();
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {

        }

        private void easy_Click(object sender, RoutedEventArgs e)
        {
            play.IsEnabled = true;
            MainWindow.Difficulty = "easy";
        }

        private void normal_Click(object sender, RoutedEventArgs e)
        {
            play.IsEnabled = true;
            MainWindow.Difficulty = "normal";
        }

        private void hard_Checked(object sender, RoutedEventArgs e)
        {
            play.IsEnabled = true;
            MainWindow.Difficulty = "hard";
        }
    }
}
