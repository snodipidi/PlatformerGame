using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PlatformerGame.GameObjects
{
    public class LevelManager
    {
        private List<LevelData> _levels = new List<LevelData>();
        private int _currentLevelIndex = 0;

        public LevelManager()
        {
            InitializeLevels();
        }
        private void InitializeLevels()
        {
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
                    new Rectangle(1200, 400, 50, 20),
                    new Rectangle(2200, 350, 50, 20),
                    new Rectangle(3200, 300, 50, 20)
                }
            });

            _levels.Add(new LevelData
            {
                LevelNumber = 3,
                IsLocked = true,
                Length = 5000,
                PlatformCount = 25,
                Difficulty = 5,
                Enemies = new List<Enemy>
        {
            new Enemy(1500, 400, 40, 30, 200, 3),
            new Enemy(2500, 350, 40, 30, 150, 4),
            new Enemy(3500, 300, 40, 30, 100, 2)
        }
            });
        }

        public LevelData GetCurrentLevel() => _levels[_currentLevelIndex];

        public List<LevelData> GetAllLevels()
        {
            // Временное отключение блокировки для тестов
            return _levels.Select(level =>
            {
                level.IsLocked = false; // Разблокируем все уровни
                return level;
            }).ToList();

            // Для рабочей версии замените на:
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