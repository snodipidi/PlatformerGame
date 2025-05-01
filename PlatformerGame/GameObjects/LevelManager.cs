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
            public int PlatformY { get; set; } // Относительно groundY
            public int MoveRange { get; set; }
            public int Speed { get; set; }
        }

        public LevelManager()
        {
            InitializeLevels();
        }

        private void InitializeLevels()
        {
            // Уровень 1
            _levels.Add(new LevelData
            {
                LevelNumber = 1,
                IsLocked = false,
                Length = 3000,
                PlatformCount = 20,
                Difficulty = 1
            });

            // Уровень 2 (с ловушками)
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

            // Уровень 3 (с врагами)
            var level3 = new LevelData
            {
                LevelNumber = 3,
                IsLocked = true,
                Length = 4000,
                PlatformCount = 0,
                Difficulty = 5
            };

            // Добавляем информацию о врагах через EnemyInfo
            level3.EnemyInfos = new List<EnemyInfo>
            {
                new EnemyInfo { X = 950, PlatformY = -120, MoveRange = 150, Speed = 3 },
                new EnemyInfo { X = 1950, PlatformY = -200, MoveRange = 120, Speed = 4 },
                new EnemyInfo { X = 3250, PlatformY = -130, MoveRange = 100, Speed = 2 }
            };
            _levels.Add(level3);

            // Уровень 4 (с колоннами)
            _levels.Add(new LevelData
            {
                LevelNumber = 4,
                IsLocked = true,
                Length = 5000,
                PlatformCount = 0,
                Difficulty = 7,
                Description = "Уровень с движущимися колоннами"
            });
        }

        public LevelData GetCurrentLevel() => _levels[_currentLevelIndex];

        public List<LevelData> GetAllLevels()
        {
            // Для теста разблокируем все уровни
            foreach (var level in _levels)
            {
                level.IsLocked = false;
            }
            return _levels.ToList();

            // В боевой версии:
            // return _levels.ToList();
        }

        public void SetCurrentLevel(int levelNumber)
        {
            var level = _levels.FirstOrDefault(l => l.LevelNumber == levelNumber);
            if (level != null && !level.IsLocked)
            {
                _currentLevelIndex = _levels.IndexOf(level);
            }
        }

        public void UnlockNextLevel()
        {
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