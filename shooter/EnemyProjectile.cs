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
    public enum ProjectileTypeEnemy
    {
        Standard,
        Sniper,
        MachineGun,
        Rocket
    }

    public class EnemyProjectile
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double DirX { get; set; }
        public double DirY { get; set; }
        public double Speed { get; set; } = 15;
        public UIElement Sprite { get; private set; }
        public bool IsMarkedForRemoval { get; set; } = false;

        public EnemyProjectile(double x, double y, double dirX, double dirY)
        {
            X = x;
            Y = y;
            DirX = dirX;
            DirY = dirY;

            Sprite = new Rectangle
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Purple,
            };

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
}
