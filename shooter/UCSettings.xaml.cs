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
    /// Interaction logic for UCSettings.xaml
    /// </summary>
    public partial class UCSettings : UserControl
    {
        public UCSettings()
        {
            InitializeComponent();
            volume.Value = SFXManager.MasterVolume;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {

        }
        private void volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update the Audio Manager immediately when slider moves
            SFXManager.SetVolume(volume.Value);
        }
    }
}
