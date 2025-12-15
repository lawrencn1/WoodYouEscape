using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private double height;
        private double width;
        public EnemyType Type { get; private set; }

        private double _fireTimerEnemy = 0;
        private const double ENEMY_COOLDOWN_DURATION = 1.0;

        public Player player;

        public FrameworkElement Sprite;

        //Burn Status
        private bool _isBurning = false;
        private double _burnDurationTimer = 0;
        private double _burnTickTimer = 0;
        private const double BURN_TICK_RATE = 0.5; 
        private const int BURN_DAMAGE = 10;

        private ScaleTransform _flipTransform;

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

        public double Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;
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

        public Enemy(double x, double y, EnemyType type, double heigth, double width)
        {
            this.X = x;
            this.Y = y;
            this.Type = type;
            this.Pv = 25;
            this.Height = heigth;
            this.Width = width;

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
                    Width *= 1.5;
                    Height *= 1.5;
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
            _flipTransform = new ScaleTransform();

            // 1. Determine which texture to use based on Enemy Type
            ImageSource targetTexture = null;

            switch (Type)
            {
                case EnemyType.MeleeTank:
                    targetTexture = TextureManager.TankTexture;
                    break;
                case EnemyType.Ranged:
                    targetTexture = TextureManager.RangedTexture;
                    break;
                case EnemyType.MeleeBasic:
                default:
                    targetTexture = TextureManager.MeleeTexture;
                    break;
            }

            // 2. Create the Visual (Image or Fallback Rectangle)
            if (targetTexture != null)
            {
                // TEXTURE FOUND: Create an Image
                var img = new Image
                {
                    Width = this.Width,
                    Height = this.Height,
                    Stretch = Stretch.Uniform,
                    Source = targetTexture, // <--- THIS WAS MISSING IN YOUR CODE
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = _flipTransform
                };
                Sprite = img;
            }
            else
            {
                // TEXTURE MISSING: Create a Colored Rectangle (Fallback)
                // This ensures you see the enemy even if the file path is wrong
                var fallbackRect = new Rectangle
                {
                    Width = this.Width,
                    Height = this.Height,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = _flipTransform
                };

                // Color coding
                switch (Type)
                {
                    case EnemyType.MeleeTank: fallbackRect.Fill = Brushes.DarkRed; break;
                    case EnemyType.Ranged: fallbackRect.Fill = Brushes.Orange; break;
                    default: fallbackRect.Fill = Brushes.Red; break;
                }

                Sprite = fallbackRect;
            }

            // 3. Set Initial Position
            UpdatePosition();
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
        public void ApplyBurn(double duration)
        {
            _isBurning = true;
            _burnDurationTimer = duration;
            _burnTickTimer = 0;

            Sprite.Opacity = 0.5;
        }

       


        public void UpdateEnemy(double deltaTime, Player player, List<EnemyProjectile> globalBulletList, Canvas canvas, List<Obstacles> obstacles, List<Enemy>Enemies)
        {
            double margin = 5;

           
            double diffX = player.X - this.X;
            double diffY = player.Y - this.Y;
            double distanceToPlayer = Math.Sqrt(diffX * diffX + diffY * diffY);

            
            double dirX = 0;
            double dirY = 0;


            if (distanceToPlayer > Distance)
            {
                dirX = diffX / distanceToPlayer;
                dirY = diffY / distanceToPlayer;

                double pixelDist = Vitesse * deltaTime;

                if (dirX < 0)
                {
                    _flipTransform.ScaleX = -1; // Face Left
                }
                else
                {
                    _flipTransform.ScaleX = 1;  // Face Right
                }
                //enemies repulsion
                double radius = 50; 

                for (int i = 0; i < Enemies.Count; i++)
                {
                    
                    if (Enemies[i] == this)
                        continue;

                    double distX = this.X - Enemies[i].X;
                    double distY = this.Y - Enemies[i].Y;
                    double distsq = (distX * distX) + (distY * distY);

                    
                    if (distsq < radius * radius && distsq > 0)
                    {
                        double dist = Math.Sqrt(distsq);

                        
                        double repX = distX / dist;
                        double repY = distY / dist;

                        dirX += repX;
                        dirY += repY;
                    }
                }

                //obstacle collisions detection
                Rect futureX = new Rect(X + (dirX * pixelDist), Y + margin,80 ,100 - (margin * 2));

                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (obstacles[i].ObstacleCollision(futureX))
                    {
                        dirX = 0; 
                        break;    
                    }
                }
                
                Rect futureY = new Rect(X + margin, Y + (dirY * pixelDist), 80 - (margin * 2), 100);

                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (obstacles[i].ObstacleCollision(futureY))
                    {
                        dirY = 0; 
                        break;
                    }
                }
            }


            // --- 2. APPLY MOVEMENT (ONCE ONLY) ---
            double moveX = dirX * Vitesse * deltaTime;
            double moveY = dirY * Vitesse * deltaTime;


            double newX = X + moveX;
            double newY = Y + moveY;

            // --- 3. CLAMP TO SCREEN (Keep inside play area) ---
            double spriteWidth = ((FrameworkElement)Sprite).Width;
            double spriteHeight = ((FrameworkElement)Sprite).Height; 

            if (newX < GameEngine.PlayableArea.Left)
                newX = GameEngine.PlayableArea.Left;
            if (newX > GameEngine.PlayableArea.Right - spriteWidth) 
                newX = GameEngine.PlayableArea.Right - spriteWidth;

            if (newY < GameEngine.PlayableArea.Top) 
                newY = GameEngine.PlayableArea.Top;
            if (newY > GameEngine.PlayableArea.Bottom - spriteHeight) 
                newY = GameEngine.PlayableArea.Bottom - spriteHeight;

            // --- 4. UPDATE POSITION ---
            X = newX;
            Y = newY;
            UpdatePosition();

            // --- 5. SHOOTING LOGIC ---
            if (Type == EnemyType.Ranged)
            {
                if (_fireTimerEnemy > 0) _fireTimerEnemy -= deltaTime;

                if (_fireTimerEnemy <= 0)
                {
                    // Only fire if we are actually close enough 
                    if (distanceToPlayer <= 600) // Example range
                    {
                        SpawnBullet(canvas, player, globalBulletList);
                        _fireTimerEnemy = ENEMY_COOLDOWN_DURATION;
                    }
                }
            }

            if(_isBurning)
            {
                _burnDurationTimer -= deltaTime;
                _burnTickTimer -= deltaTime;

                if (_burnTickTimer <= 0)
                {
                    Damage(BURN_DAMAGE);
                    _burnTickTimer = BURN_TICK_RATE;
                }

                if (_burnDurationTimer <= 0)
                {
                    _isBurning = false;
                    Sprite.Opacity = 1.0; // Reset Opacity to normal
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
