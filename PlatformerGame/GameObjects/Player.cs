using System.Collections.Generic;
using System.Drawing;

namespace PlatformerGame.GameObjects
{
    public class Player
    {
        private readonly Bitmap sprite;
        public PointF Position { get; private set; }
        public bool IsMovingLeft { get; private set; }
        public bool IsMovingRight { get; private set; }

        private const int Width = 29;
        private const int Height = 41;
        private const float Speed = 5;
        private float verticalVelocity = 0;
        private const float Gravity = 0.5f;
        private const float JumpForce = 12f;

        public Player(Rectangle startPlatform)
        {
            sprite = new Bitmap("player.png");
            Reset(startPlatform);
        }

        public void Reset(Rectangle startPlatform)
        {
            Position = new PointF(50, startPlatform.Y - Height);
            IsMovingLeft = false;
            IsMovingRight = false;
            verticalVelocity = 0;
        }

        public void Update(List<Rectangle> platforms)
        {
            Position = new PointF(
                Position.X + (IsMovingLeft ? -Speed : IsMovingRight ? Speed : 0),
                Position.Y + verticalVelocity);

            verticalVelocity += Gravity;

            var feet = new RectangleF(
                Position.X + 5,
                Position.Y + Height,
                Width - 10,
                1
            );

            foreach (var platform in platforms)
            {
                if (feet.IntersectsWith(platform) && verticalVelocity > 0)
                {
                    Position = new PointF(Position.X, platform.Y - Height);
                    verticalVelocity = 0;
                    break;
                }
            }
        }

        public void Draw(Graphics g)
        {
            if (sprite == null) return;

            if (IsMovingLeft)
            {
                g.DrawImage(sprite,
                    new RectangleF(Position.X + Width, Position.Y, -Width, Height),
                    new RectangleF(0, 0, sprite.Width, sprite.Height),
                    GraphicsUnit.Pixel);
            }
            else
            {
                g.DrawImage(sprite, Position.X, Position.Y, Width, Height);
            }
        }

        public void StartMovingLeft() => IsMovingLeft = true;
        public void StartMovingRight() => IsMovingRight = true;
        public void StopMovingLeft() => IsMovingLeft = false;
        public void StopMovingRight() => IsMovingRight = false;

        public void Jump()
        {
            if (verticalVelocity == 0)
                verticalVelocity = -JumpForce;
        }

        public bool HasFallen(int screenHeight) => Position.Y > screenHeight;
    }
}