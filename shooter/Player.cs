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
    public enum Direction { Up, Down, Left, Right }
    public class Player
    {
        private double _x;
        private double _y;
        private double _speed;
        private int _hp;

        private Image _sprite;

        private double _invincibilityTimer = 0;
        private const double INVINCIBILITY_DURATION = 1.0; // 1 second of safety after hit
        private bool isInvincible = false;

        private ScaleTransform _flipTransform;

        private int _currentFrame = 0;
        private double _animTimer = 0;
        private const double FRAME_DURATION = 0.05;

        private Direction _lastDirection = Direction.Down;

        public double X
        {
            get
            {
                return this._x;
            }

            set
            {
                this._x = value;
            }
        }

        public double Y
        {
            get
            {
                return this._y;
            }

            set
            {
                this._y = value;
            }
        }

        public double Speed
        {
            get
            {
                return this._speed;
            }

            set
            {
                this._speed = value;
            }
        }

        public int Hp // Health Points
        {
            get
            {
                return this._hp;
            }

            set
            {
                if (_hp < 0)
                {
                    //throw new ArgumentOutOfRangeException("HP must be a positive number");
                }
                this._hp = value;
            }
        }

        public Image Sprite
        {
            get
            {
                return this._sprite;
            }

            set
            {
                this._sprite = value;
            }
        }

        public bool IsInvincible
        {
            get
            {
                return this.isInvincible;
            }

            set
            {
                this.isInvincible = value;
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
                Stretch = Stretch.Uniform,

                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            _flipTransform = new ScaleTransform();
            _flipTransform.ScaleX = 1;
            Sprite.RenderTransform = _flipTransform;

            if (TextureManager.DownFrames != null && TextureManager.DownFrames.Length > 0)
            {
                Sprite.Source = TextureManager.DownFrames[0];
            }
        }

        public void Deplacement(double DirX, double DirY, double deltaTime)
        {
            X += DirX * Speed * deltaTime;
            Y += DirY * Speed * deltaTime;

            double newX = X + (DirX * Speed * deltaTime);
            double newY = Y + (DirY * Speed * deltaTime);
            double spriteW = ((FrameworkElement)Sprite).Width;
            double spriteH = ((FrameworkElement)Sprite).Height;

            if (newX < GameEngine.PlayableArea.Left)
                newX = GameEngine.PlayableArea.Left;

            // Right Wall (PlayableArea.Right is X + Width)
            if (newX > GameEngine.PlayableArea.Right - spriteW)
                newX = GameEngine.PlayableArea.Right - spriteW;

            // Top Wall (Trees above)
            if (newY < GameEngine.PlayableArea.Top)
                newY = GameEngine.PlayableArea.Top;

            // Bottom Wall (Trees below)
            if (newY > GameEngine.PlayableArea.Bottom - spriteH)
                newY = GameEngine.PlayableArea.Bottom - spriteH;

            X = newX;
            Y = newY;

            bool isMoving = false;

            if (DirY < -0.1) // UP
            {
                _flipTransform.ScaleX = 1;
                Animate(TextureManager.UpFrames, deltaTime);
                _lastDirection = Direction.Up;
                isMoving = true;
            }
            else if (DirY > 0.1) // DOWN
            {
                _flipTransform.ScaleX = 1;
                Animate(TextureManager.DownFrames, deltaTime);
                _lastDirection = Direction.Down;
                isMoving = true;
            }
            else if (DirX < -0.1) // LEFT
            {
                _flipTransform.ScaleX = 1;
                Animate(TextureManager.LeftFrames, deltaTime);
                _lastDirection = Direction.Left;
                isMoving = true;
            }
            else if (DirX > 0.1) // RIGHT
            {
                _flipTransform.ScaleX = -1;
                Animate(TextureManager.LeftFrames, deltaTime);
                _lastDirection = Direction.Right;
                isMoving = true;
            }

            //  Idle Logic
            if (!isMoving)
            {
                _currentFrame = 0; 
                _animTimer = 0;

                switch (_lastDirection)
                {
                    case Direction.Up:
                        _flipTransform.ScaleX = 1;
                        SetSprite(TextureManager.UpFrames, 0);
                        break;
                    case Direction.Down:
                        _flipTransform.ScaleX = 1;
                        SetSprite(TextureManager.DownFrames, 0);
                        break;
                    case Direction.Left:
                        _flipTransform.ScaleX = 1;
                        SetSprite(TextureManager.LeftFrames, 0);
                        break;
                    case Direction.Right:
                        _flipTransform.ScaleX = -1;
                        SetSprite(TextureManager.LeftFrames, 0);
                        break;
                }
            }

            UpdatePosition();
        }
        private void Animate(BitmapImage[] frames, double deltaTime)
        {
            if (frames == null || frames.Length == 0) return;

            _animTimer += deltaTime;

            if (_animTimer > FRAME_DURATION)
            {
                _animTimer = 0;
                _currentFrame++;
                
                //Loop animation
                if (_currentFrame >= frames.Length)
                    _currentFrame = 1;
            }

            SetSprite(frames, _currentFrame);
        }
        private void SetSprite(BitmapImage[] frames, int index)
        {
            if (frames != null && index < frames.Length && index >= 0)
            {
                if (Sprite.Source != frames[index]) // Only updates if changed
                {
                    Sprite.Source = frames[index];
                }
            }
        }

        public void UpdatePosition()
        {
            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);
        }

        public void Damage(int amount)
        {
            if (IsInvincible) return;

            //Apply Damage
            this.Hp -= amount;

            IsInvincible = true;
            _invincibilityTimer = INVINCIBILITY_DURATION;

            if (Sprite != null) Sprite.Opacity = 0.5;
        }
        public void UpdateInvincibility(double deltaTime)
        {
            if (IsInvincible)
            {
                _invincibilityTimer -= deltaTime;
                if (_invincibilityTimer <= 0)
                {
                    IsInvincible = false;
                    if (Sprite != null) Sprite.Opacity = 1.0; 
                }
            }
        }
    }
}

