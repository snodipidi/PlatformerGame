using System;
using System.Media;
using System.IO;
using System.Diagnostics;

namespace PlatformerGame
{
    public static class SoundManager
    {
        private static SoundPlayer _gameoverSound;
        private static SoundPlayer _winSound;

        // Флаг включения звука (по умолчанию включён)
        public static bool IsSoundEnabled { get; set; } = true;
        public static bool DeveloperMode { get; set; }

        public static void Initialize()
        {
            string soundsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

            _gameoverSound = LoadSound(Path.Combine(soundsPath, "gameover.wav"));
            _winSound = LoadSound(Path.Combine(soundsPath, "win.wav"));
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
                player.Load(); // Предварительная загрузка
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
            if (!IsSoundEnabled) return;
            _gameoverSound?.Play();
        }

        public static void PlayWinSound()
        {
            if (!IsSoundEnabled) return;
            _winSound?.Play();
        }
    }
}
