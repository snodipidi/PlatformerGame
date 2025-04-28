using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Linq;

namespace PlatformerGame.GameObjects
{
    public class Level
    {
        public Rectangle StartPlatform { get; }
        public List<Rectangle> Platforms { get; } = new List<Rectangle>();
        public int CameraOffset { get; set; }
        public Rectangle FinishFlag { get; private set; }
        public bool IsLevelCompleted { get; private set; }
        public int TotalLength { get; private set; }
        public Bitmap FinishFlagTexture => _finishFlagTexture;
        public float StartPosition => StartPlatform.X + 50;
        public List<Rectangle> Traps { get; } = new List<Rectangle>();

        private readonly Random random = new Random();
        private int lastPlatformX;
        private readonly Size screenSize;
        private readonly Bitmap _blockTexture;
        private readonly Bitmap _finishFlagTexture;
        private const int FinishAreaWidth = 300;

        public float Progress
        {
            get
            {
                if (TotalLength <= StartPosition) return 1f;
                float totalDistance = FinishFlag.X - StartPosition;
                float traveledDistance = CameraOffset + (screenSize.Width / 3) - StartPosition;

                float progress = traveledDistance / totalDistance;
                return Math.Min(1f, Math.Max(0f, progress));
            }
        }

        public Level(Size screenSize, LevelData data = null)
        {
            try
            {
                _blockTexture = new Bitmap("C:\\Users\\msmil\\source\\repos\\PlatformerGame\\PlatformerGame\\Resourses\\block.png");
                _finishFlagTexture = new Bitmap("C:\\Users\\msmil\\source\\repos\\PlatformerGame\\PlatformerGame\\Resourses\\finish_flag.png");
            }
            catch
            {
                _blockTexture = null;
                _finishFlagTexture = null;
            }

            this.screenSize = screenSize;
            TotalLength = data?.Length ?? 3000;

            // Стартовая платформа
            StartPlatform = new Rectangle(0, screenSize.Height - 100, 300, 20);
            Platforms.Add(StartPlatform);
            lastPlatformX = StartPlatform.Right;

            if (data?.LevelNumber == 2) // Полностью фиксированный 2 уровень
            {
                // Основные параметры уровня
                int groundY = screenSize.Height - 100;
                int platformHeight = 20;

                // 1. Стартовая зона (безопасная)
                Platforms.Add(new Rectangle(300, groundY - 50, 200, platformHeight));
                Platforms.Add(new Rectangle(550, groundY - 100, 150, platformHeight));

                // 2. Первая опасная зона (3 платформы с ловушками)
                int trapSectionY = groundY - 150;
                Platforms.Add(new Rectangle(800, trapSectionY, 200, platformHeight));
                Traps.Add(new Rectangle(850, trapSectionY - 20, 100, 20)); // Ловушка сверху

                Platforms.Add(new Rectangle(1100, trapSectionY + 50, 200, platformHeight));
                Traps.Add(new Rectangle(1150, trapSectionY + 30, 100, 20));

                Platforms.Add(new Rectangle(1400, trapSectionY, 200, platformHeight));
                Traps.Add(new Rectangle(1450, trapSectionY - 20, 100, 20));

                // 3. Безопасная зона для передышки
                Platforms.Add(new Rectangle(1700, groundY - 80, 300, platformHeight));

                // 4. Вторая опасная зона (движущиеся платформы с ловушками)
                Platforms.Add(new Rectangle(2100, groundY - 200, 150, platformHeight));
                Traps.Add(new Rectangle(2125, groundY - 220, 100, 20));

                Platforms.Add(new Rectangle(2300, groundY - 150, 150, platformHeight));
                Traps.Add(new Rectangle(2325, groundY - 170, 100, 20));

                // 5. Финальный отрезок к флагу
                Platforms.Add(new Rectangle(2600, groundY - 100, 200, platformHeight));
                Platforms.Add(new Rectangle(2900, groundY - 150, 200, platformHeight));
                Platforms.Add(new Rectangle(3200, groundY - 80, 200, platformHeight));

                // Фиксируем общую длину уровня
                TotalLength = 3500;
                lastPlatformX = TotalLength;
            }
            else
            {
                // Обычная генерация для других уровней
                int platformCount = data?.PlatformCount ?? 20;
                int difficultyLevel = data?.Difficulty ?? 1;

                for (int i = 0; i < platformCount; i++)
                    GeneratePlatform(difficultyLevel);
            }

            GenerateFinalPlatforms(data?.Difficulty ?? 1);
            CreateFinishFlag();
        }

        private void GeneratePlatform(int difficulty)
        {
            int width = random.Next(80, 150 - difficulty * 10);
            int x = lastPlatformX + random.Next(100, 250 - difficulty * 20);
            int y = random.Next(screenSize.Height / 2, screenSize.Height - 50);

            Platforms.Add(new Rectangle(x, y, width, 20));
            lastPlatformX = x + width;
        }

        private void GenerateFinalPlatforms(int difficulty)
        {
            // Большая финальная платформа
            int finalPlatformWidth = 200;
            int finalPlatformX = TotalLength - finalPlatformWidth;
            int finalPlatformY = screenSize.Height - 120;
            Platforms.Add(new Rectangle(finalPlatformX, finalPlatformY, finalPlatformWidth, 20));

            // Несколько маленьких платформ перед финишем
            for (int i = 1; i <= 3; i++)
            {
                int x = finalPlatformX - 150 * i;
                int y = finalPlatformY - (i % 2 == 0 ? 50 : 0);
                Platforms.Add(new Rectangle(x, y, 80, 15));
            }

            lastPlatformX = TotalLength;
        }

        private void CreateFinishFlag()
        {
            var lastPlatform = Platforms[Platforms.Count - 1];
            FinishFlag = new Rectangle(
                lastPlatform.Right - 25,
                lastPlatform.Y - 60,
                30,
                60);
        }

        public void Update(float playerX)
        {
            CameraOffset = (int)(playerX - screenSize.Width / 3);
            int maxOffset = TotalLength - screenSize.Width;
            CameraOffset = Math.Min(maxOffset, Math.Max(0, CameraOffset));
        }

        public void CheckCompletion(Player player)
        {
            if (!IsLevelCompleted && player.GetBounds().IntersectsWith(FinishFlag))
            {
                IsLevelCompleted = true;
                Debug.WriteLine("Уровень пройден!");
            }
        }

        public void Draw(Graphics g)
        {
            if (_blockTexture != null)
            {
                foreach (var platform in Platforms)
                {
                    DrawTexturedPlatform(g, platform);
                }
            }
            else
            {
                foreach (var platform in Platforms)
                {
                    g.FillRectangle(Brushes.Green, platform);
                }
            }

            if (_finishFlagTexture != null)
            {
                g.DrawImage(_finishFlagTexture, FinishFlag);
            }
            else
            {
                g.FillRectangle(Brushes.Red, FinishFlag);
                g.DrawRectangle(Pens.DarkRed, FinishFlag);
            }

            foreach (var trap in Traps)
            {
                int spikeCount = 5;
                int spikeWidth = trap.Width / spikeCount;

                for (int i = 0; i < spikeCount; i++)
                {
                    Point[] spike = {
                new Point(trap.X + i * spikeWidth, trap.Y + trap.Height), 
                new Point(trap.X + (i + 1) * spikeWidth, trap.Y + trap.Height), 
                new Point(trap.X + i * spikeWidth + spikeWidth/2, trap.Y) 
            };
                    g.FillPolygon(Brushes.Black, spike);
                }
            }
        }

        private void DrawTexturedPlatform(Graphics g, Rectangle platform)
        {
            if (platform.Width > _blockTexture.Width)
            {
                using (var brush = new TextureBrush(_blockTexture, WrapMode.Tile))
                {
                    brush.TranslateTransform(platform.X, platform.Y);
                    g.FillRectangle(brush, platform);
                }
            }
            else
            {
                g.DrawImage(_blockTexture, platform);
            }
        }

        public void Reset()
        {
            CameraOffset = 0;
            IsLevelCompleted = false;
        }
    }
}