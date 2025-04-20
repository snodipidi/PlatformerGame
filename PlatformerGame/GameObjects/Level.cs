using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace PlatformerGame.GameObjects
{
    public class Level
    {
        public Rectangle StartPlatform { get; }
        public List<Rectangle> Platforms { get; } = new List<Rectangle>();
        public int CameraOffset { get; set; } // Изменено на публичный set
        public Rectangle FinishFlag { get; private set; }
        public bool IsLevelCompleted { get; private set; }
        public int TotalLength { get; private set; }
        public Bitmap FinishFlagTexture => _finishFlagTexture; 

        private readonly Random random = new Random();
        private int lastPlatformX;
        private readonly Size screenSize;
        private readonly Bitmap _blockTexture;
        private readonly Bitmap _finishFlagTexture;
        private const int FinishAreaWidth = 300;
        public float StartPosition => StartPlatform.X + 50;
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

        public Level(Size screenSize)
        {
            try
            {
                _blockTexture = new Bitmap("C:\\Users\\msmil\\source\\repos\\PlatformerGame\\PlatformerGame\\Resourses\\block.png");
                _finishFlagTexture = new Bitmap("C:\\Users\\msmil\\source\\repos\\PlatformerGame\\PlatformerGame\\Resourses\\finish_flag.png");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки текстур: {ex.Message}");
                _blockTexture = null;
                _finishFlagTexture = null;
            }

            this.screenSize = screenSize;

            // Начальная платформа
            StartPlatform = new Rectangle(0, screenSize.Height - 100, 300, 20);
            Platforms.Add(StartPlatform);
            lastPlatformX = StartPlatform.Right;

            // Определяем длину уровня (примерно 3 экрана)
            TotalLength = screenSize.Width * 3;

            // Генерируем платформы до конца уровня
            while (lastPlatformX < TotalLength - FinishAreaWidth)
            {
                GeneratePlatform();
            }

            // Зона финиша с несколькими платформами
            GenerateFinalPlatforms();

            // Создаем финишный флажок
            CreateFinishFlag();
        }

        private void GenerateFinalPlatforms()
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
                lastPlatform.X + lastPlatform.Width / 2 - 15,
                lastPlatform.Y - 60,
                30,
                60);
        }

        private void GeneratePlatform()
        {
            int width = random.Next(80, 150);
            int x = lastPlatformX + random.Next(100, 250);
            int y = random.Next(screenSize.Height / 2, screenSize.Height - 50);
            Platforms.Add(new Rectangle(x, y, width, 20));
            lastPlatformX = x + width;
        }

        public void Update(float playerX)
        {
            // Камера следует за игроком с отступом в 1/3 экрана
            CameraOffset = (int)(playerX - screenSize.Width / 3);

            // Ограничиваем камеру границами уровня
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