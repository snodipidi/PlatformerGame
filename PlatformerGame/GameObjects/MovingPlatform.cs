using System.Drawing;

namespace PlatformerGame.GameObjects
{
    /// <summary>
    /// Представляет движущуюся платформу на уровне.
    /// </summary>
    public class MovingPlatform
    {
        // Границы платформы (позиция и размер)
        public Rectangle Bounds { get; private set; }
        // Дальность, на которую платформа может двигаться от начальной точки
        private readonly int _moveRange;
        // Скорость движения платформы
        private readonly int _speed;
        // Направление движения (1 или -1)
        private int _direction = 1;
        // Начальная координата X
        private readonly int _startX;
        // Начальная координата Y
        private readonly int _startY;
        // Флаг вертикального движения (если false — горизонтальное)
        private readonly bool _isVertical;

        /// <summary>
        /// Конструктор платформы.
        /// </summary>
        /// <param name="x">Начальная координата X.</param>
        /// <param name="y">Начальная координата Y.</param>
        /// <param name="width">Ширина платформы.</param>
        /// <param name="height">Высота платформы.</param>
        /// <param name="moveRange">Максимальное расстояние, на которое платформа может двигаться.</param>
        /// <param name="speed">Скорость движения платформы.</param>
        /// <param name="isVertical">Флаг вертикального движения.</param>
        public MovingPlatform(int x, int y, int width, int height,
                              int moveRange, int speed, bool isVertical)
        {
            // Сохраняем начальные координаты
            _startX = x;
            _startY = y;
            // Устанавливаем прямоугольник платформы
            Bounds = new Rectangle(x, y, width, height);
            // Сохраняем параметры движения
            _moveRange = moveRange;
            _speed = speed;
            _isVertical = isVertical;
        }

        /// <summary>
        /// Обновляет позицию платформы в зависимости от направления и типа движения.
        /// </summary>
        public void Update()
        {
            // Если движение вертикальное
            if (_isVertical)
            {
                // Вычисляем новое значение Y
                int newY = Bounds.Y + _speed * _direction;
                // Если достигнут предел движения — меняем направление
                if (newY > _startY + _moveRange || newY < _startY - _moveRange)
                    _direction *= -1;
                // Обновляем позицию платформы
                Bounds = new Rectangle(Bounds.X, newY, Bounds.Width, Bounds.Height);
            }
            else
            {
                // Вычисляем новое значение X
                int newX = Bounds.X + _speed * _direction;
                // Если достигнут предел движения — меняем направление
                if (newX > _startX + _moveRange || newX < _startX - _moveRange)
                    _direction *= -1;
                // Обновляем позицию платформы
                Bounds = new Rectangle(newX, Bounds.Y, Bounds.Width, Bounds.Height);
            }
        }

        /// <summary>
        /// Отрисовывает платформу на экране.
        /// </summary>
        /// <param name="g">Объект Graphics для рисования.</param>
        public void Draw(Graphics g)
        {
            // Создаём полупрозрачную кисть для закрашивания платформы
            using (var brush = new SolidBrush(Color.FromArgb(150, 100, 200, 100)))
            {
                // Заполняем прямоугольник платформы
                g.FillRectangle(brush, Bounds);
            }
            // Рисуем контур платформы
            g.DrawRectangle(Pens.DarkGreen, Bounds);
        }
    }
}
