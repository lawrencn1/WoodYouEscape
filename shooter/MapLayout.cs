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
    public class MapLayout
    {
        private int n;
        public Obstacles obstacles;
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


        public MapLayout(int n)
        {
            this.N = n;
        }

        public void Layout(int n, Canvas canva)
        {
        }

        public void AddObstacle(Canvas canva)
        {

        }
    }

}