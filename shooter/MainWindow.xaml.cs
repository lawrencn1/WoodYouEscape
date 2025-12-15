using shooter;
using System.Diagnostics;
using System.Numerics;
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
using System.Windows.Threading;

namespace shooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string Difficulty { get; set; }
        private GameEngine engine;

        public MainWindow()
        {
            InitializeComponent();
            engine = new GameEngine(canvas);
            engine.Start();
            MouseMove += Window_MouseMove;
            Loaded += (s, e) => canvas.Focus();

            
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            engine.inputMng.OnKeyPressed(e.Key);
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            engine.inputMng.OnKeyUp(e.Key);
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            engine.inputMng.MousePosition = e.GetPosition(canvas);
        }
    }

    }

