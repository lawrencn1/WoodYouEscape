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
    public class Obstacles
    {
        private double x;
        private double y;
        private double heigth;
        private double width;

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

        public double Heigth
        {
            get
            {
                return this.heigth;
            }

            set
            {
                this.heigth = value;
            }
        }

        public double Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;
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


        public Obstacles(double x, double y, double heigth, double width)
        {
            this.X = x;
            this.Y = y;
            this.heigth = heigth;
            this.width = width;

            Sprite = new System.Windows.Shapes.Rectangle
            {
                Width = width,
                Height = heigth,
                Fill = Brushes.White
            };

            SetPosition(x, y);

        }

        public void SetPosition(double X, double Y)
        {
            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);
        }

        public void AddToCanva(Canvas canvas)
        {
            if (!canvas.Children.Contains(Sprite))
            {
                canvas.Children.Add(Sprite);
            }
        }
    }

}