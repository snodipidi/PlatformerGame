using System.Collections.Generic;
using System.Drawing;

namespace PlatformerGame.GameObjects
{
    public class LevelData
    {
        public int LevelNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLocked { get; set; }
        public string PreviewImagePath { get; set; }

        // Параметры генерации уровня
        public int Length { get; set; }
        public int PlatformCount { get; set; }
        public int Difficulty { get; set; }
        public List<Rectangle> Traps { get; set; } = new List<Rectangle>();
    }
}