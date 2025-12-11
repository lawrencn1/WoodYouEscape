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
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public List<EnemyProjectile> enemyProjectiles = new List<EnemyProjectile>();

        private double x;
        private double y;
        private double vitesse;
        private int pv;
        private double distance;

        private double _fireTimerEnemy = 0;
        private const double ENEMY_COOLDOWN_DURATION = 1.0;

        public Player player;

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
                    //throw new ArgumentOutOfRangeException("Les PV doivent être positifs");
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

        public double Distance
        {
            get
            {
                return this.distance;
            }

            set
            {
                this.distance = value;
            }
        }

        public Enemy(double x, double y, double vitesse, double distance)
        {
            this.X = x;
            this.Y = y;
            this.Vitesse = vitesse;
            this.Distance = distance;
            this.Pv = 25;

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
        public void Damage(int qte)
        {
            Pv -= qte;
            if (Pv <= 0)
            {
            
            }
        }

        public void UpdateEnemy(double deltaTime, Player player, List<EnemyProjectile> globalBulletList, Canvas canvas)
        {
            double targetDx, targetDy;


            double diffX = (Distance * player.X) - X; 
            double diffY = (Distance * player.Y) - Y;

            if (Math.Abs(diffX) < 2) targetDx = 0;
            else targetDx = diffX;

            if (Math.Abs(diffY) < 2) targetDy = 0;
            else targetDy = diffY;

            // Normalize diagonal movement
            double length = Math.Sqrt(targetDx * targetDx + targetDy * targetDy);
            if (length > 0)
            {
                targetDx /= length;
                targetDy /= length;
            }
            Deplacement(targetDx, targetDy, deltaTime);

            // Shooting Logic
            if (_fireTimerEnemy > 0) _fireTimerEnemy -= deltaTime;

            if (_fireTimerEnemy <= 0)
            {

                SpawnBullet(canvas, player, globalBulletList);
                _fireTimerEnemy = ENEMY_COOLDOWN_DURATION;
            }
        }

        private void SpawnBullet(Canvas canvas, String Sprite, Player player)
        {
            double enemy_startX = X + 20;
            double enemy_startY = Y + 20;

            // Aim at player
            double diffX = player.X - enemy_startX;
            double diffY = player.Y - enemy_startY;
            double length = Math.Sqrt(diffX * diffX + diffY * diffY);

            double dirX = 0, dirY = 0;
            if (length > 0) { dirX = diffX / length; dirY = diffY / length; }

            EnemyProjectile newEnemyBullet = new EnemyProjectile(enemy_startX - 5, enemy_startY - 5, dirX, dirY);
            enemyProjectiles.Add(newEnemyBullet);
            canvas.Children.Add(newEnemyBullet.Sprite);
        }

        private void SpawnBullet(Canvas canvas, Player player, List<EnemyProjectile> globalBulletList)
        {
            double enemy_startX = X + 20;
            double enemy_startY = Y + 20;

            // Calculate direction towards player
            double diffX = player.X - enemy_startX;
            double diffY = player.Y - enemy_startY;
            double length = Math.Sqrt(diffX * diffX + diffY * diffY);

            double dirX = 0, dirY = 0;
            if (length > 0)
            {
                dirX = diffX / length;
                dirY = diffY / length;
            }

            // Create Bullet
            EnemyProjectile newEnemyBullet = new EnemyProjectile(enemy_startX - 5, enemy_startY - 5, dirX, dirY);
            globalBulletList.Add(newEnemyBullet);
            canvas.Children.Add(newEnemyBullet.Sprite);
        }
    }
}
