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

        private BitmapImage _textureUp;
        private BitmapImage[] _animDownFrames;
        private BitmapImage _textureLeft;
        private BitmapImage _textureRight;

        private int _currentDownFrame = 0;
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
                _textureUp = LoadTexture("pack://application:,,,/playerIdleSpritesheet/backwardsIdle1.png");
                _textureLeft = LoadTexture("pack://application:,,,/playerIdleSpritesheet/leftIdle1.png");
                _textureRight = LoadTexture("pack://application:,,,/playerIdleSpritesheet/rightIdle1.png");

                _animDownFrames = new BitmapImage[11];
                _animDownFrames[0] = LoadTexture("pack://application:,,,/playerIdleSpritesheet/fowardIdle1.png");
                for (int i = 1; i < 11; i++)
                {
                    _animDownFrames[i] = LoadTexture($"pack://application:,,,/playerDownSpritesheet/walkingDown{i-1}.png");
                }

                // Set default sprite
                if (_animDownFrames[0] != null) Sprite.Source = _animDownFrames[0];
                else if (_textureUp != null) Sprite.Source = _textureUp;
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
            if (DirY < -0.1)
            {
                if (_textureUp != null) Sprite.Source = _textureUp;
            }

            else if (DirY > 0.1)
            {
                // 4.Run the Animation Logic
                _animTimer += deltaTime;
                Console.WriteLine(_animTimer);

                if (_animTimer > FRAME_DURATION)
                {
                    _animTimer = 0;
                    _currentDownFrame++;

                    if (_currentDownFrame >= _animDownFrames.Length)
                        _currentDownFrame = 0;
                }

                // Display the current frame
                if (_animDownFrames[_currentDownFrame] != null)
                    Sprite.Source = _animDownFrames[_currentDownFrame];

            }
            else if (DirX < -0.1)
            {
                if (_textureLeft != null) Sprite.Source = _textureLeft;
            }
            else if (DirX > 0.1)
            {
                if (_textureRight != null) Sprite.Source = _textureRight;
            }
            else // IDLE 
            {
                _currentDownFrame = 0;
                _animTimer = 0;


                if (Sprite.Source != _textureUp && Sprite.Source != _textureLeft && Sprite.Source != _textureRight)
                {
                    if (_animDownFrames != null && _animDownFrames[0] != null)
                    {
                        if (Sprite.Source != _animDownFrames[0])
                        {
                            Sprite.Source = _animDownFrames[0];
                        }
                    }
                }

                UpdatePosition();
            }
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
