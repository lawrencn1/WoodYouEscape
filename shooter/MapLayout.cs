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
            switch (n)
            {
                case 1: // 4 Corners
                    AddObstacle(canva, 0, 0, h * 0.2, w * 0.3);
                    AddObstacle(canva, canva.ActualWidth - w * 0.3, 0, h * 0.2, w * 0.3); 
                    AddObstacle(canva, 0, canva.ActualHeight - h * 0.2, h * 0.2, w * 0.3); 
                    AddObstacle(canva, canva.ActualWidth - w * 0.3, canva.ActualHeight - h * 0.2, h * 0.2, w * 0.3); 
                    break;
                case 2: //4 Corners and a centered block
                    AddObstacle(canva, 0, 0, h * 0.2, w * 0.3);
                    AddObstacle(canva, canva.ActualWidth - w * 0.3, 0, h * 0.2, w * 0.3);
                    AddObstacle(canva, 0, canva.ActualHeight - h * 0.2, h * 0.2, w * 0.3);
                    AddObstacle(canva, canva.ActualWidth - w * 0.3, canva.ActualHeight - h * 0.2, h * 0.2, w * 0.3);
                    AddObstacle(canva, (canva.ActualWidth - (w * 0.3)) / 2, (canva.ActualHeight - (h * 0.2)) / 2, h * 0.2, w * 0.3);

                    break;
                case 3: //idk for now
                    AddObstacle(canva, 0, 0, canva.ActualHeight * 0.3, canva.ActualWidth * 0.4);
                    break;
                case 4: //idk for now
                    AddObstacle(canva, 0, 0, canva.ActualHeight * 0.3, canva.ActualWidth * 0.4);
                    break;
                case 5: //idk for now
                    AddObstacle(canva, 0, 0, canva.ActualHeight * 0.3, canva.ActualWidth * 0.4);
                    break;
            }
            
        }

        public void AddObstacle(Canvas canva, double x, double y, double height, double width)
        {
            Obstacles  ob = new Obstacles(x, y, height, width);
            ob.AddToCanva(canva);

            obstacles.Add(ob);
        }

        
    }

}