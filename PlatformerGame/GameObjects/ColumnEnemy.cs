using System.Drawing;

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
            // Отрисовка колонны
            g.FillRectangle(Brushes.DarkRed, Bounds);

            // Опасная зона (верхняя часть)
            g.FillRectangle(Brushes.OrangeRed,
                Bounds.X, Bounds.Y - 5,
                Bounds.Width, _killZoneHeight + 5);

            // Полосы для лучшей видимости
            for (int y = Bounds.Y; y < Bounds.Bottom; y += 10)
            {
                g.DrawLine(Pens.Black, Bounds.Left, y, Bounds.Right, y);
            }
        }
    }
}