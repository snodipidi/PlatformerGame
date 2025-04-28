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

        public Enemy(int x, int y, int width, int height, int moveRange, int speed)
        {
            Bounds = new Rectangle(x, y, width, height);
            _moveRange = moveRange;
            _speed = speed;
            _originalX = x;
        }

        public void Update()
        {
            int newX = Bounds.X + _speed * _direction;

            if (newX > _originalX + _moveRange || newX < _originalX)
            {
                _direction *= -1; 
                newX = Bounds.X + _speed * _direction;
            }

            Bounds = new Rectangle(newX, Bounds.Y, Bounds.Width, Bounds.Height);
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Red, Bounds);
            g.FillEllipse(Brushes.White, Bounds.X + 5, Bounds.Y + 5, 10, 10);
            g.FillEllipse(Brushes.White, Bounds.X + Bounds.Width - 15, Bounds.Y + 5, 10, 10);
            g.FillEllipse(Brushes.Black, Bounds.X + 8, Bounds.Y + 8, 4, 4);
            g.FillEllipse(Brushes.Black, Bounds.X + Bounds.Width - 12, Bounds.Y + 8, 4, 4);
        }
    }
}