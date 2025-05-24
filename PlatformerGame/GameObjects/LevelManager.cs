using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PlatformerGame.GameObjects
{
    public class LevelManager
    {
        private List<LevelData> _levels = new List<LevelData>();
        private int _currentLevelIndex = 0;

        public class EnemyInfo
        {
            public int X { get; set; }
            public int PlatformY { get; set; }
            public int MoveRange { get; set; }
            public int Speed { get; set; }
        }

        public LevelManager()
        {
            InitializeLevels();
        }

        private void InitializeLevels()
        {
            // Инициализация уровней без изменений
            _levels.Add(new LevelData
            {
                LevelNumber = 1,
                IsLocked = false,
                Length = 3000,
                PlatformCount = 20,
                Difficulty = 1
            });

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
            _levels.Add(level3);

            _levels.Add(new LevelData
            {
                LevelNumber = 4,
                IsLocked = true,
                Length = 5000,
                PlatformCount = 0,
                Difficulty = 7,
                Description = "Уровень с движущимися колоннами"
            });

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

        public LevelData GetCurrentLevel() => _levels[_currentLevelIndex];

        public List<LevelData> GetAllLevels()
        {
            if (SoundManager.DeveloperMode)
            {
                // Возвращаем копию уровней с разблокировкой
                return _levels.Select(l => new LevelData
                {
                    LevelNumber = l.LevelNumber,
                    IsLocked = false, // Принудительно разблокируем
                    Length = l.Length,
                    PlatformCount = l.PlatformCount,
                    Difficulty = l.Difficulty,
                    Traps = l.Traps,
                    EnemyInfos = l.EnemyInfos,
                    Description = l.Description
                }).ToList();
            }
            return _levels.ToList();
        }

        public void SetCurrentLevel(int levelNumber)
        {
            var level = _levels.FirstOrDefault(l => l.LevelNumber == levelNumber);
            if (level != null && (SoundManager.DeveloperMode || !level.IsLocked))
            {
                _currentLevelIndex = _levels.IndexOf(level);
            }
        }

        public bool IsLevelUnlocked(int levelNumber)
        {
            if (SoundManager.DeveloperMode) return true;
            if (levelNumber < 1 || levelNumber > _levels.Count) return false;
            return _levels[levelNumber - 1].IsLocked == false;
        }

        public void UnlockNextLevel()
        {
            if (SoundManager.DeveloperMode) return;

            int nextIndex = _currentLevelIndex + 1;
            if (nextIndex < _levels.Count)
            {
                _levels[nextIndex].IsLocked = false;
            }
        }

        public bool HasNextLevel()
        {
            return _currentLevelIndex + 1 < _levels.Count;
        }
    }
}