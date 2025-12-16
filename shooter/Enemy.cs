using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Policy;
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
        private bool _isTakingDamage = false;
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
        private const int BURN_DAMAGE = 8;
        private double _burnBlinkTimer = 0;
        private const double BURN_BLINK_SPEED = 0.1; 


        // Animation State
        private int _currentFrame = 0;
        private double _animTimer = 0;
        private const double FRAME_DURATION = 0.05;
        private ScaleTransform _flipTransform;
        private Image _bodyImage;      // The actual sprite we animate
        private System.Windows.Shapes.Rectangle _redOverlay; // The red filter

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
            ImageSource targetTexture = null;

            // 1. Determine which texture to use
            switch (Type)
            {
                case EnemyType.MeleeTank:
                    if (TextureManager.TankDownFrames != null && TextureManager.TankDownFrames.Length > 0)
                        targetTexture = TextureManager.TankDownFrames[0];
                    else
                        targetTexture = TextureManager.AxeTexture;
                    break;
                case EnemyType.Ranged:
                    // Use the first frame if available, otherwise fallback
                    if (TextureManager.RangedDownFrames != null && TextureManager.RangedDownFrames.Length > 0)
                        targetTexture = TextureManager.RangedDownFrames[0];
                    else
                        targetTexture = TextureManager.AxeTexture;
                    break;
                case EnemyType.MeleeBasic:
                default:
                    if (TextureManager.MeleeDownFrames != null && TextureManager.MeleeDownFrames.Length > 0)
                        targetTexture = TextureManager.MeleeDownFrames[0];
                    else
                        targetTexture = TextureManager.AxeTexture;
                    break;
            }

            // 2. Build the Visual Structure
            if (targetTexture != null)
            {
                // --- TEXTURE FOUND: Create the Flash-ready Grid ---

                // A. Create the Body Image
                _bodyImage = new Image
                {
                    Stretch = Stretch.Uniform,
                    Source = targetTexture,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = _flipTransform
                };

                // B. Create the Red Overlay (Hidden by default)
                _redOverlay = new System.Windows.Shapes.Rectangle
                {
                    Fill = Brushes.Red,  // You can also try Brushes.White for a classic arcade flash
                    Opacity = 0.7,       // 0.7 = Strong Flash, 0.4 = Weak Tint
                    Visibility = Visibility.Hidden,
                    Width = this.Width,  // Ensure sizes match exactly
                    Height = this.Height
                };

                var initialMask = new ImageBrush();
                initialMask.ImageSource = targetTexture;
                _redOverlay.OpacityMask = initialMask;
                // C. Combine them in a Grid
                var container = new Grid
                {
                    Width = this.Width,
                    Height = this.Height
                };

                container.Children.Add(_bodyImage);  // Layer 0
                container.Children.Add(_redOverlay); // Layer 1 (Top)

                Sprite = container;
            }
            else
            {
                // --- TEXTURE MISSING: Create Fallback Rectangle ---
                var fallbackRect = new System.Windows.Shapes.Rectangle
                {
                    Width = this.Width,
                    Height = this.Height,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = _flipTransform
                };

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
        public async void Damage(int amount)
        {
            Pv -= amount;
            if (Pv <= 0) { return; }
            if (_redOverlay != null)
            {
                if (_isTakingDamage) return;

                _isTakingDamage = true; // Locks the Burn Visuals out

                // 1. FORCE COLOR BACK TO RED (In case it was Orange)
                _redOverlay.Fill = Brushes.Red;
                _redOverlay.Visibility = Visibility.Visible;

                // 2. Wait
                await Task.Delay(100);

                // 3. Hide
                _redOverlay.Visibility = Visibility.Hidden;

                _isTakingDamage = false; // Re-enables the Burn Visuals
            }
        }
        public void ApplyBurn(double duration)
        {
            _isBurning = true;
            _burnDurationTimer = duration;
            _burnTickTimer = 0;

            Sprite.Opacity = 0.5;
        }


        public void UpdateEnemy(double deltaTime, Player player, List<EnemyProjectile> globalBulletList, Canvas canvas, List<Obstacles> obstacles, List<Enemy> Enemies)
        {

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
                Rect futureX = new Rect(X + (dirX * pixelDist), Y , 80, 100);

                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (obstacles[i].ObstacleCollision(futureX) && (obstacles[i].Type == ObstacleType.Wall || obstacles[i].Type == ObstacleType.Puddle))
                    {
                        dirX = 0;
                        break;
                    }
                }

                Rect futureY = new Rect(X, Y + (dirY * pixelDist), 80, 100);

                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (obstacles[i].ObstacleCollision(futureY) && (obstacles[i].Type == ObstacleType.Wall || obstacles[i].Type == ObstacleType.Puddle))
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


            BitmapSource[] useSideFrames;
            BitmapSource[] useUpFrames;
            BitmapSource[] useDownFrames;

            switch (Type)
            {
                case EnemyType.MeleeTank:
                    useSideFrames = TextureManager.TankSideFrames;
                    useUpFrames = TextureManager.TankUpFrames;
                    useDownFrames = TextureManager.TankDownFrames;
                    break;

                case EnemyType.Ranged:
                    useSideFrames = TextureManager.RangedSideFrames;
                    useUpFrames = TextureManager.RangedUpFrames;
                    useDownFrames = TextureManager.RangedDownFrames;
                    break;

                case EnemyType.MeleeBasic:
                default:
                    // Default to the standard Melee bush textures
                    useSideFrames = TextureManager.MeleeSideFrames;
                    useUpFrames = TextureManager.MeleeUpFrames;
                    useDownFrames = TextureManager.MeleeDownFrames;
                    break;
            }

            BitmapSource[] currentAnimSet = useDownFrames;

            if (Math.Abs(dirX) > Math.Abs(dirY))
            {
                // Moving Horizontally (Left or Right)
                // Use LeftFrames for both, but flip the sprite for Right
                currentAnimSet = useSideFrames;

                if (dirX > 0)
                {
                    _flipTransform.ScaleX = 1; // Face Right (Flip Left image)
                }
                else
                {
                    _flipTransform.ScaleX = -1;  // Face Left (Normal)
                }
            }
            else
            {
                // Moving Vertically (Up or Down)
                _flipTransform.ScaleX = 1; // Reset flip

                if (dirY > 0)
                {
                    currentAnimSet = useDownFrames;
                }
                else
                {
                    currentAnimSet = useUpFrames;
                }
            }

            // 2. Only Animate if actually moving
            if (Math.Abs(dirX) > 0.01 || Math.Abs(dirY) > 0.01)
            {
                Animate(currentAnimSet, deltaTime);
            }
            else
            {
                // Optional: If standing still, show Frame 0 (Idle)
                SetSprite(currentAnimSet, 0);
            }

            UpdateBurnVisuals(deltaTime);

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

            if (_isBurning)
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

        private void Animate(BitmapSource[] frames, double deltaTime)
        {
            if (frames == null || frames.Length == 0) return;

            _animTimer += deltaTime;

            if (_animTimer > FRAME_DURATION)
            {
                _animTimer = 0;
                _currentFrame++;

                // Loop: Reset to 1 (skipping 0 if 0 is the "Idle" static pose)
                // If you want to use the first frame too, change 1 to 0
                if (_currentFrame >= frames.Length)
                    _currentFrame = 1;

                SetSprite(frames, _currentFrame);
            }
        }
        private void SetSprite(BitmapSource[] frames, int index)
        {
            // 1. Safety Checks
            if (_bodyImage == null || frames == null || frames.Length == 0) return;
            if (index < 0 || index >= frames.Length) return;

            // 2. Get the current frame
            var currentFrame = frames[index];

            // 3. Update the visible Body
            _bodyImage.Source = currentFrame;

            // 4. Update the Red Overlay Mask 
            // This tells the red rectangle to only appear where the sprite pixels are
            if (_redOverlay != null)
            {
                var maskBrush = new ImageBrush();
                maskBrush.ImageSource = currentFrame;
                _redOverlay.OpacityMask = maskBrush;
            }
        }

        private void UpdateBurnVisuals(double deltaTime)
        {
            // 1. If we are currently taking "Hit Damage" (Red Flash), do not interfere!
            // The Damage() method has priority.
            if (_isTakingDamage) return;

            // 2. Are we Burning?
            if (_isBurning)
            {
                // Set color to Orange
                _redOverlay.Fill = Brushes.Orange;

                // Run the Blink Timer
                _burnBlinkTimer += deltaTime;

                if (_burnBlinkTimer >= BURN_BLINK_SPEED)
                {
                    _burnBlinkTimer = 0; // Reset timer

                    // Toggle Visibility (If Visible -> Hidden, If Hidden -> Visible)
                    if (_redOverlay.Visibility == Visibility.Visible)
                        _redOverlay.Visibility = Visibility.Hidden;
                    else
                        _redOverlay.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // 3. Not Burning and Not Taking Damage? Ensure overlay is off.
                _redOverlay.Visibility = Visibility.Hidden;
            }
        }
    }
}
