using System.Drawing;
using System.Drawing.Drawing2D;

namespace PlatformerGame.GameObjects
{
    /// <summary>
    /// Представляет вертикально движущегося врага в виде опасной колонны с шипами
    /// </summary>
    public class ColumnEnemy
    {
        /// <summary>
        /// Границы колонны (позиция и размер)
        /// </summary>
        public Rectangle Bounds { get; private set; }
        // Максимальная дистанция движения вверх от начальной позиции
        private readonly int _moveRange;
        // Скорость движения колонны (пикселей за кадр)
        private readonly int _speed;
        // Направление движения (1 - вниз, -1 - вверх)
        private int _direction = 1;
        // Y-координата платформы, к которой привязана колонна
        private readonly int _platformY;
        // X-координата платформы, к которой привязана колонна
        private readonly int _platformX;
        // Высота дополнительной опасной зоны сверху колонны (зона шипов)
        private readonly int _killZoneHeight = 15;

        /// <summary>
        /// Создает новый экземпляр движущейся колонны-врага
        /// </summary>
        public ColumnEnemy(int platformX, int platformY, int width, int height, int moveRange, int speed)
        {
            // Сохраняем координаты платформы для привязки движения
            _platformX = platformX;
            _platformY = platformY;
            // Инициализируем начальную позицию колонны (на 10 пикселей выше платформы)
            Bounds = new Rectangle(platformX, platformY - height - 10, width, height);
            // Устанавливаем параметры движения
            _moveRange = moveRange;
            _speed = speed;
        }

        /// <summary>
        /// Возвращает опасную зону, при контакте с которой игрок погибает
        /// </summary>
        public Rectangle GetKillZone()
        {
            // Возвращаем прямоугольник, объединяющий саму колонну и зону шипов сверху
            return new Rectangle(
                Bounds.X,
                Bounds.Y - _killZoneHeight,
                Bounds.Width,
                Bounds.Height + _killZoneHeight
            );
        }

        /// <summary>
        /// Обновляет позицию колонны
        /// </summary>
        public void Update()
        {
            // Рассчитываем новую позицию по Y
            int newY = Bounds.Y + _speed * _direction;
            // Проверяем достижение границ движения
            if (newY > _platformY - Bounds.Height ||
                newY < _platformY - Bounds.Height - _moveRange)
            {
                // Меняем направление движения
                _direction *= -1;
                // Корректируем позицию
                newY = Bounds.Y + _speed * _direction;
            }
            // Обновляем позицию колонны (X остается неизменным)
            Bounds = new Rectangle(_platformX, newY, Bounds.Width, Bounds.Height);
        }

        /// <summary>
        /// Отрисовывает колонну с эффектами
        /// </summary>
        public void Draw(Graphics g)
        {
            // Эффект свечения вокруг колонны (красноватый ореол)
            using (var glowBrush = new SolidBrush(Color.FromArgb(50, 255, 100, 100)))
            {
                // Рисуем прямоугольник свечения (больше основной колонны на 5px с каждой стороны)
                g.FillRectangle(glowBrush,
                    Bounds.X - 5, Bounds.Y - 5,
                    Bounds.Width + 10, Bounds.Height + 10);
            }
            // Основное тело колонны с вертикальным градиентом
            using (var brush = new LinearGradientBrush(
                Bounds,
                Color.FromArgb(200, 150, 0, 0),
                Color.FromArgb(200, 100, 0, 0),
                90f))
            {
                // Заливаем колонну градиентом
                g.FillRectangle(brush, Bounds);
            }
            // Горизонтальные полосы для создания металлического эффекта
            using (var whitePen = new Pen(Color.FromArgb(150, 255, 255, 255)))
            {
                // Рисуем линии через каждые 15 пикселей по высоте колонны
                for (int y = Bounds.Y; y < Bounds.Bottom; y += 15)
                {
                    g.DrawLine(whitePen, Bounds.Left, y, Bounds.Right, y);
                }
            }
            // Основание опасной зоны (шипы) - прямоугольная подложка
            Rectangle dangerZone = new Rectangle(
                Bounds.X - 3,
                Bounds.Y - 15,
                Bounds.Width + 6,
                15);
            // Заливаем основание шипов градиентом
            using (var spikeBrush = new LinearGradientBrush(
                dangerZone,
                Color.OrangeRed,
                Color.DarkRed,
                0f))
            {
                g.FillRectangle(spikeBrush, dangerZone);
            }
            // Отрисовка треугольных шипов
            int spikeCount = 5;
            int spikeWidth = dangerZone.Width / spikeCount;
            // Кисть и контур для рисования шипов
            using (var spikeFill = new SolidBrush(Color.OrangeRed))
            using (var spikeBorder = new Pen(Color.DarkRed, 2))
            {
                // Рисуем каждый шип
                for (int i = 0; i < spikeCount; i++)
                {
                    // Вершины треугольника (шипа)
                    Point[] spike = {
                        new Point(dangerZone.X + i * spikeWidth, dangerZone.Bottom),
                        new Point(dangerZone.X + (i + 1) * spikeWidth, dangerZone.Bottom),
                        new Point(dangerZone.X + i * spikeWidth + spikeWidth/2, dangerZone.Top)
                    };
                    // Рисуем и заливаем шип
                    g.FillPolygon(spikeFill, spike);
                    g.DrawPolygon(spikeBorder, spike);
                }
            }
            // Толстый контур вокруг колонны
            using (var borderPen = new Pen(Color.DarkRed, 3))
            {
                g.DrawRectangle(borderPen, Bounds);
            }
        }
    }
}