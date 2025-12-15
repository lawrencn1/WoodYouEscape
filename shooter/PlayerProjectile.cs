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
        public bool CausesBurn { get; set; } = false; 



        public PlayerProjectile(double x, double y, double dirX, double dirY, ProjectileTypePlayer type = ProjectileTypePlayer.Standard)
        {
            X = x;
            Y = y;
            DirX = dirX;
            DirY = dirY;

            // 1. Initialize Transform & Image container
            _rotationTransform = new RotateTransform();
            
            Sprite = new Image
            {
                Stretch = Stretch.Uniform,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = _rotationTransform
            };

            // 2. Configure Stats & Textures based on Type
            switch (type)
            {
                case ProjectileTypePlayer.FireAxe:
                    Speed = 500;

                    CausesBurn = true; // TRIGGERS THE BURN

                    Sprite.Width = 120;
                    Sprite.Height = 120;
                    // Ensure you have a FireAxe texture, or fallback to Axe
                    Sprite.Source = TextureManager.AxeTexture;

                    // Fast Spin
                    _rotationSpeed = (DirX < 0) ? -540 : 540;
                    break;

                case ProjectileTypePlayer.MachineGun:
                    Speed = 900; // Very fast
                    

                    Sprite.Width = 20;
                    Sprite.Height = 20;
                    // Placeholder if you don't have a bullet texture yet
                    Sprite.Source = TextureManager.AxeTexture;

                    // No spin, or align to direction
                    double angleBullet = Math.Atan2(DirY, DirX) * (180 / Math.PI);
                    _rotationTransform.Angle = angleBullet;
                    break;

                case ProjectileTypePlayer.Rocket:
                    Speed = 350; // Slow
                    
                    Sprite.Width = 50;
                    Sprite.Height = 20;
                    Sprite.Source = TextureManager.AxeTexture;

                    // Rotate to face direction of travel
                    double angleRocket = Math.Atan2(DirY, DirX) * (180 / Math.PI);
                    _rotationTransform.Angle = angleRocket;
                    break;

                case ProjectileTypePlayer.Standard:
                default:
                    Speed = 450;

                    Sprite.Width = 120;
                    Sprite.Height = 120;
                    Sprite.Source = TextureManager.AxeTexture;

                    // Standard Spin
                    _rotationSpeed = (DirX < 0) ? -540 : 540;
                    break;
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
}
