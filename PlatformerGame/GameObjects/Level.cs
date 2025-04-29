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
        public List<Enemy> Enemies { get; } = new List<Enemy>();

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

            if (data?.LevelNumber == 2) // Генерация для 2 уровня
            {
                int groundY = screenSize.Height - 100;
                int platformHeight = 20;
                int platformWidth = 300; // Ширина всех платформ
                int trapWidth = 100;    // Ширина ловушки
                int trapHeight = 25;    // Высота ловушки

                // 1. Стартовая безопасная зона
                Platforms.Add(new Rectangle(300, groundY - 50, platformWidth, platformHeight));
                Platforms.Add(new Rectangle(650, groundY - 100, platformWidth, platformHeight));

                // 2. Первая платформа с ловушкой по центру
                int platform1Y = groundY - 150;
                Platforms.Add(new Rectangle(1000, platform1Y, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1000 + (platformWidth - trapWidth) / 2, // Центрирование
                    platform1Y - trapHeight,
                    trapWidth,
                    trapHeight));

                // 3. Вторая платформа с ловушкой по центру
                int platform2Y = groundY - 130;
                Platforms.Add(new Rectangle(1600, platform2Y, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1600 + (platformWidth - trapWidth) / 2,
                    platform2Y - trapHeight,
                    trapWidth,
                    trapHeight));

                // 4. Третья платформа с ловушкой по центру
                int platform3Y = groundY - 170;
                Platforms.Add(new Rectangle(2200, platform3Y, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    2200 + (platformWidth - trapWidth) / 2,
                    platform3Y - trapHeight,
                    trapWidth,
                    trapHeight));

                // 5. Финальные безопасные платформы
                Platforms.Add(new Rectangle(2800, groundY - 100, platformWidth, platformHeight));
                Platforms.Add(new Rectangle(3200, groundY - 80, platformWidth, platformHeight));

                TotalLength = 3800;
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

            if (data?.LevelNumber == 3) // Фиксированный 3 уровень
            {
                int groundY = screenSize.Height - 100;
                int platformHeight = 20;
                int platformWidth = 250; // Базовая ширина платформ

                // 1. Стартовая безопасная зона
                Platforms.Add(new Rectangle(300, groundY - 50, platformWidth, platformHeight));
                Platforms.Add(new Rectangle(600, groundY - 100, platformWidth, platformHeight));

                // 2. Первая платформа с врагом (движется в пределах 200px)
                int enemy1Y = groundY - 150;
                Platforms.Add(new Rectangle(1000, enemy1Y, 300, platformHeight));
                Enemies.Add(new Enemy(1100, enemy1Y + platformHeight, 40, 30, 200, 3));

                // 3. Промежуточные платформы
                Platforms.Add(new Rectangle(1400, groundY - 80, 200, platformHeight));
                Platforms.Add(new Rectangle(1700, groundY - 120, 150, platformHeight));

                // 4. Вторая платформа с врагом (движется быстрее)
                int enemy2Y = groundY - 180;
                Platforms.Add(new Rectangle(2100, enemy2Y, 350, platformHeight));
                Enemies.Add(new Enemy(2200, enemy2Y + platformHeight, 50, 40, 150, 4));

                // 5. Секция с узкими платформами
                Platforms.Add(new Rectangle(2600, groundY - 150, 100, platformHeight));
                Platforms.Add(new Rectangle(2800, groundY - 190, 100, platformHeight));

                // 6. Третья платформа с врагом (короткий диапазон)
                int enemy3Y = groundY - 130;
                Platforms.Add(new Rectangle(3200, enemy3Y, 300, platformHeight));
                Enemies.Add(new Enemy(3300, enemy3Y + platformHeight, 60, 30, 100, 2));

                // 7. Финальные платформы
                Platforms.Add(new Rectangle(3600, groundY - 100, 200, platformHeight));
                Platforms.Add(new Rectangle(3900, groundY - 70, 250, platformHeight));

                TotalLength = 4200;
                lastPlatformX = TotalLength;

                // Удаляем случайные платформы из LevelData
                if (data != null) data.PlatformCount = 0;
            }
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
            foreach (var enemy in Enemies)
            {
                enemy.Update();
            }
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

            foreach (var enemy in Enemies)
            {
                enemy.Draw(g);
            }
        }

        public bool CheckPlayerCollision(Player player)
        {
            var playerBounds = player.GetBounds();

            // Проверка столкновения с ловушками
            foreach (var trap in Traps)
            {
                // Увеличиваем зону поражения на 5px сверху для надежности
                Rectangle trapKillZone = new Rectangle(
                    trap.X,
                    trap.Y - 5,
                    trap.Width,
                    trap.Height + 5);

                if (playerBounds.IntersectsWith(trapKillZone))
                {
                    return true;
                }
            }

            // Проверка столкновения с врагами
            foreach (var enemy in Enemies)
            {
                if (playerBounds.IntersectsWith(enemy.Bounds))
                {
                    return true;
                }
            }

            return false;
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