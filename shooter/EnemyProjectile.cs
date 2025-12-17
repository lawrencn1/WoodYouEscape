using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace shooter
{

    public class EnemyProjectile
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double DirX { get; set; }
        public double DirY { get; set; }
        public double Speed { get; set; } = 15;
        public Image Sprite { get; private set; }
        public bool IsMarkedForRemoval { get; set; } = false;
        
        private ScaleTransform _scaleTransform;
        private RotateTransform _rotateTransform;

        public EnemyProjectile(double x, double y, double dirX, double dirY)
        {
            X = x;
            Y = y;
            DirX = dirX;
            DirY = dirY;
            _scaleTransform = new ScaleTransform();
            _rotateTransform = new RotateTransform();

            Sprite = new Image
            {
                Width = 30,
                Height = 30,
                Stretch = Stretch.Uniform,
                RenderTransformOrigin = new Point(0.5, 0.5),
                Source = TextureManager.EnemyProjectileTexture,
                RenderTransform = _rotateTransform
            };

            double angle = Math.Atan2(DirY, DirX) * (180 / Math.PI);
            _rotateTransform.Angle = angle;
            
            if (DirX < 0)
            {
                _scaleTransform.ScaleX = -1; // Flip Horizontally (Left)
            }
            else
            {
                _scaleTransform.ScaleX = 1;  // Normal (Right)
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

            // Cleanup if it goes off screen (Should not happen since porjectile has collision with play area)
            if (Y < -50 || Y > 2000 || X < -50 || X > 2000)
            {
                IsMarkedForRemoval = true;
            }
        }
    }
}
