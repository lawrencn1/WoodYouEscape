using shooter;
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
        FireAxe,
        LightAxe,  
        HeavyAxe   
    }
}

    public class PlayerProjectile
    {
        public ProjectileTypePlayer Type { get; private set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double DirX { get; set; }
        public double DirY { get; set; }
        public double Speed { get; set; } = 15;
        public Image Sprite { get; private set; }
        public bool IsMarkedForRemoval { get; set; } = false;
        
        private RotateTransform _rotationTransform;
        private double _rotationSpeed;

        private ScaleTransform _scaleTransform;
        public bool CausesBurn { get; set; } = false; 



        public PlayerProjectile(double x, double y, double dirX, double dirY, ProjectileTypePlayer type = ProjectileTypePlayer.Standard)
        {
            X = x;
            Y = y;
            DirX = dirX;
            DirY = dirY;

            this.Type = type;

            // 1. Initialize Transform & Image container
            _rotationTransform = new RotateTransform();
            _scaleTransform = new ScaleTransform();
            
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(_scaleTransform);     
            transformGroup.Children.Add(_rotationTransform);


            Sprite = new Image
            {
                Stretch = Stretch.Uniform,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = transformGroup
            };

            // 2. Configure Stats & Textures based on Type
            switch (type)
            {
                case ProjectileTypePlayer.FireAxe:
                {
                    Speed = 500;

                    CausesBurn = true; // TRIGGERS THE BURN

                    Sprite.Width = 120;
                    Sprite.Height = 120;
                    // Ensure you have a FireAxe texture, or fallback to Axe
                    Sprite.Source = TextureManager.FireAxeTexture;

                    if (DirX < 0)
                    {
                        _scaleTransform.ScaleX = -1; // Flip Horizontally
                        _rotationSpeed = -540;       // Spin Counter-Clockwise
                    }
                    else
                    {
                        _scaleTransform.ScaleX = 1;  // Normal
                        _rotationSpeed = 540;        // Spin Clockwise
                    }

                    break;
                }

                case ProjectileTypePlayer.LightAxe:
                {
                    Speed = 690; // Very fast
                    Sprite.Width = 90;
                    Sprite.Height = 90;
                    Sprite.Source = TextureManager.LightAxeTexture;

                    // No spin, or align to direction
                    if (DirX < 0)
                    {
                        _scaleTransform.ScaleX = -1; // Flip Horizontally
                        _rotationSpeed = -630;       // Spin Counter-Clockwise
                    }
                    else
                    {
                        _scaleTransform.ScaleX = 1;  // Normal
                        _rotationSpeed = 630;        // Spin Clockwise
                    }
                    break;
                }

                case ProjectileTypePlayer.HeavyAxe:
                {
                    Speed = 300; // Slow

                    Sprite.Width = 130;
                    Sprite.Height = 130;
                    Sprite.Source = TextureManager.HeavyAxeTexture;

                    // Rotate to face direction of travel
                    if (DirX < 0)
                    {
                        _scaleTransform.ScaleX = -1; // Flip Horizontally
                        _rotationSpeed = -495;       // Spin Counter-Clockwise
                    }
                    else
                    {
                        _scaleTransform.ScaleX = 1;  // Normal
                        _rotationSpeed = 495;        // Spin Clockwise
                    }
                    break;
                }
                case ProjectileTypePlayer.Standard: 
                default:
                {
                    Speed = 450;

                    Sprite.Width = 120;
                    Sprite.Height = 120;
                    Sprite.Source = TextureManager.AxeTexture;

                    // Standard Spin
                    if (DirX < 0)
                    {
                        _scaleTransform.ScaleX = -1; // Flip Horizontally
                        _rotationSpeed = -540;       // Spin Counter-Clockwise
                    }
                    else
                    {
                        _scaleTransform.ScaleX = 1;  // Normal
                        _rotationSpeed = 540;        // Spin Clockwise
                    }
                    break; 
                }
            }

            // 3. Set Initial Position
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

