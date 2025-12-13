using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace shooter
{
    public class MapLayout
    {
        private int n;
        public List<Obstacles> obstacles = new List<Obstacles>();
        public int N
        {
            get
            {
                return this.n;
            }

            set
            {
                this.n = value;
            }
        }


        public MapLayout(int n, Canvas canva)
        {
            this.N = n;
            Layout(n, canva);
        }

        public void Layout(int n, Canvas canva)
        {
            double h = canva.ActualHeight;
            double w = canva.ActualWidth;
            double size1 = 0.2;
            double size2 = 0.3;
            switch (n)
            {
                case 1: // 4 Corners (We gonna improve itccuz it's shi)
                    AddObstacle(canva, 0, 0, h * size1, w * size2, ObstacleType.Wall);
                    AddObstacle(canva, canva.ActualWidth - w * size2, 0, h * size1, w * size2, ObstacleType.Wall); 
                    AddObstacle(canva, 0, canva.ActualHeight - h * size1, h * size1, w * size2, ObstacleType.Wall); 
                    AddObstacle(canva, canva.ActualWidth - w * size1, canva.ActualHeight - h * size1, h * size1, w * size2, ObstacleType.Wall); 
                    break;
                case 2: //4 Corners and a centered block (We gonna improve it cuz it's shi)
                    AddObstacle(canva, 0, 0, h * size1, w * size2, ObstacleType.Wall);
                    AddObstacle(canva, canva.ActualWidth - w * size2, 0, h * size1, w * size2, ObstacleType.Wall);
                    AddObstacle(canva, 0, canva.ActualHeight - h * size1, h * size1, w * size2, ObstacleType.Wall);
                    AddObstacle(canva, canva.ActualWidth - w * size2, canva.ActualHeight - h * size1, h * size1, w * size2, ObstacleType.Wall);
                    AddObstacle(canva, (canva.ActualWidth - (w * size2)) / 2, (canva.ActualHeight - (h * size1)) / 2, h * size1, w * size2, ObstacleType.Wall);
                    AddObstacle(canva, 300, (canva.ActualHeight - (h * size1)) / 2, h * 0.1, w * 0.1, ObstacleType.Start);


                    break;
                case 3: //Cross (We gonna improve it cuz it's shi)
                    //ver
                    AddObstacle(canva, (w - w * 0.1) / 2, (h - h * 0.75) / 2, h * 0.75,  w * 0.1, ObstacleType.Wall);
                    //hor
                    AddObstacle(canva, (w - w * 0.75) / 2, (h - w * 0.1) / 2, w * 0.1, w * 0.75, ObstacleType.Wall);
                    break;
                case 4: //idk for now

                    break;
                case 5: //idk for now
                    AddObstacle(canva, 0, 0, canva.ActualHeight * 0.3, canva.ActualWidth * 0.4, ObstacleType.Wall);
                    break;
            }
            
        }

        public void AddObstacle(Canvas canva, double x, double y, double height, double width, ObstacleType type)
        {
            Obstacles  ob = new Obstacles(x, y, height, width, type);
            ob.AddToCanva(canva);

            obstacles.Add(ob);
        }

        
    }

}