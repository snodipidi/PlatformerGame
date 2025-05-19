using System.Drawing;

namespace PlatformerGame.GameObjects
{
    public abstract class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; }
        public int Height { get; }
        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        protected GameObject(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public abstract void Update();
        public abstract void Draw(Graphics g);
        public abstract void HandleCollision(GameObject other);
    }
} 