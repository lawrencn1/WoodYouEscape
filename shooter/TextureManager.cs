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
        public static BitmapImage FireAxeTexture;
        public static BitmapImage LightAxeTexture;
        public static BitmapImage HeavyAxeTexture;
        public static BitmapImage EnemyProjectileTexture;

        //ENEMY TEXTURES
        public static BitmapSource[] MeleeUpFrames;
        public static BitmapSource[] MeleeDownFrames;
        public static BitmapSource[] MeleeSideFrames;

        public static BitmapSource[] TankUpFrames;
        public static BitmapSource[] TankDownFrames;
        public static BitmapSource[] TankSideFrames;

        public static BitmapSource[] RangedUpFrames;
        public static BitmapSource[] RangedDownFrames;
        public static BitmapSource[] RangedSideFrames;

        // PLAYER TEXTURES
        public static BitmapImage[] UpFrames;
        public static BitmapImage[] DownFrames;
        public static BitmapImage[] LeftFrames;
        //public static BitmapImage[] RightFrames;


        public static void LoadTextures()
        {
            try
            {
                // Load Enemy Animations
                // Bushbush
                string meleeSheetPath = "pack://application:,,,/enemySpritesheet/MeleeSpriteSheet.png";
                string tankSheetPath = "pack://application:,,,/enemySpritesheet/TankSpriteSheet.png";
                string rangedSheetPath = "pack://application:,,,/enemySpritesheet/RangedSpriteSheet.png";

                // Row 0 is Down (Front)
                MeleeDownFrames = SliceSpriteSheet(meleeSheetPath, 8, 3, 0, 8);
                // Row 1 is Side
                MeleeSideFrames = SliceSpriteSheet(meleeSheetPath, 8, 3, 1, 8);
                // Row 2 is Up (Back)
                MeleeUpFrames = SliceSpriteSheet(meleeSheetPath, 8, 3, 2, 8);

                // 
                TankUpFrames = SliceSpriteSheet(tankSheetPath, 8, 3, 2, 8);
                TankSideFrames = SliceSpriteSheet(tankSheetPath, 8, 3, 1, 8);
                TankDownFrames = SliceSpriteSheet(tankSheetPath, 8, 3, 0, 8);

                RangedUpFrames = SliceSpriteSheet(rangedSheetPath, 8, 3, 2, 8);
                RangedDownFrames = SliceSpriteSheet(rangedSheetPath, 8, 3, 0, 8);
                RangedSideFrames = SliceSpriteSheet(rangedSheetPath, 8, 3, 1, 8);


                // Load Player Animations
                UpFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/backwardsIdle1.png", "pack://application:,,,/playerUpSpritesheet/walkingUp");
                DownFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/fowardIdle1.png", "pack://application:,,,/playerDownSpritesheet/walkingDown");
                LeftFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/leftIdle1.png", "pack://application:,,,/playerLeftSpritesheet/walkingLeft");
                //RightFrames = LoadPlayerDirection("pack://application:,,,/playerIdleSpritesheet/rightIdle1.png", "pack://application:,,,/playerRightSpritesheet/walkingRight");

                // Load Projectile Textures
                AxeTexture = LoadBitmap("pack://application:,,,/axes/normalAxe.png");
                FireAxeTexture = LoadBitmap("pack://application:,,,/axes/fireAxe.png");
                LightAxeTexture = LoadBitmap("pack://application:,,,/axes/lightAxe.png");
                HeavyAxeTexture = LoadBitmap("pack://application:,,,/axes/heavyAxe.png");
                EnemyProjectileTexture = LoadBitmap("pack://application:,,,/enemySpritesheet/enemyProjectile.png");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading textures: " + ex.Message);
            }
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

        private static BitmapSource[] SliceSpriteSheet(string path, int totalColumns, int totalRows, int targetRow, int framesToTake)
        {
            //Load the full Srite Sheet
            BitmapImage fullSheet = LoadBitmap(path);

            if (fullSheet == null) return new BitmapSource[0];

            //Calculate the size of one single frame
            int frameWidth = fullSheet.PixelWidth / totalColumns;
            int frameHeight = fullSheet.PixelHeight / totalRows;

            BitmapSource[] frames = new BitmapSource[framesToTake];

            //Loop through columns and cut the frames
            for (int i = 0; i < framesToTake; i++)
            {
                //Calculate X and Y coordinates for the cut
                int x = i * frameWidth;
                int y = targetRow * frameHeight; // Jump down to the specific row

                //Create the crop
                //Defines the rectangle (X, Y, Width, Height)
                var crop = new CroppedBitmap(fullSheet, new Int32Rect(x, y, frameWidth, frameHeight));

                // Freeze for performance
                crop.Freeze();

                frames[i] = crop;
            }

            return frames;
        }
    }
}

