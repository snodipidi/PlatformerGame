using System.Drawing;
using System.Drawing.Drawing2D;

namespace PlatformerGame.GameObjects
{
    public class ColumnEnemy
    {
        public Rectangle Bounds { get; private set; }
        private readonly int _moveRange;
        private readonly int _speed;
        private int _direction = 1;
        private readonly int _platformY;
        private readonly int _platformX;
        private readonly int _killZoneHeight = 15; // Дополнительная опасная зона

        public ColumnEnemy(int platformX, int platformY, int width, int height, int moveRange, int speed)
        {
            _platformX = platformX;
            _platformY = platformY;
            // Стартовая позиция - выше платформы
            Bounds = new Rectangle(platformX, platformY - height - 10, width, height);
            _moveRange = moveRange;
            _speed = speed;
        }

        public Rectangle GetKillZone()
        {
            // Возвращаем зону, которая убивает игрока (включая верхнюю часть)
            return new Rectangle(
                Bounds.X,
                Bounds.Y - _killZoneHeight,
                Bounds.Width,
                Bounds.Height + _killZoneHeight);
        }

        public void Update()
        {
            int newY = Bounds.Y + _speed * _direction;

            // Движение между верхней точкой и платформой
            if (newY > _platformY - Bounds.Height || newY < _platformY - Bounds.Height - _moveRange)
            {
                _direction *= -1;
                newY = Bounds.Y + _speed * _direction;
            }

            Bounds = new Rectangle(_platformX, newY, Bounds.Width, Bounds.Height);
        }

        public void Draw(Graphics g)
        {
            // 1. Фоновая подсветка
            using (var glowBrush = new SolidBrush(Color.FromArgb(50, 255, 100, 100)))
            {
                g.FillRectangle(glowBrush,
                    Bounds.X - 5, Bounds.Y - 5,
                    Bounds.Width + 10, Bounds.Height + 10);
            }

            // 2. Основное тело колонны (градиент)
            using (var brush = new LinearGradientBrush(
                Bounds,
                Color.FromArgb(200, 150, 0, 0),
                Color.FromArgb(200, 100, 0, 0),
                90f))
            {
                g.FillRectangle(brush, Bounds);
            }

            // 3. Металлические полосы
            using (var whitePen = new Pen(Color.FromArgb(150, 255, 255, 255)))
            {
                for (int y = Bounds.Y; y < Bounds.Bottom; y += 15)
                {
                    g.DrawLine(whitePen, Bounds.Left, y, Bounds.Right, y);
                }
            }

            // 4. Опасная зона (шипы)
            Rectangle dangerZone = new Rectangle(
                Bounds.X - 3,
                Bounds.Y - 15,
                Bounds.Width + 6,
                15);

            using (var spikeBrush = new LinearGradientBrush(
                dangerZone,
                Color.OrangeRed,
                Color.DarkRed,
                0f))
            {
                g.FillRectangle(spikeBrush, dangerZone);
            }

            // 5. Шипы (треугольники)
            int spikeCount = 5;
            int spikeWidth = dangerZone.Width / spikeCount;

            using (var spikeFill = new SolidBrush(Color.OrangeRed))
            using (var spikeBorder = new Pen(Color.DarkRed, 2))
            {
                for (int i = 0; i < spikeCount; i++)
                {
                    Point[] spike = {
                new Point(dangerZone.X + i * spikeWidth, dangerZone.Bottom),
                new Point(dangerZone.X + (i + 1) * spikeWidth, dangerZone.Bottom),
                new Point(dangerZone.X + i * spikeWidth + spikeWidth/2, dangerZone.Top)
            };
                    g.FillPolygon(spikeFill, spike);
                    g.DrawPolygon(spikeBorder, spike);
                }
            }

            // 6. Контур
            using (var borderPen = new Pen(Color.DarkRed, 3))
            {
                g.DrawRectangle(borderPen, Bounds);
            }
        }
    }
}