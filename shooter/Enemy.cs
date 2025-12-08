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
    public class Enemy
    {
        private double x;
        private double y;
        private double vitesse;
        private int pv;

        private UIElement sprite;

        public double X
        {
            get
            {
                return this.X1;
            }

            set
            {
                this.X1 = value;
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

        public double Vitesse
        {
            get
            {
                return this.vitesse;
            }

            set
            {
                this.vitesse = value;
            }
        }

        public int Pv
        {
            get
            {
                return this.pv;
            }

            set
            {
                if (pv < 0)
                {
                    throw new ArgumentOutOfRangeException("Les PV doivent être positifs");
                }
                this.pv = value;
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

        public double X1 { get => this.x; set => this.x = value; }

        public Enemy(double x, double y, double vitesse)
        {
            this.X = x;
            this.Y = y;
            this.Vitesse = vitesse;
            this.Pv = 100;

            Sprite = new System.Windows.Shapes.Ellipse
            {
                Width = 40,
                Height = 40,
                Fill = Brushes.Red
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
            Pv -= qte;
            if (Pv <= 0)
            {
            }
        }
    }
}
