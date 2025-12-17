using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace shooter
{
    public static class SFXManager
    {
        private static MediaPlayer _musicPlayer = new MediaPlayer();

        private static List<MediaPlayer> _sfxPool;
        private static int _sfxPoolIndex = 0;
        private static int _poolSize = 5;
        private static double masterVolume = 1.0;

        public static MediaPlayer MusicPlayer
        {
            get
            {
                return _musicPlayer;
            }

            set
            {
                _musicPlayer = value;
            }
        }

        public static List<MediaPlayer> SfxPool
        {
            get
            {
                return _sfxPool;
            }

            set
            {
                _sfxPool = value;
            }
        }

        public static int SfxPoolIndex
        {
            get
            {
                return _sfxPoolIndex;
            }

            set
            {
                _sfxPoolIndex = value;
            }
        }

        public static int PoolSize
        {
            get
            {
                return _poolSize;
            }

            set
            {
                _poolSize = value;
            }
        }

        public static double MasterVolume
        {
            get
            {
                return masterVolume;
            }

            set
            {
                masterVolume = value;
            }
        }

        static SFXManager()
        {
            SfxPool = new List<MediaPlayer>();

            for (int i = 0; i < PoolSize; i++)
            {
                SfxPool.Add(new MediaPlayer());
            }
        }

        public static void LoadMusic(string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory+ "Music/" + fileName;
            MusicPlayer.Open(new Uri(path));

            //Set up looping immediately
            MusicPlayer.MediaEnded += (sender, e) =>
            {
                MusicPlayer.Position = TimeSpan.Zero;
                MusicPlayer.Play();
            };

            // Prepare volume
            MusicPlayer.Volume = MasterVolume;
        }
        public static void PlayMusic()
        {
            MusicPlayer.Play();
        }

        public static void StopMusic()
        {
            MusicPlayer.Stop();
        }
        public static void SetVolume(double volume)
        {
            // Ensure volume is between 0.0 and 1.0
            if (volume < 0)
                volume = 0;
            if (volume > 1) 
                volume = 1;

            MasterVolume = volume;
            MusicPlayer.Volume = MasterVolume; // Update volume immediately
        }

        // SFX method
        public static void PlaySound(string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Music/" + fileName;

            MediaPlayer sfxPlayer = SfxPool[SfxPoolIndex];
            SfxPoolIndex++;
            if (SfxPoolIndex >= PoolSize) 
                SfxPoolIndex = 0;
            sfxPlayer.Open(new Uri(path));
            sfxPlayer.Volume = 1.5;
            sfxPlayer.Play();
        }
    }
}
    

