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

        private List<PlayerProjectile> bullets = new List<PlayerProjectile>();
        private int fireCooldownPlayer = 0;
        private int fireCooldownEnemy = 0;
        private double _currentCooldownDuration = 0.15; 
        private ProjectileTypePlayer _currentWeapon = ProjectileTypePlayer.Standard;

        static readonly double distance_coef = 1.2;

        public GameEngine(Canvas canvas)
        {
            inputMng = new InputManager();
            joueur = new Player(100, 100, 350);
            enemy = new Enemy(100, 100, 200);

            canvas.Children.Add(joueur.Sprite); 
            joueur.UpdatePosition();

            canvas.Children.Add(enemy.Sprite);
            joueur.UpdatePosition();

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
            UpdateEnemy(deltaTime);
             
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

            if (fireCooldownPlayer > 0) fireCooldownPlayer--;

            if (inputMng.IsShootPressed && fireCooldownPlayer <= 0)
            {
                SpawnBullet(mainWindow.canvas, "Player");
                Console.WriteLine("hehe");
                fireCooldownPlayer = 20;
            }
        }

        public void UpdateEnemy(double deltaTime)
        {
            double targetDx, targetDy;

            if ((distance_coef * joueur.X) - enemy.X < 2 && (distance_coef * joueur.X) - enemy.X > -2) 
                targetDx = 0;
            else 
                targetDx = (distance_coef * joueur.X) - enemy.X;

            if ((distance_coef * joueur.Y) - enemy.Y < 2 && (distance_coef * joueur.Y) - enemy.Y > - 2) 
                targetDy = 0;
            else
                targetDy = (distance_coef * joueur.Y) - enemy.Y;
            
            Console.WriteLine($"{targetDx} {targetDy}");


            double length = Math.Sqrt(targetDx * targetDx + targetDy * targetDy);
            if (length > 0)
            {
                targetDx /= length;
                targetDy /= length;
            }
            enemy.Deplacement(targetDx, targetDy, deltaTime);

            if (fireCooldownEnemy > 0) fireCooldownEnemy--;

            if (fireCooldownEnemy <= 0)
            {
                SpawnBullet(mainWindow.canvas, "Enemy");
                Console.WriteLine("hehe");
                fireCooldownEnemy = 20;
            }
        }
        private void SetWeapon(ProjectileTypePlayer type)
        {
            _currentWeapon = type;

            // Optional: Adjust fire rate based on weapon
            switch (type)
            {
                case ProjectileTypePlayer.MachineGun:
                    _currentCooldownDuration = 20; // Fast fire
                    break;
                case ProjectileTypePlayer.Sniper:
                    _currentCooldownDuration = 0.5; // Slow fire
                    break;
                case ProjectileTypePlayer.Rocket:
                    _currentCooldownDuration = 0.5;
                    fireCooldownPlayer = 40;// Medium fire
                    break;
                case ProjectileTypePlayer.Standard:
                default:
                    _currentCooldownDuration = 0.5;
                    fireCooldownPlayer = 20;
                    break;
            }
        }

        private void SpawnBullet(Canvas canvas, String Sprite)
        {
            double player_startX = joueur.X + 20;
            double player_startY = joueur.Y + 20;

            double enemy_startX = enemy.X + 20;
            double enemy_startY = enemy.Y + 20;

            if (Sprite == "Player")
            {
                Point target = inputMng.MousePosition;

                double diffX = target.X - player_startX;
                double diffY = target.Y - player_startY;

                double length = Math.Sqrt(diffX * diffX + diffY * diffY);

                double dirX = 0;
                double dirY = -1;

                if (length > 0)
                {
                    dirX = diffX / length;
                    dirY = diffY / length;
                }

                PlayerProjectile newBullet = new PlayerProjectile(player_startX - 5, player_startY - 5, dirX, dirY, _currentWeapon);

                bullets.Add(newBullet);
                canvas.Children.Add(newBullet.Sprite);
            }

            else
            {
                Point target = new Point(player_startX,player_startY);

                double diffX = target.X - enemy_startX;
                double diffY = target.Y - enemy_startY;

                double length = Math.Sqrt(diffX * diffX + diffY * diffY);

                double dirX = 0;
                double dirY = -1;

                if (length > 0)
                {
                    dirX = diffX / length;
                    dirY = diffY / length;
                }

                PlayerProjectile newBullet = new PlayerProjectile(enemy_startX - 5, enemy_startY - 5, dirX, dirY);

                bullets.Add(newBullet);
                canvas.Children.Add(newBullet.Sprite);
            }
        }

        private void UpdateBullets(double deltaTime, Canvas _canvas)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(deltaTime);

                if (bullets[i].IsMarkedForRemoval)
                {
                    _canvas.Children.Remove(bullets[i].Sprite);
                    bullets.RemoveAt(i);
                }
            }
        }
    }
}
