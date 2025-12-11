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
    public enum EnemyType
    {
        MeleeBasic,
        MeleeTank,
        Ranged
    }
    public class Enemy
    {
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public List<EnemyProjectile> enemyProjectiles = new List<EnemyProjectile>();

        private double x;
        private double y;
        private double vitesse;
        private int pv;
        private double distance;

        public EnemyType Type { get; private set; }

        private double _fireTimerEnemy = 0;
        private const double ENEMY_COOLDOWN_DURATION = 1.0;

        public Player player;

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

        public Enemy(double x, double y, EnemyType type)
        {
            this.X = x;
            this.Y = y;
            this.Type = type;
            this.Pv = 25;

            // 2. Initialize Stats based on Type
            InitializeStats();

            // 3. Initialize Visuals (Sprite + Hitbox)
            InitializeVisuals();
        }
        private void InitializeStats()
        {
            switch (Type)
            {
                case EnemyType.MeleeBasic:
                    Vitesse = 200;
                    Pv = 30;
                    Distance = 30; // Chases until collision
                    break;

                case EnemyType.MeleeTank:
                    Vitesse = 100; // Slower
                    Pv = 80;       // Harder to kill
                    Distance = 40;
                    break;

                case EnemyType.Ranged:
                    Vitesse = 180;
                    Pv = 15;       // Weak
                    Distance = 300; // Stops 300px away to shoot
                    _fireTimerEnemy = 1.5;
                    break;
            }     

        }
        private void InitializeVisuals()
        {
            double size = (Type == EnemyType.MeleeTank) ? 60 : 40; // Tanks are bigger
            Brush color = Brushes.Red;

            if (Type == EnemyType.MeleeTank) color = Brushes.DarkRed;
            if (Type == EnemyType.Ranged) color = Brushes.Orange;

            // Create Sprite
            Sprite = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = color
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
            // --- 1. Calculate Direction & Intended Movement ---
            double diffX = player.X - this.X;
            double diffY = player.Y - this.Y;
            double distanceToPlayer = Math.Sqrt(diffX * diffX + diffY * diffY);

            // Default movement is 0 (if we are stopped)
            double moveX = 0;
            double moveY = 0;

            // Only calculate movement if we are outside the stop distance
            if (distanceToPlayer > Distance)
            {
                double dirX = diffX / distanceToPlayer;
                double dirY = diffY / distanceToPlayer;

                moveX = dirX * Vitesse * deltaTime;
                moveY = dirY * Vitesse * deltaTime;
            }

            // --- 2. Calculate Potential New Position ---
            double newX = X + moveX;
            double newY = Y + moveY;

            // --- 3. Clamp to Playable Area ---
            // Ensure GameEngine.PlayableArea is defined in your GameEngine class!
            double spriteSize = ((FrameworkElement)Sprite).Width;

            // Left/Right Walls
            if (newX < GameEngine.PlayableArea.Left)
                newX = GameEngine.PlayableArea.Left;
            if (newX > GameEngine.PlayableArea.Right - spriteSize)
                newX = GameEngine.PlayableArea.Right - spriteSize;

            // Top/Bottom Walls
            if (newY < GameEngine.PlayableArea.Top)
                newY = GameEngine.PlayableArea.Top;
            if (newY > GameEngine.PlayableArea.Bottom - spriteSize)
                newY = GameEngine.PlayableArea.Bottom - spriteSize;

            // --- 4. Apply Final Position ---
            X = newX;
            Y = newY;

            UpdatePosition();

            // --- 5. Shooting Logic ---
            if (Type == EnemyType.Ranged)
            {
                if (_fireTimerEnemy > 0) _fireTimerEnemy -= deltaTime;

                if (_fireTimerEnemy <= 0)
                {
                    SpawnBullet(canvas, player, globalBulletList);
                    _fireTimerEnemy = ENEMY_COOLDOWN_DURATION; // Reset timer
                }
            }
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
