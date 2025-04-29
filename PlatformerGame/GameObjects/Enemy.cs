// Enemy.cs
using System.Drawing;

namespace PlatformerGame.GameObjects
{
    /// <summary>
    /// Представляет вражеского персонажа в платформенной игре.
    /// </summary>
    public class Enemy
    {
        /// <summary>
        /// Возвращает прямоугольные границы врага.
        /// </summary>
        /// <value>
        /// <see cref="Rectangle"/>, представляющий позицию и размер врага.
        /// </value>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Диапазон, в пределах которого враг может двигаться.
        /// </summary>
        private readonly int _moveRange;

        /// <summary>
        /// Скорость, с которой двигается враг.
        /// </summary>
        private readonly int _speed;

        /// <summary>
        /// Направление, в котором враг в данный момент двигается (1 - вправо, -1 - влево).
        /// </summary>
        private int _direction = 1;

        /// <summary>
        /// Исходная X-координата врага, используемая в качестве точки отсчета для диапазона движения.
        /// </summary>
        private readonly int _originalX;

        /// <summary>
        /// Y-координата платформы, на которой стоит враг.
        /// </summary>
        private readonly int _platformY;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Enemy"/>.
        /// </summary>
        /// <param name="x">Начальная X-координата врага.</param>
        /// <param name="platformY">Y-координата платформы, на которой стоит враг.</param>
        /// <param name="width">Ширина врага.</param>
        /// <param name="height">Высота врага.</param>
        /// <param name="moveRange">Диапазон, в пределах которого враг может двигаться по горизонтали.</param>
        /// <param name="speed">Скорость, с которой двигается враг.</param>
        public Enemy(int x, int platformY, int width, int height, int moveRange, int speed)
        {
            _platformY = platformY;
            Bounds = new Rectangle(x, platformY - height, width, height);
            _moveRange = moveRange;
            _speed = speed;
            _originalX = x;
        }

        /// <summary>
        /// Обновляет позицию врага на основе его скорости и направления, удерживая его в пределах заданного диапазона движения.
        /// </summary>
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

        /// <summary>
        /// Рисует врага на экране.
        /// </summary>
        /// <param name="g">Объект <see cref="Graphics"/>, используемый для рисования.</param>
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