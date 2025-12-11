using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace shooter
{
    public enum ProjectileTypePlayer
    {
        Standard,
        Sniper,
        MachineGun,
        Rocket
    }

    public class PlayerProjectile
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double DirX { get; set; }
        public double DirY { get; set; }
        public double Speed { get; set; } = 15;
        public Image Sprite { get; private set; }
        public bool IsMarkedForRemoval { get; set; } = false;
        
        private RotateTransform _rotationTransform;
        private double _rotationSpeed;



        public PlayerProjectile(double x, double y, double dirX, double dirY, ProjectileTypePlayer type = ProjectileTypePlayer.Standard)
        {
            X = x;
            Y = y;
            DirX = dirX;
            DirY = dirY;

            switch (type)
            {
                case ProjectileTypePlayer.Sniper:
                    Speed = 60; // Very fast
                    //Sprite = new Rectangle
                    //{
                    //    Width = 6,
                    //    Height = 20,
                    //    Fill = Brushes.Yellow,
                    //    RenderTransform = new RotateTransform(0) // Logic for rotation could be added later
                    //};
                    break;

                case ProjectileTypePlayer.MachineGun:
                    Speed = 25; // Fast
                    //Sprite = new Ellipse // Round bullets
                    //{
                    //    Width = 8,
                    //    Height = 8,
                    //    Fill = Brushes.Orange
                    //};
                    break;

                case ProjectileTypePlayer.Rocket:
                    Speed = 7; // Slow
                    //Sprite = new Rectangle
                    //{
                    //    Width = 20,
                    //    Height = 20,
                    //    Fill = Brushes.DarkRed,
                    //    Stroke = Brushes.Black,
                    //    StrokeThickness = 2
                    //};
                    break;

                case ProjectileTypePlayer.Standard:
                default:
                    Speed = 300;
                    var flipTransform = new ScaleTransform();

                    if (DirX < 0)
                    {
                        flipTransform.ScaleX = -1;
                        _rotationSpeed = -540;
                    }
                    else
                    {
                        flipTransform.ScaleX = 1;
                        _rotationSpeed = 540;
                    }
                    _rotationTransform = new RotateTransform(0);


                    var transformGroup = new TransformGroup();
                    transformGroup.Children.Add(flipTransform);
                    transformGroup.Children.Add(_rotationTransform);


                    // Create the image sprite
                    var axeImage = new Image
                    {
                        Width = 120, // Adjust size as needed
                        Height = 120,
                        Source = TextureManager.AxeTexture,
                        Stretch = Stretch.Uniform,
                        RenderTransformOrigin = new Point(0.5, 0.5), // Center point for spinning
                        RenderTransform = transformGroup
                    };
                    Sprite = axeImage;
                    break;
            }

            if (Sprite != null)
            {
                Canvas.SetLeft(Sprite, X);
                Canvas.SetTop(Sprite, Y);
            }

        }


        public void Update(double deltaTime)
        {
            X += DirX * Speed * deltaTime;
            Y += DirY * Speed * deltaTime;

            if (_rotationTransform != null)
            {
                _rotationTransform.Angle += _rotationSpeed * deltaTime;
            }

            if (Sprite != null)
            {
                Canvas.SetLeft(Sprite, X);
                Canvas.SetTop(Sprite, Y);
            }

            if (Y < -50 || Y > 2000 || X < -50 || X > 2000)
            {
                IsMarkedForRemoval = true;
            }
        }
    }
}
