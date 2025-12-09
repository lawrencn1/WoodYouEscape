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
        public Enemy mechant;
        public InputManager inputMng;
        public Player joueur;

        private Stopwatch _stopwatch;
        private long _lastTick;

        private List<PlayerProjectile> bullets = new List<PlayerProjectile>();
        private int fireCooldown = 0;
        private double _currentCooldownDuration = 0.15; 
        private ProjectileType _currentWeapon = ProjectileType.Standard;

        static readonly double distance_coef = 1.2;

        public GameEngine(Canvas canvas)
        {
            inputMng = new InputManager();
            joueur = new Player(100, 100, 350);
            mechant = new Enemy(100, 100, 200);

            canvas.Children.Add(joueur.Sprite); 
            joueur.UpdatePosition();

            canvas.Children.Add(mechant.Sprite);
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
            if (inputMng.IsKey1Pressed) SetWeapon(ProjectileType.Standard);
            if (inputMng.IsKey2Pressed) SetWeapon(ProjectileType.MachineGun);
            if (inputMng.IsKey3Pressed) SetWeapon(ProjectileType.Sniper);
            if (inputMng.IsKey4Pressed) SetWeapon(ProjectileType.Rocket);

            if (fireCooldown > 0) fireCooldown--;

            if (inputMng.IsShootPressed && fireCooldown <= 0)
            {
                SpawnBullet(mainWindow.canvas);
                Console.WriteLine("hehe");
                fireCooldown = 20;
            }
        }

        public void UpdateEnemy(double deltaTime)
        {
            double targetDx, targetDy;

            if ((distance_coef * joueur.X) - mechant.X < 2 && (distance_coef * joueur.X) - mechant.X > -2) 
                targetDx = 0;
            else 
                targetDx = (distance_coef * joueur.X) - mechant.X;

            if ((distance_coef * joueur.Y) - mechant.Y < 2 && (distance_coef * joueur.Y) - mechant.Y > - 2) 
                targetDy = 0;
            else
                targetDy = (distance_coef * joueur.Y) - mechant.Y;
            
            Console.WriteLine($"{targetDx} {targetDy}");


            double length = Math.Sqrt(targetDx * targetDx + targetDy * targetDy);
            if (length > 0)
            {
                targetDx /= length;
                targetDy /= length;
            }
            mechant.Deplacement(targetDx, targetDy, deltaTime);
        }
        private void SetWeapon(ProjectileType type)
        {
            _currentWeapon = type;

            // Optional: Adjust fire rate based on weapon
            switch (type)
            {
                case ProjectileType.MachineGun:
                    _currentCooldownDuration = 20; // Fast fire
                    break;
                case ProjectileType.Sniper:
                    _currentCooldownDuration = 0.5; // Slow fire
                    break;
                case ProjectileType.Rocket:
                    _currentCooldownDuration = 0.5;
                    fireCooldown = 40;// Medium fire
                    break;
                case ProjectileType.Standard:
                default:
                    _currentCooldownDuration = 0.5;
                    fireCooldown = 20;
                    break;
            }
        }

        private void SpawnBullet(Canvas canvas)
        {
            double startX = joueur.X + 20;
            double startY = joueur.Y + 20;

            Point target = inputMng.MousePosition;

            double diffX = target.X - startX;
            double diffY = target.Y - startY;

            double length = Math.Sqrt(diffX * diffX + diffY * diffY);

            double dirX = 0;
            double dirY = -1; 

            if (length > 0)
            {
                dirX = diffX / length;
                dirY = diffY / length;
            }

            PlayerProjectile newBullet = new PlayerProjectile(startX - 5, startY - 5, dirX, dirY, _currentWeapon);

            bullets.Add(newBullet);
            canvas.Children.Add(newBullet.Sprite);
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
