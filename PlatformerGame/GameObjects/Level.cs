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
        public List<ColumnEnemy> ColumnEnemies { get; } = new List<ColumnEnemy>();

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

            if (data?.LevelNumber == 3)
            {
                int groundY = screenSize.Height - 100;
                int platformHeight = 20;
                int spikesHeight = 25;

                // 1. Стартовая зона (2 платформы)
                Platforms.Add(new Rectangle(300, groundY - 50, 200, platformHeight));
                Platforms.Add(new Rectangle(550, groundY - 100, 180, platformHeight));

                // 2. Первый враг (единственный)
                int enemyPlatformY = groundY - 120;
                Platforms.Add(new Rectangle(900, enemyPlatformY, 220, platformHeight));
                Enemies.Add(new Enemy(
                    950,                    // X позиция
                    enemyPlatformY,         // Y верхнего края платформы
                    150,                    // Диапазон движения
                    3));                    // Скорость

                // 3. Первые шипы (удлиненная платформа)
                int spikes1PlatformY = groundY - 100;
                int spikes1PlatformWidth = 250; // Увеличили ширину
                Platforms.Add(new Rectangle(1300, spikes1PlatformY, spikes1PlatformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1300 + (spikes1PlatformWidth - 100) / 2, // Центрируем шипы
                    spikes1PlatformY - spikesHeight,
                    100,  // Ширина шипов
                    spikesHeight));

                // 4. Вторые шипы (удлиненная платформа)
                int spikes2PlatformY = groundY - 100;
                int spikes2PlatformWidth = 300; // Увеличили ширину
                Platforms.Add(new Rectangle(1700, spikes2PlatformY, spikes2PlatformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1700 + (spikes2PlatformWidth - 100) / 2,
                    spikes2PlatformY - spikesHeight,
                    100,  // Ширина шипов
                    spikesHeight));

                // 5. Широкие платформы для передышки
                Platforms.Add(new Rectangle(2200, groundY - 120, 350, platformHeight));
                Platforms.Add(new Rectangle(2650, groundY - 80, 300, platformHeight));

                // 6. Финишная платформа
                Platforms.Add(new Rectangle(3050, groundY - 60, 250, platformHeight));

                TotalLength = 3500; // Немного увеличили длину
                lastPlatformX = TotalLength;
            }

            if (data?.LevelNumber == 4) // Генерация 4 уровня с фиксированными платформами
            {
                int groundY = screenSize.Height - 100;
                int platformHeight = 20;
                int spikesHeight = 25;
                int platformWidth = 200;

                // 1. Стартовая зона (3 платформы)
                Platforms.Add(new Rectangle(300, groundY - 50, platformWidth, platformHeight));
                Platforms.Add(new Rectangle(600, groundY - 100, platformWidth, platformHeight));
                Platforms.Add(new Rectangle(900, groundY - 80, platformWidth, platformHeight));

                // 2. Первая колонна с платформой
                int column1X = 1200;
                Platforms.Add(new Rectangle(column1X, groundY - 120, platformWidth, platformHeight));
                ColumnEnemies.Add(new ColumnEnemy(
                    column1X + platformWidth / 2, // Центр платформы
                    groundY - 120,
                    30,  // Ширина колонны
                    150, // Высота колонны
                    120, // Диапазон движения
                    2    // Скорость
                ));

                // 3. Шипы после первой колонны
                Platforms.Add(new Rectangle(1600, groundY - 100, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1600 + (platformWidth - 100) / 2, // Центрированные шипы
                    groundY - 100 - spikesHeight,
                    100, // Ширина шипов
                    spikesHeight
                ));

                // 4. Враг на отдельной платформе
                int enemyPlatformX = 2000;
                Platforms.Add(new Rectangle(enemyPlatformX, groundY - 150, platformWidth, platformHeight));
                Enemies.Add(new Enemy(
                    enemyPlatformX + 20, // Позиция X
                    groundY - 150,      // Y платформы
                    100,                // Диапазон движения
                    2                   // Скорость
                ));

                // 5. Вторая колонна с платформой
                int column2X = 2500;
                Platforms.Add(new Rectangle(column2X, groundY - 80, platformWidth, platformHeight));
                ColumnEnemies.Add(new ColumnEnemy(
                    column2X + platformWidth / 2,
                    groundY - 80,
                    35, 180, 150, 3 // Более высокая и быстрая колонна
                ));

                // 6. Финальные платформы с шипами
                Platforms.Add(new Rectangle(3000, groundY - 100, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    3000 + (platformWidth - 120) / 2,
                    groundY - 100 - spikesHeight,
                    120,
                    spikesHeight
                ));
                Platforms.Add(new Rectangle(3400, groundY - 60, 250, platformHeight));

                TotalLength = 3800;
                lastPlatformX = TotalLength;
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
            foreach (var enemy in ColumnEnemies)
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

            // После отрисовки платформ:
            foreach (var trap in Traps)
            {
                int spikeCount = trap.Width / 20; // По шипу каждые 20px
                int spikeWidth = trap.Width / spikeCount;

                using (var spikeBrush = new SolidBrush(Color.DarkRed))
                {
                    for (int i = 0; i < spikeCount; i++)
                    {
                        Point[] spike = {
                        new Point(trap.X + i * spikeWidth, trap.Bottom),
                        new Point(trap.X + (i + 1) * spikeWidth, trap.Bottom),
                        new Point(trap.X + i * spikeWidth + spikeWidth/2, trap.Top)
                        };
                        g.FillPolygon(spikeBrush, spike);
                    }
                }
            }

            foreach (var enemy in Enemies)
            {
                enemy.Draw(g);
            }
            foreach (var enemy in ColumnEnemies)
            {
                enemy.Draw(g);
            }
        }

        public bool CheckPlayerCollision(Player player)
        {
            var playerBounds = player.GetBounds();
            foreach (var enemy in ColumnEnemies)
            {
                if (playerBounds.IntersectsWith(enemy.GetKillZone()))
                    return true;
            }
            foreach (var enemy in Enemies)
            {
                if (playerBounds.IntersectsWith(enemy.Bounds))
                    return true;
            }
            foreach (var trap in Traps)
            {
                Rectangle trapKillZone = new Rectangle(
                    trap.X - 3,
                    trap.Y - 7,
                    trap.Width + 6,
                    trap.Height + 10);

                if (playerBounds.IntersectsWith(trapKillZone))
                    return true;
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