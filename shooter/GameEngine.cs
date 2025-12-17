using shooter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace shooter
{
    
    public class GameEngine
    {
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        //Public
        public InputManager inputMng;
        public Player joueur;
        public List<EnemyProjectile> globalEnemyProjectiles = new List<EnemyProjectile>();
        public List<Enemy> Enemies = new List<Enemy>();
        public static Rect PlayableArea = new Rect(100,295,1080,1080);
        public EnemiesGenerator enemiesGenerator;
        public UCGUI UCGUI = new UCGUI();
        public UCSettings UCsettings = new UCSettings();
        //Private
        private double _fireTimerPlayer = 0;
        private double _currentCooldownDuration = 1;
        private Canvas _gameCanvas;
        private Stopwatch _stopwatch;
        private long _lastTick;
        private MapLayout _mapLayout;
        private List<PlayerProjectile> playerProjectiles = new List<PlayerProjectile>();
        private ProjectileTypePlayer _currentWeapon = ProjectileTypePlayer.Standard;
        private int _map;
        private Random _random = new Random();
        private int _mapNumber = 0;
        private int _mapMax;
        private int _score = 0;


        public GameEngine(Canvas canvas)
        {

            _gameCanvas = canvas;

            double nativeWidth = _gameCanvas.Width;   
            double nativeHeight = _gameCanvas.Height;

            TextureManager.LoadTextures();
            inputMng = new InputManager();

            PlayableArea = new Rect(100, 150, nativeWidth - 150 , nativeHeight - 260);

            _stopwatch = new Stopwatch();
 
        }

        public void Start()
        {
            GameRule(_gameCanvas);  
        }

        public void Pause()
        {
            CompositionTarget.Rendering -= GameLoop;
            _stopwatch.Stop(); 
        }

        public void Resume()
        {
            CompositionTarget.Rendering += GameLoop;

            _stopwatch.Restart();
            _lastTick = 0;
        }
        public void Stop()
        {
            
            CompositionTarget.Rendering -= GameLoop;
            _stopwatch.Stop();

            //Remove UC
            if (_gameCanvas.Children.Contains(UCGUI))
            {
                _gameCanvas.Children.Remove(UCGUI);
            }

            // 2. Remove Settings if they are open
            if (_gameCanvas.Children.Contains(UCsettings))
            {
                _gameCanvas.Children.Remove(UCsettings);
            }

            //Clears projectiles
            for (int i = 0; i < playerProjectiles.Count; i++)
            {
                _gameCanvas.Children.Remove(playerProjectiles[i].Sprite);
            }
            playerProjectiles.Clear();

            
            for (int i = 0;i < globalEnemyProjectiles.Count; i++)
            {
                _gameCanvas.Children.Remove(globalEnemyProjectiles[i].Sprite);
            }
            globalEnemyProjectiles.Clear();

            // Clears Enemies
            for (int i = 0; i < Enemies.Count; i++) 
            {
                _gameCanvas.Children.Remove(Enemies[i].Sprite);
            }
            Enemies.Clear();

            // Clears Layout UC
            if (_mapLayout != null && _mapLayout.layoutVisual != null)
            {
                _gameCanvas.Children.Remove(_mapLayout.layoutVisual);
            }

            //clears obstacles
            for (int i = 0; i < _mapLayout.obstacles.Count; i++)
            {
                _gameCanvas.Children.Remove(_mapLayout.obstacles[i].Sprite);
            }
            _mapLayout.obstacles.Clear();

            //clears player
            if (_gameCanvas.Children.Contains(joueur.Sprite))
            {
                _gameCanvas.Children.Remove(joueur.Sprite);
            }

            UCGUI = new UCGUI();
            UCsettings = new UCSettings();

            
        }
        private void BeginGameplay()
        {
            GUI(_gameCanvas, UCGUI);

            UCGUI.Weapon.Content = "Hache de bûcheron";

            SFXManager.PlayMusic();

            mapChange(_gameCanvas);

            for (int i = 0; i < _mapLayout.obstacles.Count; i++)
            {
                if (_mapLayout.obstacles[i].Type == ObstacleType.Start)
                {
                    joueur = new Player(_mapLayout.obstacles[i].X + _mapLayout.obstacles[i].X * 0.20, _mapLayout.obstacles[i].Y , 200);
                }
            }
            if (!_gameCanvas.Children.Contains(joueur.Sprite))
            {
                _gameCanvas.Children.Add(joueur.Sprite);
            }
            joueur.UpdatePosition();           

            // 3. Start Game Loop
            var border = new Rectangle
            {
                Width = PlayableArea.Width,
                Height = PlayableArea.Height,
                Stroke = Brushes.Yellow,
                StrokeThickness = 3
            };
            Canvas.SetLeft(border, PlayableArea.X);
            Canvas.SetTop(border, PlayableArea.Y);
            _gameCanvas.Children.Add(border);


            _stopwatch.Start();
            _lastTick = _stopwatch.ElapsedTicks;
            CompositionTarget.Rendering += GameLoop;

            //SPAWN
            if (MainWindow.DIFFICULTY == "easy")
            {
                EnemiesRandomizer(_gameCanvas, 1, EnemyType.MeleeBasic);
                EnemiesRandomizer(_gameCanvas, 1, EnemyType.Ranged);
                EnemiesRandomizer(_gameCanvas, 1, EnemyType.MeleeTank);
                _mapMax = 2;
            }

            else if (MainWindow.DIFFICULTY == "normal")
            {
                EnemiesRandomizer(_gameCanvas, 2, EnemyType.MeleeBasic);
                EnemiesRandomizer(_gameCanvas, 2, EnemyType.Ranged);
                EnemiesRandomizer(_gameCanvas, 1, EnemyType.MeleeTank);
                _mapMax = 3;
            }

            else
            {
                EnemiesRandomizer(_gameCanvas, 3, EnemyType.MeleeBasic);
                EnemiesRandomizer(_gameCanvas, 3, EnemyType.Ranged);
                EnemiesRandomizer(_gameCanvas, 1, EnemyType.MeleeTank);
                _mapMax = 4;
            }
 
        }

        private void GameLoop(object sender, EventArgs e)
        {
      
            if (joueur.Hp < 0)
            {
                Stop();
                Lose(_gameCanvas);
            }

            long currentTick = _stopwatch.ElapsedTicks;
            double deltaTime = (double)(currentTick - _lastTick) / Stopwatch.Frequency;
            _lastTick = currentTick;

            UpdatePlayer(deltaTime);
            joueur.UpdateInvincibility(deltaTime);
            UpdatePlayerBullets(deltaTime, _gameCanvas);
            UpdateEnemyBullets(deltaTime, _gameCanvas);

            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].UpdateEnemy(deltaTime, joueur, globalEnemyProjectiles, _gameCanvas, _mapLayout.obstacles, Enemies);

                if (Enemies[i].Pv <= 0)
                {
                    switch (Enemies[i].Type)
                    {
                        case EnemyType.MeleeBasic:
                            _score += 15;
                            break;
                        case EnemyType.MeleeTank:
                            _score += 20;
                            break;
                        case EnemyType.Ranged:
                            _score += 5;
                            break;
                    }

                    _gameCanvas.Children.Remove(Enemies[i].Sprite);
                    Enemies.RemoveAt(i);   
                }
            }
            CheckCollisions();
            Life(_gameCanvas, joueur);
            
        }

        private void UpdatePlayerBullets(double deltaTime, Canvas canvas)
        {
            for (int i = playerProjectiles.Count - 1; i >= 0; i--)
            {
                var bullet = playerProjectiles[i];
                playerProjectiles[i].Update(deltaTime);
                var sprite = bullet.Sprite as FrameworkElement;

                double centerX = bullet.X + (sprite.Width / 2);
                double centerY = bullet.Y + (sprite.Height / 2);

                Point bulletCenter = new Point(centerX, centerY);
                
                if (!PlayableArea.Contains(bulletCenter))
                {
                    bullet.IsMarkedForRemoval = true;
                }

                if (playerProjectiles[i].IsMarkedForRemoval)
                {
                    canvas.Children.Remove(playerProjectiles[i].Sprite);
                    playerProjectiles.RemoveAt(i);
                }
            }
        }

        private void UpdateEnemyBullets(double deltaTime, Canvas canvas)
        {
            for (int i = globalEnemyProjectiles.Count - 1; i >= 0; i--)
            {
                var bullet = globalEnemyProjectiles[i];
                globalEnemyProjectiles[i].Update(deltaTime);

                Point bulletPos = new Point(bullet.X, bullet.Y);

                if (!PlayableArea.Contains(bulletPos))
                {
                    bullet.IsMarkedForRemoval = true;
                }

                if (globalEnemyProjectiles[i].IsMarkedForRemoval)
                {
                    canvas.Children.Remove(globalEnemyProjectiles[i].Sprite);
                    globalEnemyProjectiles.RemoveAt(i);
                }
            }
        }

        public void UpdatePlayer(double deltaTime)
        {
            double dx = 0;
            double dy = 0;
            double speed = 350;
            double pixeldist = speed * deltaTime;
            double margin = 5;

            bool restartGame = false;
            bool stopGame = false;

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

            //CollisionCheck

            Rect futureX = new Rect(joueur.X + (dx * pixeldist), joueur.Y, 80, 100 - (margin * 2));
            Rect futureY = new Rect(joueur.X, joueur.Y + (dy * pixeldist), 80 - (margin * 2), 100);

            for (int i = 0; i < _mapLayout.obstacles.Count; i++)
            {
                if (_mapLayout.obstacles[i].ObstacleCollision(futureX) && _mapLayout.obstacles[i].Type != ObstacleType.Start && _mapLayout.obstacles[i].Type != ObstacleType.End)
                {
                    dx = 0;
                }

                else if (_mapLayout.obstacles[i].ObstacleCollision(futureX) && _mapLayout.obstacles[i].Type == ObstacleType.End && Enemies.Count == 0)
                {
                    for (int j = 0; j < _mapLayout.obstacles.Count; j++)
                    {
                        if (_mapLayout.obstacles[j].Type == ObstacleType.Start)
                        {
                            restartGame = true;
                        }
                        Console.WriteLine(dx);
                    }
                }
            }

            for (int i = 0; i < _mapLayout.obstacles.Count; i++)
            {
                if (_mapLayout.obstacles[i].ObstacleCollision(futureY) && _mapLayout.obstacles[i].Type != ObstacleType.Start && _mapLayout.obstacles[i].Type != ObstacleType.End)
                {
                    dy = 0;
                }

                else if (_mapLayout.obstacles[i].ObstacleCollision(futureY) && _mapLayout.obstacles[i].Type == ObstacleType.End && Enemies.Count == 0)
                {
                    for (int j = 0; j < _mapLayout.obstacles.Count; j++)
                    {
                        if (_mapLayout.obstacles[j].Type == ObstacleType.Start)
                        {
                            restartGame = true;
                        }
                        Console.WriteLine(dy);
                    }
                }
            }

            joueur.Deplacement(dx, dy, deltaTime);

            // Weapon switching 
            if (inputMng.IsKey1Pressed) SetWeapon(ProjectileTypePlayer.Standard);
            if (inputMng.IsKey2Pressed) SetWeapon(ProjectileTypePlayer.LightAxe);
            if (inputMng.IsKey3Pressed) SetWeapon(ProjectileTypePlayer.FireAxe);
            if (inputMng.IsKey4Pressed) SetWeapon(ProjectileTypePlayer.HeavyAxe);

            if (_fireTimerPlayer > 0) _fireTimerPlayer -= deltaTime;

            if (inputMng.IsShootPressed && _fireTimerPlayer <= 0)
            {
                SFXManager.PlaySound("axeSpinning.wav");
                SpawnBullet(mainWindow.canvas, "Player");
                _fireTimerPlayer = _currentCooldownDuration;
            }

            if (restartGame)
            {
                if (_mapNumber == _mapMax - 1 && MainWindow.GAMEMODE != "Infinite")
                {
                    Stop();
                    Win(_gameCanvas);
                }

                else
                {
                    Stop();
                    BeginGameplay();
                    _mapNumber++;
                }
            }

        }

        private void CheckCollisions()
        {

            // PLAYER BULLETS VS ENEMIES
            for (int i = playerProjectiles.Count - 1; i >= 0; i--)
            {
                var bullet = playerProjectiles[i];


                double bWidth = (bullet.Sprite != null) ? bullet.Sprite.Width : 60;
                double bHeight = (bullet.Sprite != null) ? bullet.Sprite.Height : 60;
                Rect bulletRect = new Rect(bullet.X, bullet.Y, bWidth, bHeight);

                for (int j = Enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = Enemies[j];
                    Rect enemyRect = new Rect(enemy.X, enemy.Y, enemy.Width, enemy.Height);

                    if (bulletRect.IntersectsWith(enemyRect))
                    {
                        
                        switch (bullet.Type)
                        {
                            case ProjectileTypePlayer.LightAxe:
                                enemy.Damage(5); 
                                break;
                            case ProjectileTypePlayer.HeavyAxe:
                                enemy.Damage(75); 
                                break;
                            case ProjectileTypePlayer.FireAxe:
                                enemy.Damage(20); 
                                break;
                            case ProjectileTypePlayer.Standard:
                            default:
                                enemy.Damage(25);
                                break;
                        }

                        if (bullet.CausesBurn)
                        {
                            enemy.ApplyBurn(3.0);
                        }

                        // Cleanup
                        if (bullet.Sprite != null)
                        {
                            _gameCanvas.Children.Remove(bullet.Sprite);
                        }
                        playerProjectiles.RemoveAt(i);

                        break; // Stop checking enemies for this bullet
                    }
                }
            }


            // ENEMY BULLETS VS PLAYER

            Rect playerRect = new Rect(joueur.X, joueur.Y, 80, 100);

            for (int k = globalEnemyProjectiles.Count - 1; k >= 0; k--)
            {
                var enemyBullet = globalEnemyProjectiles[k];

                // Use sprite size or default 10x10
                Rect eBulletRect = new Rect(enemyBullet.X, enemyBullet.Y, 10, 10);

                if (eBulletRect.IntersectsWith(playerRect))
                {
                    joueur.Damage(15);

                    if (enemyBullet.Sprite != null)
                    {
                        _gameCanvas.Children.Remove(enemyBullet.Sprite);
                    }
                    globalEnemyProjectiles.RemoveAt(k);

                    if (joueur.Hp <= 0)
                    {
                        Stop();
                        Lose(_gameCanvas);
                        return;
                    }
                }
            }

            // MELEE COLLISION (Enemy Body vs Player Body)

            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = Enemies[i];
                Rect enemyBodyRect = new Rect(enemy.X, enemy.Y, enemy.Width, enemy.Height);

                if (playerRect.IntersectsWith(enemyBodyRect))
                {
                    int contactDamage = 0;
                    switch (enemy.Type)
                    {
                        case EnemyType.MeleeTank: contactDamage = 25; break;
                        case EnemyType.MeleeBasic: contactDamage = 10; break;
                        case EnemyType.Ranged: contactDamage = 5; break;
                    }

                    joueur.Damage(contactDamage);

                    if (joueur.Hp < 0)
                    {
                        Stop();
                        Lose(_gameCanvas);
                        return;
                    }
                }
            }
        }

        public void EnemiesRandomizer(Canvas canvas, int Enemiesnumber, EnemyType type)
        {
            Random random = new Random();
            double height = 80;
            double width = 80;

            for (int x = 0; x < Enemiesnumber; x++)
            {
                bool validPosition = false;
                
                while (!validPosition )
                {
       
                    double randX = random.Next(0, (int)_gameCanvas.ActualWidth);
                    double randY = random.Next(0, (int)_gameCanvas.ActualHeight);
                    
                    bool collision = false; 

                    for (int i = 0; i < _mapLayout.obstacles.Count; i++)
                    {
                        
                        if (_mapLayout.obstacles[i].EnemyInObstacle(randX, randY, height * 5, width * 5))
                        {
                            collision = true; 
                            break; 
                        }
                    }

                    if (!collision)
                    {
                        validPosition = true;
                        Enemy enemy = new Enemy(randX, randY, type, height, width);
                        canvas.Children.Add(enemy.Sprite);
                        enemy.UpdatePosition();
                        Enemies.Add(enemy);
                    }
                }
            }

        }
        

        private void SetWeapon(ProjectileTypePlayer type)
        {
            _currentWeapon = type;

            switch (type)
            {
                case ProjectileTypePlayer.LightAxe:
                    _currentCooldownDuration = 0.42; // Fast 
                    UCGUI.Weapon.Content = "Hachette";
                    break;
                case ProjectileTypePlayer.FireAxe:
                    _currentCooldownDuration = 1.2; // Slow 
                    UCGUI.Weapon.Content = "Hache Enflammée";
                    break;
                case ProjectileTypePlayer.HeavyAxe:
                    _currentCooldownDuration = 2.2;
                    UCGUI.Weapon.Content = "Hache Lourde";
                    break;
                case ProjectileTypePlayer.Standard:
                default:
                    _currentCooldownDuration = 0.8;
                    UCGUI.Weapon.Content = "Hache de bûcheron";
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
                double diffX = (target.X)- player_startX;
                double diffY = (target.Y)- player_startY;
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

        
        
        //User controls functions
        private void GameRule(Canvas canva)
        {   
            GameRules uc = new GameRules();

            uc.Width = canva.Width;   // 1920
            uc.Height = canva.Height; // 1080

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

            uc.Width = canva.Width;   // 1920
            uc.Height = canva.Height; // 1080

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

            uc.Width = canva.Width;   // 1920
            uc.Height = canva.Height; // 1080

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

            uc.Width = canva.Width;   // 1920
            uc.Height = canva.Height; // 1080

            uc.play.Click += (sender, e) =>
            {
                canva.Children.Remove(uc);
                BeginGameplay();
                
            };
        }

        private void Win(Canvas canva)
        {
            UCWin uc = new UCWin();

            uc.Width = canva.Width;   // 1920
            uc.Height = canva.Height; // 1080

            canva.Children.Add(uc);
            uc.Score.Content = $"Score : {_score}";

            uc.Restart.Click += (sender, e) =>
            {
                canva.Children.Remove(uc);
                _mapNumber = 0;
                _score = 0;
                GameMode(canva);

            };
        }

        private void Lose(Canvas canva)
        {
            UCLose uc = new UCLose();

            uc.Width = canva.Width;   // 1920
            uc.Height = canva.Height; // 1080

            canva.Children.Add(uc);
            uc.Score.Content = $"Score : {_score}";

            uc.Restart.Click += (sender, e) =>
            {
                canva.Children.Remove(uc);
                _mapNumber = 0;
                _score = 0;
                GameMode(canva);

            };
        }

        private void Settings(Canvas canva, UCSettings settings)
        {
            canva.Children.Add(settings);
            Panel.SetZIndex(settings, 98);

            settings.Width = canva.Width;   // 1920
            settings.Height = canva.Height; // 1080

            settings.volume.Value = SFXManager.MasterVolume;

            settings.Close.Click += (sender, e) =>
            {
                SFXManager.SetVolume(settings.volume.Value);
                Resume();
                canva.Children.Remove(settings);
                
            };
        }
        private void GUI(Canvas canva, UCGUI uc)
        {
            

            uc.Width = canva.Width;   // 1920
            uc.Height = canva.Height; // 1080

            canva.Children.Add(uc);

            Panel.SetZIndex(uc, 98);
            uc.Settings.Click += (sender, e) =>
            {
                Pause();
                if (canva.Children.Contains(UCsettings))
                {
                    SFXManager.SetVolume(UCsettings.volume.Value);
                    Resume();
                    canva.Children.Remove(UCsettings);
                }
                else
                  Settings(canva, UCsettings);
            };
        }

        private void mapChange(Canvas canva)
        {
            _map = _random.Next(1, 3);
            _mapLayout = new MapLayout(_map, _gameCanvas);
        }

        public void Life(Canvas canvas, Player player)
        {
            double put;
            int playerMaxLife = 100;
            int life = player.Hp;
            double lenght = 1000;
            double coef = ((100 * life) / playerMaxLife) * 0.1;
            
            put = (lenght * coef) / 20;
            if  (!(put < 0))
                UCGUI.Green.Width = put;

            UCGUI.Life.Content = $"{life}/{playerMaxLife}";
            if (MainWindow.GAMEMODE == "Infinite")
            {
                UCGUI.Lvl.Content = $"Lvl : {_mapNumber + 1}";
            }
            else
            {
                UCGUI.Lvl.Content = $"Lvl : {_mapNumber + 1} / {_mapMax}";
            }

            UCGUI.Score.Content = $"Score : {_score}";
            
        }
    }
}
