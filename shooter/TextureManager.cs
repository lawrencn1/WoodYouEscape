using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace shooter
{
    public class TextureManager
    {
        //PROJECTILE TEXTURES
        public static BitmapImage AxeTexture;

        //ENEMY TEXTURES
        public static BitmapImage[] MeleeUpFrames;
        public static BitmapImage[] MeleeDownFrames;
        public static BitmapImage[] MeleeSideFrames;

        public static BitmapImage[] TankUpFrames;
        public static BitmapImage[] TankDownFrames;
        public static BitmapImage[] TankSideFrames;

        public static BitmapImage[] RangedUpFrames;
        public static BitmapImage[] RangedDownFrames;
        public static BitmapImage[] RangedSideFrames;

        // PLAYER TEXTURES
        public static BitmapImage[] UpFrames;
        public static BitmapImage[] DownFrames;
        public static BitmapImage[] LeftFrames;
        public static BitmapImage[] RightFrames;



        public static void LoadTextures()
        {
            try
            {
                // Load Enemy Textures (WILL UPDATE TO ANIMS)
                // Bushbush
                MeleeUpFrames = LoadPlayerDirection("pack://application:,,,/enemySpritesheet/enemyDown.png", "pack://application:,,,/playerUpSpritesheet/walkingUp");
                MeleeDownFrames = LoadPlayerDirection("pack://application:,,,/enemySpritesheet/enemyUp.png", "pack://application:,,,/playerDownSpritesheet/walkingDown");
                MeleeSideFrames = LoadPlayerDirection("pack://application:,,,/enemySpritesheet/enemySide.png", "pack://application:,,,/playerLeftSpritesheet/walkingLeft");

                // 
                TankUpFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/backwardsIdle1.png", "pack://application:,,,/playerUpSpritesheet/walkingUp");
                TankDownFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/fowardIdle1.png", "pack://application:,,,/playerDownSpritesheet/walkingDown");
                TankSideFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/leftIdle1.png", "pack://application:,,,/playerLeftSpritesheet/walkingLeft");

                RangedUpFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/backwardsIdle1.png", "pack://application:,,,/playerUpSpritesheet/walkingUp");
                RangedDownFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/fowardIdle1.png", "pack://application:,,,/playerDownSpritesheet/walkingDown");
                RangedSideFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/leftIdle1.png", "pack://application:,,,/playerLeftSpritesheet/walkingLeft");


                // Load Player Animations
                UpFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/backwardsIdle1.png", "pack://application:,,,/playerUpSpritesheet/walkingUp");
                DownFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/fowardIdle1.png", "pack://application:,,,/playerDownSpritesheet/walkingDown");
                LeftFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/leftIdle1.png", "pack://application:,,,/playerLeftSpritesheet/walkingLeft");
                //RightFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/rightIdle1.png", "pack://application:,,,/playerRightSpritesheet/walkingRight");

                // Load Projectile Textures
                AxeTexture = LoadBitmap("pack://application:,,,/axes/normalAxe.png");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading textures: " + ex.Message);
            }
        }

        private static ImageBrush LoadBrush(string path)
        {
            var brush = new ImageBrush();
            brush.ImageSource = LoadBitmap(path);
            if (brush.CanFreeze) brush.Freeze();
            return brush;
        }

        private static BitmapImage[] LoadPlayerDirection(string idlePath, string walkPrefix)
        {
            var frames = new BitmapImage[11];

            // Frame 0 is Idle
            frames[0] = LoadBitmap(idlePath);

            // Frames 1-10 are Walking
            for (int i = 1; i < 11; i++)
            {
                frames[i] = LoadBitmap($"{walkPrefix}{i - 1}.png");
            }
            return frames;
        }

        private static BitmapImage LoadBitmap(string path)
        {
            try
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(path);
                img.CacheOption = BitmapCacheOption.OnLoad; // Load immediately
                img.EndInit();
                img.Freeze();
                return img;
            }
            catch
            {
                return null;
            }
        }
    }
}

