using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace shooter
{
    public class MapLayout 
    {
        public UCLayout1 layout1 = new UCLayout1();
        public UCLayout2 layout2 = new UCLayout2();
        public UCLayout3 layout3 = new UCLayout3();
        private int n;
        public List<Obstacles> obstacles = new List<Obstacles>();

        public UserControl layoutVisual;
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
            int count = 1;
            double coef = canva.ActualHeight / layout1.grid.Height;
            double coef2 = canva.ActualWidth / layout1.grid.Width;
            double playableX = 100;
            double playableY = 160;
            FrameworkElement layoutSource = null;
            
            switch (n)
            {
                case 1:
                    layoutVisual = layout1;
                    break;
                case 2:
                    layoutVisual = layout2;
                    break;
                case 3:
                    layoutVisual = layout3;
                    break;
                case 4:
                    layoutVisual = layout1; // Placeholder
                    break;
                case 5:
                    layoutVisual = layout1; // Placeholder
                    break;
                default:
                    layoutVisual = layout1;
                    break;
            }

            switch (n)
            {
                case 1:
                    
                    while (true)
                    {
                        var control = (FrameworkElement)layout1.FindName($"a{count}");
                        if (control == null)
                            break;
                        double height = control.Height * coef;
                        double width = control.Width * coef2;
                        double X = control.Margin.Left * coef2;
                        double Y = control.Margin.Top * coef;
                        if ((String)control.Tag == "Start")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.Start);
                        else if ((String)control.Tag == "End")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.End);
                        else
                            AddObstacle(canva, X + playableX , Y + playableY, height, width, ObstacleType.Wall);

                        count++;
                    }

                    break;
                case 2: //4 Corners and a centered block (We gonna improve it cuz it's shi)

                    while (true)
                    {
                        var control = (FrameworkElement)layout2.FindName($"a{count}");
                        if (control == null)
                            break;
                        double height = control.Height * coef;
                        double width = control.Width * coef2;
                        double X = control.Margin.Left * coef2;
                        double Y = control.Margin.Top * coef;
                        if ((String)control.Tag == "Start")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.Start);
                        else if ((String)control.Tag == "End")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.End);
                        else
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.Wall);

                        count++;
                    }

                    break;
                case 3: //Cross (We gonna improve it cuz it's shi)
                    while (true)
                    {
                        var control = (FrameworkElement)layout3.FindName($"a{count}");
                        if (control == null)
                            break;
                        double height = control.Height * coef;
                        double width = control.Width * coef2;
                        double X = control.Margin.Left * coef2;
                        double Y = control.Margin.Top * coef;
                        if ((String)control.Tag == "Start")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.Start);
                        else if ((String)control.Tag == "End")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.End);
                        else
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.Wall);

                        count++;
                    }
                    break;
                case 4: //idk for now
                    while (true)
                    {
                        var control = (FrameworkElement)layout1.FindName($"a{count}");
                        if (control == null)
                            break;
                        double height = control.Height * coef;
                        double width = control.Width * coef2;
                        double X = control.Margin.Left * coef2;
                        double Y = control.Margin.Top * coef;
                        if ((String)control.Tag == "Start")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.Start);
                        else if ((String)control.Tag == "End")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.End);
                        else
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.Wall);

                        count++;
                    }
                    break;
                case 5: //idk for now

                    while (true)
                    {
                        var control = (FrameworkElement)layout1.FindName($"a{count}");
                        if (control == null)
                            break;
                        double height = control.Height * coef;
                        double width = control.Width * coef2;
                        double X = control.Margin.Left * coef2;
                        double Y = control.Margin.Top * coef;
                        if ((String)control.Tag == "Start")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.Start);
                        else if ((String)control.Tag == "End")
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.End);
                        else
                            AddObstacle(canva, X + playableX, Y + playableY, height, width, ObstacleType.Wall);

                        count++;
                    }

                    break;
            }
            if (layoutVisual != null)
            {
                // Set the size to match your math coefficients
                layoutVisual.Width = layout1.grid.Width * coef2;
                layoutVisual.Height = layout1.grid.Height * coef;

                // Position it exactly where your math starts
                Canvas.SetLeft(layoutVisual, playableX);
                Canvas.SetTop(layoutVisual, playableY);

                // Add to Canvas (Background layer)
                if (!canva.Children.Contains(layoutVisual))
                {
                    canva.Children.Add(layoutVisual);
                    Canvas.SetZIndex(layoutVisual, -1); // Ensure it is behind the player
                }
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