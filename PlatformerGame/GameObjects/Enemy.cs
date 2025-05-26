using System.Drawing;
using System.Drawing.Drawing2D;

namespace PlatformerGame.GameObjects
{
    /// <summary>
    /// Класс, представляющий врага в платформере
    /// </summary>
    public class Enemy
    {
        /// <summary>
        /// Границы врага (позиция и размер)
        /// </summary>
        public Rectangle Bounds { get; private set; }
        // Максимальная дистанция перемещения от начальной позиции
        private readonly int _moveRange;
        // Скорость перемещения врага (пикселей за кадр)
        private readonly int _speed;
        // Направление движения (1 - вправо, -1 - влево)
        private int _direction = 1;
        // Исходная X-координата позиции врага
        private readonly int _originalX;
        // Спрайт для отрисовки врага
        private Bitmap _sprite;
        // Ширина врага после масштабирования
        private const int Width = 45;
        // Высота врага после масштабирования
        private const int Height = 70;

        /// <summary>
        /// Создает нового врага
        /// </summary>
        /// <param name="x">Начальная X-координата</param>
        /// <param name="platformTopY">Y-координата верхнего края платформы</param>
        /// <param name="moveRange">Дистанция перемещения</param>
        /// <param name="speed">Скорость перемещения</param>
        public Enemy(int x, int platformTopY, int moveRange, int speed)
        {
            // Пытаемся загрузить спрайт из файла
            try
            {
                _sprite = new Bitmap("Resourses\\skeleton.png");
            }
            catch
            {
                // Если загрузка не удалась, используем null
                _sprite = null;
            }
            // Устанавливаем начальную позицию (над платформой)
            Bounds = new Rectangle(x, platformTopY - Height, Width, Height);
            // Сохраняем параметры движения
            _moveRange = moveRange;
            _speed = speed;
            _originalX = x;
        }

        /// <summary>
        /// Обновляет позицию врага
        /// </summary>
        public void Update()
        {
            // Вычисляем новую позицию по X
            int newX = Bounds.X + _speed * _direction;
            // Проверяем достижение границ движения
            if (newX > _originalX + _moveRange || newX < _originalX)
            {
                // Меняем направление движения
                _direction *= -1;
                // Корректируем позицию
                newX = Bounds.X + _speed * _direction;
            }
            // Обновляем границы врага
            Bounds = new Rectangle(newX, Bounds.Y, Width, Height);
        }

        /// <summary>
        /// Отрисовывает врага
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки</param>
        public void Draw(Graphics g)
        {
            // Если спрайт не загружен, рисуем красный прямоугольник
            if (_sprite == null)
            {
                g.FillRectangle(Brushes.Red, Bounds);
                return;
            }
            // Сохраняем текущий режим интерполяции
            var oldMode = g.InterpolationMode;
            // Устанавливаем режим NearestNeighbor для сохранения пиксельности
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            // Если враг движется влево, отражаем спрайт
            if (_direction < 0)
            {
                // Отрисовываем отраженный спрайт
                g.DrawImage(_sprite,
                    new Rectangle(Bounds.X + Width, Bounds.Y, -Width, Height),
                    new Rectangle(0, 0, _sprite.Width, _sprite.Height),
                    GraphicsUnit.Pixel);
            }
            else
            {
                // Отрисовываем спрайт в обычном виде
                g.DrawImage(_sprite, Bounds,
                    new Rectangle(0, 0, _sprite.Width, _sprite.Height),
                    GraphicsUnit.Pixel);
            }
            // Восстанавливаем исходный режим интерполяции
            g.InterpolationMode = oldMode;
        }
    }
}