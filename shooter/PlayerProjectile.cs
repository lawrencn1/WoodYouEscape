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
    public enum ProjectileType
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
        public UIElement Sprite { get; private set; }
        public bool IsMarkedForRemoval { get; set; } = false;

        public PlayerProjectile(double x, double y, double dirX, double dirY, ProjectileType type = ProjectileType.Standard)
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
}
