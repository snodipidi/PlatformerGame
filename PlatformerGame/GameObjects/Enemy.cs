using System.Drawing;
using System.Drawing.Drawing2D;

namespace PlatformerGame.GameObjects
{
    public class Enemy
    {
        public Rectangle Bounds { get; private set; }
        private readonly int _moveRange;
        private readonly int _speed;
        private int _direction = 1;
        private readonly int _originalX;
        private Bitmap _sprite;
        private const int Width = 45;  // Ширина после масштабирования
        private const int Height = 70; // Высота после масштабирования

        public Enemy(int x, int platformTopY, int moveRange, int speed)
        {
            try
            {
                _sprite = new Bitmap("C:\\Users\\msmil\\source\\repos\\PlatformerGame\\PlatformerGame\\Resourses\\skeleton.png");
            }
            catch
            {
                _sprite = null;
            }

            Bounds = new Rectangle(x, platformTopY - Height, Width, Height);
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

            Bounds = new Rectangle(newX, Bounds.Y, Width, Height);
        }

        public void Draw(Graphics g)
        {
            if (_sprite == null)
            {
                g.FillRectangle(Brushes.Red, Bounds);
                return;
            }

            var oldMode = g.InterpolationMode;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (_direction < 0) // Отражаем спрайт при движении влево
            {
                g.DrawImage(_sprite,
                    new Rectangle(Bounds.X + Width, Bounds.Y, -Width, Height),
                    new Rectangle(0, 0, _sprite.Width, _sprite.Height),
                    GraphicsUnit.Pixel);
            }
            else
            {
                g.DrawImage(_sprite, Bounds,
                    new Rectangle(0, 0, _sprite.Width, _sprite.Height),
                    GraphicsUnit.Pixel);
            }

            g.InterpolationMode = oldMode;
        }
    }
}