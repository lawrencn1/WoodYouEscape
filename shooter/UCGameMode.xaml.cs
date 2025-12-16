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
    /// Interaction logic for UCGameMode.xaml
    /// </summary>
    public partial class UCGameMode : UserControl
    {
        public UCGameMode()
        {
            InitializeComponent();
        }

        private void validate_Click(object sender, RoutedEventArgs e)
        {

        }


        private void normal_Checked(object sender, RoutedEventArgs e)
        {
            if (validate != null)
            {
                validate.IsEnabled = true;
                MainWindow.GAMEMODE = "Normal";
            }
        }

        private void infinite_Checked(object sender, RoutedEventArgs e)
        {
            validate.IsEnabled = true;
            MainWindow.GAMEMODE = "Infinite";
        }
    }
}
