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
    public enum ObstacleType
    {
       Wall, 
       Puddle,
       Start,
       End
    }

    public class Obstacles
    {
        private double x;
        private double y;
        private double heigth;
        private double width;

        public ObstacleType Type { get; private set; }

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


        public Obstacles(double x, double y, double heigth, double width, ObstacleType type)
        {
            this.X = x;
            this.Y = y;
            this.heigth = heigth;
            this.width = width;
            this.Type = type;

            Sprite = new System.Windows.Shapes.Rectangle
            {
                Width = width,
                Height = heigth,
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

        public bool ObstacleCollision(Rect spriteRect)
        {

                Rect obstacleRect = new Rect(X ,Y, Width ,Heigth);


                if (spriteRect.IntersectsWith(obstacleRect))
                {
                    return true;
                }
            
            return false;
        }

        public bool EnemyInObstacle(double x, double y, double height, double width)
        {
            Rect obstacleRect = new Rect(X, Y, Width, Heigth);
            Rect EnemyRect = new Rect(x, y, height, width);

            if (EnemyRect.IntersectsWith(obstacleRect))
            {
                return true;
            }

            return false;
        }
    }

}