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
        private static int _poolSize = 10;
        public static double MasterVolume { get; private set; } = 1.0;

        static SFXManager()
        {
            _sfxPool = new List<MediaPlayer>();

            for (int i = 0; i < _poolSize; i++)
            {
                _sfxPool.Add(new MediaPlayer());
            }
        }

        public static void LoadMusic(string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory+ "Music/" + fileName;
            _musicPlayer.Open(new Uri(path));

            // Set up looping immediately
            _musicPlayer.MediaEnded += (sender, e) =>
            {
                _musicPlayer.Position = TimeSpan.Zero;
                _musicPlayer.Play();
            };

            // Prepare volume
            _musicPlayer.Volume = MasterVolume;
        }
        public static void PlayMusic()
        {
            _musicPlayer.Play();
        }

        public static void StopMusic()
        {
            _musicPlayer.Stop();
        }
        public static void SetVolume(double volume)
        {
            // Ensure volume is between 0.0 and 1.0
            if (volume < 0) volume = 0;
            if (volume > 1) volume = 1;

            MasterVolume = volume;
            _musicPlayer.Volume = MasterVolume; // Update music immediately
        }

        // SFX method (Creates a new player for every sound so they can overlap)
        public static void PlaySound(string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Music/" + fileName;

            MediaPlayer sfxPlayer = _sfxPool[_sfxPoolIndex];
            _sfxPoolIndex++;
            if (_sfxPoolIndex >= _poolSize) 
                _sfxPoolIndex = 0;
            sfxPlayer.Open(new Uri(path));
            sfxPlayer.Volume = 1.5;
            sfxPlayer.Play();
        }
    }
}
    

