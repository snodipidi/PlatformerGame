using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PlatformerGame.GameObjects
{
    public class Player : IDisposable
    {
        // Размеры персонажа (как в оригинале)
        private const int Width = 29;
        private const int Height = 41;

        // Спрайты
        private Bitmap _standingSprite;
        private Bitmap _walkingSprite;
        private bool _showWalkingSprite = false;
        private int _animationCounter = 0;
        private const int AnimationDelay = 10;
        private Level _level;


        // Физика (без изменений)
        public PointF Position { get; private set; }
        public bool IsMovingLeft { get; private set; }
        public bool IsMovingRight { get; private set; }
        private const float Speed = 5;
        private float _verticalVelocity = 0;
        private const float Gravity = 0.5f;
        private const float JumpForce = 12f;
        public float GetProgressX() => Position.X;

        public Player(Rectangle startPlatform, Level level)
        {
            _level = level; // Инициализация уровня
            try
            {
                _standingSprite = new Bitmap("Resourses\\player.png");
                _walkingSprite = new Bitmap("Resourses\\player_an.png");
            }
            catch
            {
                // Обработка ошибок загрузки спрайтов
            }
            Reset(startPlatform);
        }


        public void Reset(Rectangle startPlatform)
        {
            Position = new PointF(50, startPlatform.Y - Height);
            IsMovingLeft = false;
            IsMovingRight = false;
            _verticalVelocity = 0;
            _showWalkingSprite = false;
            _animationCounter = 0;
        }

        public void Update()
        {
            var allPlatforms = new List<Rectangle>(_level.Platforms);
            allPlatforms.AddRange(_level.MovingPlatforms.Select(mp => mp.Bounds));

            Position = new PointF(
                Position.X + (IsMovingLeft ? -Speed : IsMovingRight ? Speed : 0),
                Position.Y + _verticalVelocity);

            _verticalVelocity += Gravity;

            var feet = new RectangleF(
                Position.X + 5,
                Position.Y + Height,
                Width - 10,
                1);

            foreach (var platform in allPlatforms)
            {
                if (feet.IntersectsWith(platform) && _verticalVelocity > 0)
                {
                    Position = new PointF(Position.X, platform.Y - Height);
                    _verticalVelocity = 0;
                    break;
                }
            }

            if (IsMovingLeft || IsMovingRight)
            {
                _animationCounter++;
                if (_animationCounter >= AnimationDelay)
                {
                    _animationCounter = 0;
                    _showWalkingSprite = !_showWalkingSprite;
                }
            }
            else
            {
                _showWalkingSprite = false;
            }
        }

        public void Draw(Graphics g)
        {
            Bitmap currentSprite = _showWalkingSprite ? _walkingSprite : _standingSprite;

            if (currentSprite == null) return;

            if (IsMovingLeft)
            {
                // Отражаем по горизонтали
                g.DrawImage(currentSprite,
                    new RectangleF(Position.X + Width, Position.Y, -Width, Height),
                    new RectangleF(0, 0, currentSprite.Width, currentSprite.Height),
                    GraphicsUnit.Pixel);
            }
            else
            {
                g.DrawImage(currentSprite,
                    new RectangleF(Position.X, Position.Y, Width, Height),
                    new RectangleF(0, 0, currentSprite.Width, currentSprite.Height),
                    GraphicsUnit.Pixel);
            }
        }

        // Остальные методы без изменений
        public void StartMovingLeft() => IsMovingLeft = true;
        public void StartMovingRight() => IsMovingRight = true;
        public void StopMovingLeft() => IsMovingLeft = false;
        public void StopMovingRight() => IsMovingRight = false;

        public void Jump()
        {
            if (_verticalVelocity == 0)
                _verticalVelocity = -JumpForce;
        }

        public bool HasFallen(int screenHeight) => Position.Y > screenHeight;

        public void Dispose()
        {
            _standingSprite?.Dispose();
            _walkingSprite?.Dispose();
        }

        public RectangleF GetBounds()
        {
            return new RectangleF(Position.X, Position.Y, Width, Height);
        }
    }
}