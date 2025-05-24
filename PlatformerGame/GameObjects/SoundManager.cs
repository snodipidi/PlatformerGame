using System;
using System.Diagnostics;
using System.IO;
using System.Media;

namespace PlatformerGame
{
    public static class SoundManager
    {
        private static SoundPlayer _gameoverSound;
        private static SoundPlayer _winSound;
        private static SoundPlayer _musicPlayer;

        public static bool IsSoundEnabled { get; set; } = true;
        public static bool DeveloperMode { get; set; }

        private static string _soundsPath;

        public static void Initialize()
        {
            _soundsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

            _gameoverSound = LoadSound(Path.Combine(_soundsPath, "gameover.wav"));
            _winSound = LoadSound(Path.Combine(_soundsPath, "win.wav"));
        }

        private static SoundPlayer LoadSound(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Debug.WriteLine($"Файл не найден: {path}");
                    return null;
                }

                var player = new SoundPlayer(path);
                player.Load();
                return player;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки звука: {ex.Message}");
                return null;
            }
        }

        public static void PlayGameOverSound()
        {
            if (IsSoundEnabled && _gameoverSound != null)
            {
                try { _gameoverSound.Play(); }
                catch (Exception ex) { Debug.WriteLine($"Ошибка воспроизведения звука поражения: {ex.Message}"); }
            }
        }

        public static void PlayWinSound()
        {
            if (IsSoundEnabled && _winSound != null)
            {
                try { _winSound.Play(); }
                catch (Exception ex) { Debug.WriteLine($"Ошибка воспроизведения звука победы: {ex.Message}"); }
            }
        }

        public static void PlayMusic(string fileName)
        {
            StopMusic();

            if (!IsSoundEnabled) return;

            string path = Path.Combine(_soundsPath, fileName);
            if (!File.Exists(path))
            {
                Debug.WriteLine($"Музыка не найдена: {path}");
                return;
            }

            try
            {
                _musicPlayer = new SoundPlayer(path);
                _musicPlayer.Load();
                _musicPlayer.PlayLooping(); // Повторяется бесконечно
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка воспроизведения музыки: {ex.Message}");
            }
        }

        public static void StopMusic()
        {
            try
            {
                _musicPlayer?.Stop();
                _musicPlayer = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка остановки музыки: {ex.Message}");
            }
        }
    }
}
