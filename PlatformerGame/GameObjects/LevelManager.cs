using System.Collections.Generic;
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
                IsLocked = true, // Начинаем заблокированным
                Length = 4500,
                PlatformCount = 30,
                Difficulty = 3
            });
        }

        public LevelData GetCurrentLevel() => _levels[_currentLevelIndex];

        public List<LevelData> GetAllLevels() => _levels.ToList();

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