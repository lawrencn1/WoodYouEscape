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
        public static BitmapSource[] MeleeUpFrames;
        public static BitmapSource[] MeleeDownFrames;
        public static BitmapSource[] MeleeSideFrames;

        public static BitmapSource[] TankUpFrames;
        public static BitmapSource[] TankDownFrames;
        public static BitmapSource[] TankSideFrames;

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
                // Load Enemy Animations
                // Bushbush
                string bushSheetPath = "pack://application:,,,/enemySpritesheet/MeleeSpriteSheet.png";
                string treeSheetPath = "pack://application:,,,/enemySpritesheet/TankSpriteSheet.png";

                // Row 0 is Down (Front)
                MeleeDownFrames = SliceSpriteSheet(bushSheetPath, 8, 3, 0, 8);
                // Row 1 is Side
                MeleeSideFrames = SliceSpriteSheet(bushSheetPath, 8, 3, 1, 8);
                // Row 2 is Up (Back)
                MeleeUpFrames = SliceSpriteSheet(bushSheetPath, 8, 3, 2, 8);

                // 
                TankUpFrames = SliceSpriteSheet(treeSheetPath, 8, 3, 0, 8);
                TankDownFrames = SliceSpriteSheet(treeSheetPath, 8, 3, 1, 8);
                TankSideFrames = SliceSpriteSheet(treeSheetPath, 8, 3, 2, 8);

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
            // 1. Load the full Master Sheet
            BitmapImage fullSheet = LoadBitmap(path);

            if (fullSheet == null) return new BitmapSource[0];

            // 2. Calculate the size of one single frame
            int frameWidth = fullSheet.PixelWidth / totalColumns;
            int frameHeight = fullSheet.PixelHeight / totalRows;

            BitmapSource[] frames = new BitmapSource[framesToTake];

            // 3. Loop through columns and cut the frames
            for (int i = 0; i < framesToTake; i++)
            {
                // Calculate X and Y coordinates for the cut
                int x = i * frameWidth;
                int y = targetRow * frameHeight; // Jump down to the specific row

                // Create the crop
                // Int32Rect defines the rectangle (X, Y, Width, Height)
                var crop = new CroppedBitmap(fullSheet, new Int32Rect(x, y, frameWidth, frameHeight));

                // Freeze it for performance
                crop.Freeze();

                frames[i] = crop;
            }

            return frames;
        }
    }
}

