using System.Drawing;

namespace PlatformerGame.GameObjects
{
    public class MovingPlatform
    {
        public Rectangle Bounds { get; private set; }
        private readonly int _moveRange;
        private readonly int _speed;
        private int _direction = 1;
        private readonly int _startX;
        private readonly int _startY;
        private readonly bool _isVertical;

        public MovingPlatform(int x, int y, int width, int height,
                            int moveRange, int speed, bool isVertical)
        {
            _startX = x;
            _startY = y;
            Bounds = new Rectangle(x, y, width, height);
            _moveRange = moveRange;
            _speed = speed;
            _isVertical = isVertical;
        }

        public void Update()
        {
            if (_isVertical)
            {
                int newY = Bounds.Y + _speed * _direction;
                if (newY > _startY + _moveRange || newY < _startY - _moveRange)
                    _direction *= -1;
                Bounds = new Rectangle(Bounds.X, newY, Bounds.Width, Bounds.Height);
            }
            else
            {
                int newX = Bounds.X + _speed * _direction;
                if (newX > _startX + _moveRange || newX < _startX - _moveRange)
                    _direction *= -1;
                Bounds = new Rectangle(newX, Bounds.Y, Bounds.Width, Bounds.Height);
            }
        }

        public void Draw(Graphics g)
        {
            using (var brush = new SolidBrush(Color.FromArgb(150, 100, 200, 100)))
            {
                g.FillRectangle(brush, Bounds);
            }
            g.DrawRectangle(Pens.DarkGreen, Bounds);
        }
    }
}