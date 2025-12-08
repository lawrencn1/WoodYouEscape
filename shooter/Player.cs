using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace shooter
{
    public class Player
    {
        private double x;
        private double y;
        private double speed;
        private int hp;

        private UIElement sprite;

        public double X
        {
            get
            {
                return this.x;
            }

            set
            {
                this.x = value;
            }
        }

        public double Y
        {
            get
            {
                return this.y;
            }

            set
            {
                this.y = value;
            }
        }

        public double Speed
        {
            get
            {
                return this.speed;
            }

            set
            {
                this.speed = value;
            }
        }

        public int Hp // Health Points
        {
            get
            {
                return this.hp;
            }

            set
            {
                if (hp < 0)
                {
                    throw new ArgumentOutOfRangeException("HP must be a positive number");
                }
                this.hp = value;
            }
        }

        public UIElement Sprite
        {
            get
            {
                return this.sprite;
            }

            set
            {
                this.sprite = value;
            }
        }

        public Player(double x, double y, double vitesse)
        {
            // Spawn position, movement speed, Health points & Sprite
            this.X = x;
            this.Y = y;
            this.Speed = vitesse;
            this.Hp = 100;

            Sprite = new System.Windows.Shapes.Ellipse
            {
                Width = 40,
                Height = 40,
                Fill = Brushes.Blue
            };

        }
        public void Deplacement(double DirX, double DirY, double deltaTime)
        {
            X += DirX * Speed * deltaTime;
            Y += DirY * Speed * deltaTime;
            UpdatePosition();
        }
        public void UpdatePosition()
        {
            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);
        }
        public void Damage(int quantity)
        {
            Hp -= quantity;
            if (Hp <= 0)
            {
            }
        }
    }
}
