using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;

namespace PlatformerGame.GameObjects
{
    /// <summary>
    /// Управляет списком уровней, текущим уровнем и логикой разблокировки.
    /// </summary>
    public class LevelManager
    {
        // Список всех уровней
        private List<LevelData> _levels = new List<LevelData>();

        // Индекс текущего активного уровня
        private int _currentLevelIndex = 0;

        // Флаг включения/отключения звуков
        public static bool IsSoundEnabled { get; set; } = true;

        // Плеер для звука победы
        private static SoundPlayer _victorySound;

        /// <summary>
        /// Структура для хранения информации об одном враге.
        /// </summary>
        public class EnemyInfo
        {
            // Координата X врага
            public int X { get; set; }

            // Y-координата платформы, на которой находится враг
            public int PlatformY { get; set; }

            // Диапазон передвижения врага
            public int MoveRange { get; set; }

            // Скорость врага
            public int Speed { get; set; }
        }

        /// <summary>
        /// Конструктор, инициализирующий уровни.
        /// </summary>
        public LevelManager()
        {
            // Вызов метода инициализации уровней
            InitializeLevels();
        }

        /// <summary>
        /// Инициализирует список уровней.
        /// </summary>
        private void InitializeLevels()
        {
            // Первый уровень — простой, разблокирован
            _levels.Add(new LevelData
            {
                LevelNumber = 1,
                IsLocked = false,
                Length = 3000,
                PlatformCount = 20,
                Difficulty = 1
            });

            // Второй уровень — с ловушками, заблокирован
            _levels.Add(new LevelData
            {
                LevelNumber = 2,
                IsLocked = true,
                Length = 4500,
                PlatformCount = 0,
                Difficulty = 3,
                Traps = new List<Rectangle>
                {
                    new Rectangle(1200, 400, 100, 25),
                    new Rectangle(2200, 350, 100, 25),
                    new Rectangle(3200, 300, 100, 25)
                }
            });

            // Третий уровень — с врагами, заблокирован
            var level3 = new LevelData
            {
                LevelNumber = 3,
                IsLocked = true,
                Length = 4000,
                PlatformCount = 0,
                Difficulty = 5,
                EnemyInfos = new List<EnemyInfo>
                {
                    new EnemyInfo { X = 950, PlatformY = -120, MoveRange = 150, Speed = 3 },
                    new EnemyInfo { X = 1950, PlatformY = -200, MoveRange = 120, Speed = 4 },
                    new EnemyInfo { X = 3250, PlatformY = -130, MoveRange = 100, Speed = 2 }
                }
            };

            // Добавляем уровень 3 в список
            _levels.Add(level3);

            // Четвёртый уровень с движущимися колоннами
            _levels.Add(new LevelData
            {
                LevelNumber = 4,
                IsLocked = true,
                Length = 5000,
                PlatformCount = 0,
                Difficulty = 7,
                Description = "Уровень с движущимися колоннами"
            });

            // Пятый уровень — самый сложный, с движущимися платформами
            _levels.Add(new LevelData
            {
                LevelNumber = 5,
                IsLocked = true,
                Length = 6000,
                PlatformCount = 0,
                Difficulty = 9,
                Description = "Смертельный микс с движущимися платформами"
            });
        }

        /// <summary>
        /// Возвращает данные текущего уровня.
        /// </summary>
        public LevelData GetCurrentLevel() => _levels[_currentLevelIndex];

        /// <summary>
        /// Возвращает список всех уровней, с разблокировкой в режиме разработчика.
        /// </summary>
        public List<LevelData> GetAllLevels()
        {
            // Если включен режим разработчика — возвращаем все уровни разблокированными
            if (SoundManager.DeveloperMode)
            {
                return _levels.Select(l => new LevelData
                {
                    LevelNumber = l.LevelNumber,
                    IsLocked = false,
                    Length = l.Length,
                    PlatformCount = l.PlatformCount,
                    Difficulty = l.Difficulty,
                    Traps = l.Traps,
                    EnemyInfos = l.EnemyInfos,
                    Description = l.Description
                }).ToList();
            }

            // Иначе — возвращаем копию оригинального списка
            return _levels.ToList();
        }

        /// <summary>
        /// Возвращает общий прогресс в виде числа от 0 до 1.
        /// </summary>
        public float GetTotalProgress()
        {
            // В режиме разработчика всегда возвращаем 100%
            if (SoundManager.DeveloperMode) return 1f;

            // Общее количество уровней
            int totalLevels = _levels.Count;

            // Если уровней нет — возвращаем 0
            if (totalLevels <= 1) return 0f;

            // Считаем количество пройденных уровней (разблокированных минус 1 текущий)
            int passedLevels = _levels.Count(l => !l.IsLocked) - 1;

            // Не допускаем отрицательного значения
            passedLevels = Math.Max(passedLevels, 0);

            // Вычисляем и возвращаем прогресс
            return (float)passedLevels / (totalLevels - 1);
        }

        /// <summary>
        /// Устанавливает текущий уровень по его номеру.
        /// </summary>
        public void SetCurrentLevel(int levelNumber)
        {
            // Ищем уровень по номеру
            var level = _levels.FirstOrDefault(l => l.LevelNumber == levelNumber);

            // Если найден и разблокирован (или включён режим разработчика) — устанавливаем
            if (level != null && (SoundManager.DeveloperMode || !level.IsLocked))
            {
                _currentLevelIndex = _levels.IndexOf(level);
            }
        }

        /// <summary>
        /// Проверяет, разблокирован ли уровень по его номеру.
        /// </summary>
        public bool IsLevelUnlocked(int levelNumber)
        {
            // В режиме разработчика — все уровни разблокированы
            if (SoundManager.DeveloperMode) return true;

            // Проверка на выход за пределы массива
            if (levelNumber < 1 || levelNumber > _levels.Count) return false;

            // Возвращаем значение IsLocked
            return _levels[levelNumber - 1].IsLocked == false;
        }

        /// <summary>
        /// Разблокирует следующий уровень, если режим разработчика не включён.
        /// </summary>
        public void UnlockNextLevel()
        {
            // В режиме разработчика — пропускаем
            if (SoundManager.DeveloperMode) return;

            // Получаем индекс следующего уровня
            int nextIndex = _currentLevelIndex + 1;

            // Если он существует — разблокируем
            if (nextIndex < _levels.Count)
            {
                _levels[nextIndex].IsLocked = false;
            }
        }

        /// <summary>
        /// Проверяет, существует ли следующий уровень.
        /// </summary>
        public bool HasNextLevel()
        {
            // В режиме разработчика — просто проверяем индекс
            if (SoundManager.DeveloperMode)
                return _currentLevelIndex + 1 < _levels.Count;

            // Иначе — проверяем, что следующий уровень не выходит за лимит
            return _currentLevelIndex + 1 < _levels.Count
                && _levels[_currentLevelIndex + 1].LevelNumber <= 5;
        }
    }
}
