using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace shooter
{
    public class Player
    {
        private double x;
        private double y;
        private double speed;
        private int hp;

        private Image sprite;

        private BitmapImage[] _animUpFrames;
        private BitmapImage[] _animDownFrames;
        private BitmapImage[] _animLeftFrames;
        private BitmapImage[] _animRightFrames;

        private int _currentUpFrame = 0;
        private int _currentDownFrame = 0;
        private int _currentLeftFrame = 0;
        private int _currentRightFrame = 0;
        private double _animTimer = 0;
        private const double FRAME_DURATION = 0.05;

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

        public double Speed
        {
            get
            {
                return this.speed;
            }

            set
            {
                this.speed = value;
            }
        }

        public int Hp // Health Points
        {
            get
            {
                return this.hp;
            }

            set
            {
                if (hp < 0)
                {
                    throw new ArgumentOutOfRangeException("HP must be a positive number");
                }
                this.hp = value;
            }
        }

        public Image Sprite
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

        public Player(double x, double y, double vitesse)
        {
            // Spawn position, movement speed, Health points & Sprite
            this.X = x;
            this.Y = y;
            this.Speed = vitesse;
            this.Hp = 100;

            Sprite = new Image
            {
                Width = 80,
                Height = 100,
                Stretch = Stretch.Uniform // Keeps the aspect ratio of your png
            };

            // Load images from the project resources
            // URI Format: "pack://application:,,,/Folder/filename.png"
            try
            {
                _animUpFrames = new BitmapImage[11];
                _animUpFrames[0] = LoadTexture("pack://application:,,,/playerIdleSpritesheet/backwardsIdle1.png");
                for (int i = 1; i < 11; i++)
                {
                    _animUpFrames[i] = LoadTexture($"pack://application:,,,/playerUpSpritesheet/walkingUp{i - 1}.png");
                }
                 
                _animDownFrames = new BitmapImage[11];
                _animDownFrames[0] = LoadTexture("pack://application:,,,/playerIdleSpritesheet/fowardIdle1.png");
                for (int i = 1; i < 11; i++)
                {
                    _animDownFrames[i] = LoadTexture($"pack://application:,,,/playerDownSpritesheet/walkingDown{i - 1}.png");
                }

                _animLeftFrames = new BitmapImage[11];
                _animLeftFrames[0] = LoadTexture("pack://application:,,,/playerIdleSpritesheet/leftIdle1.png");
                for (int i = 1; i < 11; i++)
                {
                    _animLeftFrames[i] = LoadTexture($"pack://application:,,,/playerLeftSpritesheet/walkingLeft{i - 1}.png");
                }

                _animRightFrames = new BitmapImage[11];
                _animRightFrames[0] = LoadTexture("pack://application:,,,/playerIdleSpritesheet/rightIdle1.png");
                for (int i = 1; i < 11; i++)
                {
                    _animRightFrames[i] = LoadTexture($"pack://application:,,,/playerRightSpritesheet/walkingRight{i - 1}.png");
                }

                // Set default sprite
                if (_animDownFrames[0] != null) Sprite.Source = _animDownFrames[0];

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading images: " + ex.Message);
            }
}

        private BitmapImage LoadTexture(string path)
        {
            try
            {
                return new BitmapImage(new Uri(path));
            }
            catch
            {
                return null;
            }
        }


        public void Deplacement(double DirX, double DirY, double deltaTime)
        {
            X += DirX * Speed * deltaTime;
            Y += DirY * Speed * deltaTime;

            if (DirY < -0.1) // UP
            {
                _animTimer += deltaTime;

                if (_animTimer > FRAME_DURATION)
                {
                    _animTimer = 0;
                    _currentUpFrame++;

                    if (_currentUpFrame >= _animUpFrames.Length)
                        _currentUpFrame = 0;
                }

                var currentFrame = _animUpFrames[_currentUpFrame];
                if (currentFrame != null && Sprite.Source != currentFrame)
                    Sprite.Source = currentFrame;
            }

            else if (DirY > 0.1) // DOWN
            {
                _animTimer += deltaTime;

                if (_animTimer > FRAME_DURATION)
                {
                    _animTimer = 0;
                    _currentDownFrame++;

                    if (_currentDownFrame >= _animDownFrames.Length)
                        _currentDownFrame = 0;
                }

                var currentFrame = _animDownFrames[_currentDownFrame];
                if (currentFrame != null && Sprite.Source != currentFrame)
                    Sprite.Source = currentFrame;
            }

            else if (DirX < -0.1) // Moving Left
            {

                _animTimer += deltaTime;

                if (_animTimer > FRAME_DURATION)
                {
                    _animTimer = 0;
                    _currentLeftFrame++;

                    if (_currentLeftFrame >= _animLeftFrames.Length)
                        _currentLeftFrame = 0;
                }

                var currentFrame = _animLeftFrames[_currentLeftFrame];
                if (currentFrame != null && Sprite.Source != currentFrame)
                    Sprite.Source = currentFrame;
            }

            else if (DirX > 0.1) // Moving Right
            {
                _animTimer += deltaTime;

                if (_animTimer > FRAME_DURATION)
                {
                    _animTimer = 0;
                    _currentRightFrame++;


                    if (_currentRightFrame >= _animRightFrames.Length)
                        _currentRightFrame = 0;
                }

                var currentFrame = _animRightFrames[_currentRightFrame];
                if (currentFrame != null && Sprite.Source != currentFrame)
                    Sprite.Source = currentFrame;
            }

            else // IDLE (Not moving)
            {
                _currentUpFrame = 0;
                _currentDownFrame = 0;
                _currentLeftFrame = 0;
                _currentRightFrame = 0;
                _animTimer = 0;

                // --- SMART IDLE LOGIC ---
                // This checks which direction you were last facing and snaps to the Idle frame (frame 0) of that direction.

                if (IsFrameInArray(Sprite.Source, _animUpFrames))
                {
                    if (Sprite.Source != _animUpFrames[0]) Sprite.Source = _animUpFrames[0];
                }
                else if (IsFrameInArray(Sprite.Source, _animLeftFrames))
                {
                    if (Sprite.Source != _animLeftFrames[0]) Sprite.Source = _animLeftFrames[0];
                }
                else if (IsFrameInArray(Sprite.Source, _animRightFrames))
                {
                    if (Sprite.Source != _animRightFrames[0]) Sprite.Source = _animRightFrames[0];
                }
                else
                {
                    // Default to Down Idle if nothing else matches or we were already facing down
                    if (_animDownFrames != null && _animDownFrames[0] != null)
                    {
                        if (Sprite.Source != _animDownFrames[0]) Sprite.Source = _animDownFrames[0];
                    }
                }
            }

            UpdatePosition();
        }
        
        private bool IsFrameInArray(ImageSource current, BitmapImage[] frames)
        {
            if (frames == null || current == null) return false;
            foreach (var frame in frames)
            {
                if (frame == current) return true;
            }
            return false;
        }

        public void UpdatePosition()
        {
            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);
        }
        public void Damage(int quantity)
        {
            Hp -= quantity;
            if (Hp <= 0)
            {
            }
        }
    }
}
