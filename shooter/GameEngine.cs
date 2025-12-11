using shooter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace shooter
{
    
    public class GameEngine
    {
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        private Canvas _gameCanvas;
        
        public InputManager inputMng;
        public Player joueur;


        private Stopwatch _stopwatch;
        private long _lastTick;

        private List<PlayerProjectile> playerProjectiles = new List<PlayerProjectile>();
        private List<EnemyProjectile> enemyProjectiles = new List<EnemyProjectile>();
        public List<Enemy> Enemies = new List<Enemy>();

        private double _fireTimerPlayer = 0;
        private double _fireTimerEnemy = 0;

        private double _currentCooldownDuration = 1;
        private const double ENEMY_COOLDOWN_DURATION = 1.0;

        private ProjectileTypePlayer _currentWeapon = ProjectileTypePlayer.Standard;

        static readonly double distance_coef = 1.2;

        public GameEngine(Canvas canvas)
        {
            _gameCanvas = canvas;

            inputMng = new InputManager();
            
            joueur = new Player(100, 100, 350);

            _stopwatch = new Stopwatch();

        }
        public void Start()
        {
            GameRule(_gameCanvas);
            
        }
        public void Stop()
        {
            CompositionTarget.Rendering -= GameLoop;
            _stopwatch.Stop();
        }

        private void BeginGameplay()
        {

            if (!_gameCanvas.Children.Contains(joueur.Sprite))
            {
                _gameCanvas.Children.Add(joueur.Sprite);
            }
            joueur.UpdatePosition();

            // 2. Spawn Enemies
            SpawnEnemies(_gameCanvas, 200, 200);
            SpawnEnemies(_gameCanvas, 400, 0);

            // 3. Start Game Loop
            _stopwatch.Start();
            _lastTick = _stopwatch.ElapsedTicks;
            CompositionTarget.Rendering += GameLoop;
        }


        private void GameLoop(object sender, EventArgs e)
        {

            long currentTick = _stopwatch.ElapsedTicks;
            double deltaTime = (double)(currentTick - _lastTick) / Stopwatch.Frequency;
            _lastTick = currentTick;

            UpdatePlayer(deltaTime);
            UpdateBullets(deltaTime, _gameCanvas);

            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].UpdateEnemy(deltaTime, joueur);
                Enemies[i].UpdateBullets(deltaTime, _gameCanvas);
            }

        }

        public void SpawnEnemies(Canvas canvas, double X, double Y)
        {
            Enemy enemy = new Enemy(X, Y, 200, 1.2);
            canvas.Children.Add(enemy.Sprite);
            enemy.UpdatePosition();

            Enemies.Add(enemy);
        }
        public void UpdatePlayer(double deltaTime)
        {
            double dx = 0;
            double dy = 0;

            if (inputMng.IsLeftPressed) dx -= 1;
            if (inputMng.IsRightPressed) dx += 1;
            if (inputMng.IsUpPressed) dy -= 1;
            if (inputMng.IsDownPressed) dy += 1;

            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length > 0)
            {
                dx /= length;
                dy /= length;
            }
            joueur.Deplacement(dx, dy, deltaTime);

            // Weapon Switching Logic
            if (inputMng.IsKey1Pressed) SetWeapon(ProjectileTypePlayer.Standard);
            if (inputMng.IsKey2Pressed) SetWeapon(ProjectileTypePlayer.MachineGun);
            if (inputMng.IsKey3Pressed) SetWeapon(ProjectileTypePlayer.Sniper);
            if (inputMng.IsKey4Pressed) SetWeapon(ProjectileTypePlayer.Rocket);

            if (_fireTimerPlayer > 0) _fireTimerPlayer -= deltaTime;

            if (inputMng.IsShootPressed && _fireTimerPlayer <= 0)
            {
                SpawnBullet(mainWindow.canvas, "Player");
                _fireTimerPlayer = _currentCooldownDuration;
            }
        }

        private void SetWeapon(ProjectileTypePlayer type)
        {
            _currentWeapon = type;

            // Optional: Adjust fire rate based on weapon
            switch (type)
            {
                case ProjectileTypePlayer.MachineGun:
                    _currentCooldownDuration = 0.15; // Fast fire
                    break;
                case ProjectileTypePlayer.Sniper:
                    _currentCooldownDuration = 1.2; // Slow fire
                    break;
                case ProjectileTypePlayer.Rocket:
                    _currentCooldownDuration = 1.5;
                    // Medium fire
                    break;
                case ProjectileTypePlayer.Standard:
                default:
                    _currentCooldownDuration = 1;

                    break;
            }
        }

        private void SpawnBullet(Canvas canvas, String Sprite)
        {
            if (Sprite == "Player")
            {
                double player_startX = joueur.X;
                double player_startY = joueur.Y;

                Point target = inputMng.MousePosition;
                double diffX = target.X - player_startX;
                double diffY = target.Y - player_startY;
                double length = Math.Sqrt(diffX * diffX + diffY * diffY);

                double dirX = 0, dirY = 0;
                if (length > 0) { 
                    dirX = diffX / length; 
                    dirY = diffY / length; 
                }

                PlayerProjectile newBullet = new PlayerProjectile(player_startX-10, player_startY-10, dirX, dirY, _currentWeapon);
                playerProjectiles.Add(newBullet);
                canvas.Children.Add(newBullet.Sprite);
            }
  
        }

        private void UpdateBullets(double deltaTime, Canvas _canvas)
        {
            //Player projectiles
            for (int i = playerProjectiles.Count - 1; i >= 0; i--)
            {
                playerProjectiles[i].Update(deltaTime);

                if (playerProjectiles[i].IsMarkedForRemoval)
                {
                    _canvas.Children.Remove(playerProjectiles[i].Sprite);
                    playerProjectiles.RemoveAt(i);
                }
            }
        }

        //User controls fonctions
        private void GameRule(Canvas canva)
        {   
            GameRules uc = new GameRules();

            uc.Width = SystemParameters.PrimaryScreenWidth;
            uc.Height = SystemParameters.PrimaryScreenHeight;

            canva.Children.Add(uc);

            uc.validate.Click += (sender, e) =>
            {
                canva.Children.Remove(uc);
                GameControl(canva);
            };
        }

        private void GameControl(Canvas canva)
        {
            UCGameControls uc = new UCGameControls();

            uc.Width = SystemParameters.PrimaryScreenWidth;
            uc.Height = SystemParameters.PrimaryScreenHeight;

            canva.Children.Add(uc);

            uc.validate.Click += (sender, e) =>
            {
                canva.Children.Remove(uc);
                GameMode(canva);
            };
        }

        private void GameMode(Canvas canva)
        {
            UCGameMode uc = new UCGameMode();

            uc.Width = SystemParameters.PrimaryScreenWidth;
            uc.Height = SystemParameters.PrimaryScreenHeight;

            canva.Children.Add(uc);

            uc.validate.Click += (sender, e) =>
            {
                canva.Children.Remove(uc);
                Difficulty(canva);
            };
        }

        private void Difficulty(Canvas canva)
        {
            UCDifficulty uc = new UCDifficulty();
            canva.Children.Add(uc);

            uc.Width = SystemParameters.PrimaryScreenWidth;
            uc.Height = SystemParameters.PrimaryScreenHeight;

            uc.play.Click += (sender, e) =>
            {
                canva.Children.Remove(uc);
                BeginGameplay();
            };
        }
    }
}
