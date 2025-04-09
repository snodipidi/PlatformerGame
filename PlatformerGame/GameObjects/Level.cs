using System;
using System.Collections.Generic;
using System.Drawing;

namespace PlatformerGame.GameObjects
{
    public class Level
    {
        public Rectangle StartPlatform { get; }
        public List<Rectangle> Platforms { get; } = new List<Rectangle>();
        public int CameraOffset { get; private set; }

        private readonly Random random = new Random();
        private int lastPlatformX;
        private readonly Size screenSize;

        public Level(Size screenSize)
        {
            this.screenSize = screenSize;
            StartPlatform = new Rectangle(0, screenSize.Height - 100, 300, 20);
            Platforms.Add(StartPlatform);
            lastPlatformX = StartPlatform.Right;

            for (int i = 0; i < 10; i++)
                GeneratePlatform();
        }

        public void Update(float playerX)
        {
            CameraOffset = (int)playerX - screenSize.Width / 2;
            if (CameraOffset < 0) CameraOffset = 0;

            while (lastPlatformX < CameraOffset + screenSize.Width + 200)
                GeneratePlatform();

            Platforms.RemoveAll(p => p.Right < CameraOffset - 100);
        }

        private void GeneratePlatform()
        {
            int width = random.Next(80, 150);
            int x = lastPlatformX + random.Next(100, 250);
            int y = random.Next(screenSize.Height / 2, screenSize.Height - 50);
            Platforms.Add(new Rectangle(x, y, width, 20));
            lastPlatformX = x + width;
        }

        public void Draw(Graphics g)
        {
            foreach (var platform in Platforms)
            {
                try
                {
                    g.FillRectangle(Brushes.Green, platform);
                }
                catch { /* игнорируем ошибки отрисовки */ }
            }
        }
    }
}