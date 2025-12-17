using shooter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace shooter
{
    public enum ProjectileTypePlayer
    {
        Standard,
        FireAxe,
        LightAxe,
        HeavyAxe
    }

    public class PlayerProjectile
    {
        private ProjectileTypePlayer type;
        private double x;
        private double y;
        private double dirX;
        private double dirY;
        private double speed = 15;
        private Image sprite;
        private bool isMarkedForRemoval = false;

        private RotateTransform _rotationTransform;
        private double _rotationSpeed;

        private ScaleTransform _scaleTransform;
        public bool CausesBurn { get; set; } = false;

        public ProjectileTypePlayer Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }

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

        public double DirX
        {
            get
            {
                return this.dirX;
            }

            set
            {
                this.dirX = value;
            }
        }

        public double DirY
        {
            get
            {
                return this.dirY;
            }

            set
            {
                this.dirY = value;
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

        public bool IsMarkedForRemoval
        {
            get
            {
                return this.isMarkedForRemoval;
            }

            set
            {
                this.isMarkedForRemoval = value;
            }
        }

        public PlayerProjectile(double x, double y, double dirX, double dirY, ProjectileTypePlayer type = ProjectileTypePlayer.Standard)
        {
            X = x;
            Y = y;
            DirX = dirX;
            DirY = dirY;

            this.Type = type;

            //Initialize Transform & Image container
            _rotationTransform = new RotateTransform();
            _scaleTransform = new ScaleTransform();

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(_scaleTransform);
            transformGroup.Children.Add(_rotationTransform);


            Sprite = new Image
            {
                Stretch = Stretch.Uniform,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = transformGroup
            };

            //Configure Stats & Textures based on Type
            switch (type)
            {
                case ProjectileTypePlayer.FireAxe:
                    {
                        Speed = 500;

                        CausesBurn = true; // Triggers Burn

                        Sprite.Width = 120;
                        Sprite.Height = 120;
                        Sprite.Source = TextureManager.FireAxeTexture;

                        if (DirX < 0)
                        {
                            _scaleTransform.ScaleX = -1; // Flip Horizontally
                            _rotationSpeed = -540;       // Spin Counter-Clockwise
                        }
                        else
                        {
                            _scaleTransform.ScaleX = 1;  // Normal
                            _rotationSpeed = 540;        // Spin Clockwise
                        }

                        break;
                    }

                case ProjectileTypePlayer.LightAxe:
                    {
                        Speed = 690; 
                        Sprite.Width = 90;
                        Sprite.Height = 90;
                        Sprite.Source = TextureManager.LightAxeTexture;

                        if (DirX < 0)
                        {
                            _scaleTransform.ScaleX = -1; // Flip Horizontally
                            _rotationSpeed = -630;       // Spin Counter-Clockwise
                        }
                        else
                        {
                            _scaleTransform.ScaleX = 1;  // Normal
                            _rotationSpeed = 630;        // Spin Clockwise
                        }
                        break;
                    }

                case ProjectileTypePlayer.HeavyAxe:
                    {
                        Speed = 300; 

                        Sprite.Width = 130;
                        Sprite.Height = 130;
                        Sprite.Source = TextureManager.HeavyAxeTexture;

                        if (DirX < 0)
                        {
                            _scaleTransform.ScaleX = -1; // Flip Horizontally
                            _rotationSpeed = -495;       // Spin Counter-Clockwise
                        }
                        else
                        {
                            _scaleTransform.ScaleX = 1;  // Normal
                            _rotationSpeed = 495;        // Spin Clockwise
                        }
                        break;
                    }
                case ProjectileTypePlayer.Standard:
                default:
                    {
                        Speed = 450;

                        Sprite.Width = 120;
                        Sprite.Height = 120;
                        Sprite.Source = TextureManager.AxeTexture;

                        if (DirX < 0)
                        {
                            _scaleTransform.ScaleX = -1; // Flip Horizontally
                            _rotationSpeed = -540;       // Spin Counter-Clockwise
                        }
                        else
                        {
                            _scaleTransform.ScaleX = 1;  // Normal
                            _rotationSpeed = 540;        // Spin Clockwise
                        }
                        break;
                    }
            }

            //Set Initial Position
            if (Sprite != null)
            {
                Canvas.SetLeft(Sprite, X);
                Canvas.SetTop(Sprite, Y);
            }
        }


        public void Update(double deltaTime)
        {
            X += DirX * Speed * deltaTime;
            Y += DirY * Speed * deltaTime;

            if (_rotationTransform != null)
            {
                _rotationTransform.Angle += _rotationSpeed * deltaTime;
            }

            if (Sprite != null)
            {
                Canvas.SetLeft(Sprite, X);
                Canvas.SetTop(Sprite, Y);
            }

            if (Y < -50 || Y > 2000 || X < -50 || X > 2000)
            {
                IsMarkedForRemoval = true;
            }
        }
    }
}

