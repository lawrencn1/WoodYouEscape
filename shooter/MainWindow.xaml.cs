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

    public class Player
    {
        public double X;
        public double Y;
        public double Vitesse;
        public int PV;

        public UIElement Sprite;

        public Player(double x, double y, double vitesse)
        {
            X = x;
            Y = y;
            Vitesse = vitesse;
            PV = 100;

            Sprite = new System.Windows.Shapes.Ellipse
            {
                Width = 40,
                Height = 40,
                Fill = Brushes.Blue
            };

    }
        public void Deplacement(double dx, double dy, double deltaTime)
        {
            X += dx * Vitesse * deltaTime;
            Y += dy * Vitesse * deltaTime;
            UpdatePosition();
        }
        public void UpdatePosition()
        {
            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);
        }
        public void Degat(int qte)
        {
            PV -= qte;
            if (PV <= 0)
            {
            }
        }
    }

    public enum ProjectileType
    {
    Standard,
    Sniper,
    MachineGun,
    Rocket
    }
public class Bullet
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double DirX { get; set; }
        public double DirY { get; set; }
        public double Speed { get; set; } = 15;
        public UIElement Sprite { get; private set; }
        public bool IsMarkedForRemoval { get; set; } = false;

        public Bullet(double x, double y, double dirX, double dirY, ProjectileType type = ProjectileType.Standard)
        {
        X = x;
        Y = y;
        DirX = dirX;
        DirY = dirY;

        switch (type)
        {
            case ProjectileType.Sniper:
                Speed = 80; // Very fast
                Sprite = new Rectangle
                {
                    Width = 6,
                    Height = 20, 
                    Fill = Brushes.Yellow,
                    RenderTransform = new RotateTransform(0) // Logic for rotation could be added later
                };
                break;

            case ProjectileType.MachineGun:
                Speed = 25; // Fast
                Sprite = new Ellipse // Round bullets
                {
                    Width = 8,
                    Height = 8,
                    Fill = Brushes.Orange
                };
                break;

            case ProjectileType.Rocket:
                Speed = 7; // Slow
                Sprite = new Rectangle
                {
                    Width = 20,
                    Height = 20,
                    Fill = Brushes.DarkRed,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                break;

            case ProjectileType.Standard:
            default:
                Speed = 15;
                Sprite = new Rectangle
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red
                };
                break;
        }

        Canvas.SetLeft(Sprite, X);
        Canvas.SetTop(Sprite, Y);

        }

        public void Update(double deltaTime)
        {
            // Move along the calculated vector
            X += DirX * Speed;
            Y += DirY * Speed;

            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);

            // Cleanup if it goes off screen (checking all boundaries now)
            if (Y < -50 || Y > 2000 || X < -50 || X > 2000)
            {
                IsMarkedForRemoval = true;
            }
        }
    }



    public class InputManager
    {
        public bool IsShootPressed;
        public bool IsRightPressed;
        public bool IsLeftPressed;
        public bool IsUpPressed;
        public bool IsDownPressed;

        public bool IsKey1Pressed;
        public bool IsKey2Pressed;
        public bool IsKey3Pressed;
        public bool IsKey4Pressed;

    public Point MousePosition { get; set; }
    public void OnKeyPressed(Key key)
    {
        if (key == Key.Space) IsShootPressed = true;

        if (key == Key.D1 || key == Key.NumPad1) IsKey1Pressed = true;
        if (key == Key.D2 || key == Key.NumPad2) IsKey2Pressed = true;
        if (key == Key.D3 || key == Key.NumPad3) IsKey3Pressed = true;
        if (key == Key.D4 || key == Key.NumPad4) IsKey4Pressed = true;

        if (key == Key.Right || key == Key.D) IsRightPressed = true;
        if (key == Key.Left || key == Key.Q) IsLeftPressed = true;
        if (key == Key.Up || key == Key.Z) IsUpPressed = true;
        if (key == Key.Down || key == Key.S) IsDownPressed = true;
    }

    public void OnKeyUp(Key key)
    {
        if (key == Key.Space) IsShootPressed = false;

        if (key == Key.D1 || key == Key.NumPad1) IsKey1Pressed = false;
        if (key == Key.D2 || key == Key.NumPad2) IsKey2Pressed = false;
        if (key == Key.D3 || key == Key.NumPad3) IsKey3Pressed = false;
        if (key == Key.D4 || key == Key.NumPad4) IsKey4Pressed = false;

        if (key == Key.Right || key == Key.D) IsRightPressed = false;
        if (key == Key.Left || key == Key.Q) IsLeftPressed = false;
        if (key == Key.Up || key == Key.Z) IsUpPressed = false;
        if (key == Key.Down || key == Key.S) IsDownPressed = false;
    }
}

  