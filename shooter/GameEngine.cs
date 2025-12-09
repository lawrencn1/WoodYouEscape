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
        public Enemy enemy;
        public InputManager inputMng;
        public Player joueur;

        private Stopwatch _stopwatch;
        private long _lastTick;

        private List<PlayerProjectile> playerProjectiles = new List<PlayerProjectile>();
        private List<EnemyProjectile> enemyProjectiles = new List<EnemyProjectile>();

        private double _fireTimerPlayer = 0;
        private double _fireTimerEnemy = 0;

        private double _currentCooldownDuration = 0.15;
        private const double ENEMY_COOLDOWN_DURATION = 1.0;

        private ProjectileTypePlayer _currentWeapon = ProjectileTypePlayer.Standard;

        static readonly double distance_coef = 1.2;

        public GameEngine(Canvas canvas)
        {
            inputMng = new InputManager();
            
            joueur = new Player(100, 100, 350);
            canvas.Children.Add(joueur.Sprite); 
            joueur.UpdatePosition();


            //enemy = new Enemy(100, 100, 200);
            //canvas.Children.Add(enemy.Sprite);
            //enemy.UpdatePosition();

            _stopwatch = new Stopwatch();
   
        }
        public void Start()
        {
            _stopwatch.Start();
            _lastTick = _stopwatch.ElapsedTicks;
            CompositionTarget.Rendering += GameLoop;
        }
        public void Stop()
        {
            CompositionTarget.Rendering -= GameLoop;
            _stopwatch.Stop();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            long currentTick = _stopwatch.ElapsedTicks;
            double deltaTime = (double)(currentTick - _lastTick) / Stopwatch.Frequency;
            _lastTick = currentTick;

            UpdatePlayer(deltaTime);
            UpdateBullets(deltaTime, mainWindow.canvas);
            //UpdateEnemy(deltaTime);
             
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
            Console.WriteLine(dx + " " + dy);

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

        public void UpdateEnemy(double deltaTime)
        {
            double targetDx, targetDy;

            //Enemy movement for following the player
            if ((distance_coef * joueur.X) - enemy.X < 2 && (distance_coef * joueur.X) - enemy.X > -2) 
                targetDx = 0;
            else 
                targetDx = (distance_coef * joueur.X) - enemy.X;

            if ((distance_coef * joueur.Y) - enemy.Y < 2 && (distance_coef * joueur.Y) - enemy.Y > - 2) 
                targetDy = 0;
            else
                targetDy = (distance_coef * joueur.Y) - enemy.Y;
            
            Console.WriteLine($"{targetDx} {targetDy}");

            //Diagonal movement normalization
            double length = Math.Sqrt(targetDx * targetDx + targetDy * targetDy);
            if (length > 0)
            {
                targetDx /= length;
                targetDy /= length;
            }
            enemy.Deplacement(targetDx, targetDy, deltaTime);

            if (_fireTimerEnemy > 0) _fireTimerEnemy -= deltaTime;

            if (_fireTimerEnemy <= 0)
            {
                SpawnBullet(mainWindow.canvas, "Enemy");
                _fireTimerEnemy = _currentCooldownDuration;
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
                    _currentCooldownDuration = 0.25;

                    break;
            }
        }

        private void SpawnBullet(Canvas canvas, String Sprite)
        {
            if (Sprite == "Player")
            {
                double player_startX = joueur.X + 20;
                double player_startY = joueur.Y + 20;

                Point target = inputMng.MousePosition;
                double diffX = target.X - player_startX;
                double diffY = target.Y - player_startY;
                double length = Math.Sqrt(diffX * diffX + diffY * diffY);

                double dirX = 0, dirY = -1;
                if (length > 0) { dirX = diffX / length; dirY = diffY / length; }

                PlayerProjectile newBullet = new PlayerProjectile(player_startX - 5, player_startY - 5, dirX, dirY, _currentWeapon);
                playerProjectiles.Add(newBullet);
                canvas.Children.Add(newBullet.Sprite);
            }
            else if (Sprite == "Enemy" && enemy != null)
            {
                double enemy_startX = enemy.X + 20;
                double enemy_startY = enemy.Y + 20;

                // Aim at player
                double diffX = joueur.X - enemy_startX;
                double diffY = joueur.Y - enemy_startY;
                double length = Math.Sqrt(diffX * diffX + diffY * diffY);

                double dirX = 0, dirY = 0;
                if (length > 0) { dirX = diffX / length; dirY = diffY / length; }

                EnemyProjectile newEnemyBullet = new EnemyProjectile(enemy_startX - 5, enemy_startY - 5, dirX, dirY);
                enemyProjectiles.Add(newEnemyBullet);
                canvas.Children.Add(newEnemyBullet.Sprite);
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
            // Enemy projectiles
            for (int j = enemyProjectiles.Count - 1; j >= 0; j--)
            {
                enemyProjectiles[j].Update(deltaTime);

                if (enemyProjectiles[j].IsMarkedForRemoval)
                {
                    _canvas.Children.Remove(enemyProjectiles[j].Sprite);
                    enemyProjectiles.RemoveAt(j);
                }
            }
        }
    }
}
