// Enemy.cs
using System.Drawing;

namespace PlatformerGame.GameObjects
{
    public class Enemy
    {
        public Rectangle Bounds { get; private set; }
        private readonly int _moveRange;
        private readonly int _speed;
        private int _direction = 1;
        private readonly int _originalX;
        private readonly int _platformY; // Y-координата платформы

        public Enemy(int x, int platformY, int width, int height, int moveRange, int speed)
        {
            _platformY = platformY;
            Bounds = new Rectangle(x, platformY - height, width, height);
            _moveRange = moveRange;
            _speed = speed;
            _originalX = x;
        }

        public void Update()
        {
            // Движение врага в пределах диапазона
            int newX = Bounds.X + _speed * _direction;

            if (newX > _originalX + _moveRange || newX < _originalX)
            {
                _direction *= -1; // Меняем направление
                newX = Bounds.X + _speed * _direction;
            }

            Bounds = new Rectangle(newX, _platformY - Bounds.Height, Bounds.Width, Bounds.Height);
        }

        public void Draw(Graphics g)
        {
            // Тело врага
            g.FillRectangle(Brushes.Red, Bounds);

            // Глаза (чтобы видеть направление движения)
            int eyeOffset = _direction > 0 ? 5 : Bounds.Width - 15;
            g.FillEllipse(Brushes.White, Bounds.X + eyeOffset, Bounds.Y + 5, 10, 10);
            g.FillEllipse(Brushes.Black, Bounds.X + eyeOffset + 3, Bounds.Y + 8, 4, 4);
        }
    }
}