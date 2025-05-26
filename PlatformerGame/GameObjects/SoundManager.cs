using System;
using System.Diagnostics;
using System.IO;
using System.Media;

namespace PlatformerGame
{
    /// <summary>
    /// Статический класс для управления воспроизведением звуков и музыки в игре.
    /// </summary>
    public static class SoundManager
    {
        // Плеер для звука поражения
        private static SoundPlayer _gameoverSound;
        // Плеер для звука победы
        private static SoundPlayer _winSound;
        // Плеер для фоновой музыки
        private static SoundPlayer _musicPlayer;

        // Флаг, определяющий, включён ли звук в целом
        public static bool IsSoundEnabled { get; set; } = true;

        // Флаг разработчика (пока не используется)
        public static bool DeveloperMode { get; set; }

        // Путь к директории со звуками
        private static string _soundsPath;

        /// <summary>
        /// Инициализирует менеджер звуков, загружая нужные звуки.
        /// </summary>
        public static void Initialize()
        {
            // Формируем путь к папке Sounds, находящейся в папке запуска
            _soundsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

            // Загружаем звук поражения
            _gameoverSound = LoadSound(Path.Combine(_soundsPath, "gameover.wav"));

            // Загружаем звук победы
            _winSound = LoadSound(Path.Combine(_soundsPath, "win.wav"));
        }

        /// <summary>
        /// Загружает звуковой файл по указанному пути.
        /// </summary>
        /// <param name="path">Путь к файлу звука.</param>
        /// <returns>Объект SoundPlayer или null, если загрузка не удалась.</returns>
        private static SoundPlayer LoadSound(string path)
        {
            try
            {
                // Проверяем, существует ли файл
                if (!File.Exists(path))
                {
                    // Выводим отладочное сообщение, если файл не найден
                    Debug.WriteLine($"Файл не найден: {path}");
                    return null;
                }

                // Создаём SoundPlayer и загружаем звук
                var player = new SoundPlayer(path);
                player.Load();
                return player;
            }
            catch (Exception ex)
            {
                // В случае ошибки выводим сообщение
                Debug.WriteLine($"Ошибка загрузки звука: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Воспроизводит звук поражения, если он включён и загружен.
        /// </summary>
        public static void PlayGameOverSound()
        {
            if (IsSoundEnabled && _gameoverSound != null)
            {
                try
                {
                    // Запускаем воспроизведение
                    _gameoverSound.Play();
                }
                catch (Exception ex)
                {
                    // При ошибке выводим отладочное сообщение
                    Debug.WriteLine($"Ошибка воспроизведения звука поражения: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Воспроизводит звук победы, если он включён и загружен.
        /// </summary>
        public static void PlayWinSound()
        {
            if (IsSoundEnabled && _winSound != null)
            {
                try
                {
                    // Запускаем воспроизведение
                    _winSound.Play();
                }
                catch (Exception ex)
                {
                    // При ошибке выводим отладочное сообщение
                    Debug.WriteLine($"Ошибка воспроизведения звука победы: {ex.Message}");
                }
            }
        }

        // Флаг, включена ли фоновая музыка
        public static bool IsMusicEnabled { get; set; } = true;

        /// <summary>
        /// Воспроизводит фоновую музыку из указанного файла.
        /// </summary>
        /// <param name="fileName">Имя файла музыки.</param>
        public static void PlayMusic(string fileName)
        {
            // Останавливаем текущую музыку, если она играет
            StopMusic();

            // Проверяем, разрешено ли воспроизведение звука и музыки
            if (!IsSoundEnabled || !IsMusicEnabled)
                return;

            // Формируем полный путь к музыкальному файлу
            string path = Path.Combine(_soundsPath, fileName);

            // Если файл не найден — выходим
            if (!File.Exists(path))
            {
                Debug.WriteLine($"Музыка не найдена: {path}");
                return;
            }

            try
            {
                // Создаём плеер и запускаем бесконечное воспроизведение
                _musicPlayer = new SoundPlayer(path);
                _musicPlayer.Load();
                _musicPlayer.PlayLooping();
            }
            catch (Exception ex)
            {
                // В случае ошибки — сообщение в отладку
                Debug.WriteLine($"Ошибка воспроизведения музыки: {ex.Message}");
            }
        }

        /// <summary>
        /// Останавливает фоновую музыку, если она играет.
        /// </summary>
        public static void StopMusic()
        {
            try
            {
                // Если плеер существует — останавливаем и очищаем
                _musicPlayer?.Stop();
                _musicPlayer = null;
            }
            catch (Exception ex)
            {
                // Сообщение при ошибке остановки
                Debug.WriteLine($"Ошибка остановки музыки: {ex.Message}");
            }
        }
    }
}
