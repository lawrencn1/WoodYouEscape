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
        public static ImageBrush MeleeTexture;
        public static ImageBrush TankTexture;
        public static ImageBrush RangedTexture;

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
                MeleeTexture = LoadBrush("pack://application:,,,/shooter;component/UIAssets/melee.png");
                TankTexture = LoadBrush("pack://application:,,,/shooter;component/UIAssets/tank.png");
                RangedTexture = LoadBrush("pack://application:,,,/shooter;component/UIAssets/ranged.png");

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

