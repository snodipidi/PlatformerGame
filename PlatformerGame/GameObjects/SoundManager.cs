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

        // Для поражения (Game Over)
        public static void PlayGameOverSound()
        {
            _gameoverSound?.Play();
        }

        // Для победы (Level Complete)
        public static void PlayWinSound()
        {
            _winSound?.Play();
        }
    }
}