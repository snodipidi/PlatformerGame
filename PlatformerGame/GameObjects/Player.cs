using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PlatformerGame.GameObjects
{
    /// <summary>
    /// Класс, представляющий игрока в платформере.
    /// Обрабатывает физику, анимацию и отрисовку персонажа.
    /// </summary>
    public class Player : IDisposable
    {
        // Ширина и высота игрока в пикселях
        private const int Width = 29;
        private const int Height = 41;

        // Текущий спрайт стоящего игрока
        private Bitmap _standingSprite;

        // Спрайт анимации ходьбы
        private Bitmap _walkingSprite;

        // Флаг показа анимационного спрайта
        private bool _showWalkingSprite = false;

        // Счётчик кадров для смены спрайта
        private int _animationCounter = 0;

        // Задержка между кадрами анимации
        private const int AnimationDelay = 10;

        // Уровень, в котором находится игрок
        private Level _level;

        // Текущая позиция игрока
        public PointF Position { get; private set; }

        // Флаг движения влево
        public bool IsMovingLeft { get; private set; }

        // Флаг движения вправо
        public bool IsMovingRight { get; private set; }

        // Скорость перемещения по горизонтали
        private const float Speed = 5;

        // Вертикальная скорость (гравитация, прыжок)
        private float _verticalVelocity = 0;

        // Константа силы гравитации
        private const float Gravity = 0.5f;

        // Сила прыжка
        private const float JumpForce = 12f;

        /// <summary>
        /// Возвращает координату X игрока для определения прогресса уровня.
        /// </summary>
        public float GetProgressX() => Position.X;

        /// <summary>
        /// Конструктор игрока. Загружает спрайты и инициализирует начальное состояние.
        /// </summary>
        /// <param name="startPlatform">Платформа, на которой появляется игрок.</param>
        /// <param name="level">Уровень, к которому относится игрок.</param>
        public Player(Rectangle startPlatform, Level level)
        {
            _level = level; // Сохраняем ссылку на уровень

            try
            {
                // Загружаем спрайты игрока
                _standingSprite = new Bitmap("Resourses\\player.png");
                _walkingSprite = new Bitmap("Resourses\\player_an.png");
            }
            catch
            {
                // Игнорируем ошибки загрузки (например, если файл не найден)
            }

            // Устанавливаем стартовую позицию
            Reset(startPlatform);
        }

        /// <summary>
        /// Сброс состояния игрока до начального.
        /// </summary>
        /// <param name="startPlatform">Платформа, на которой появляется игрок.</param>
        public void Reset(Rectangle startPlatform)
        {
            // Устанавливаем позицию чуть выше платформы
            Position = new PointF(50, startPlatform.Y - Height);
            IsMovingLeft = false;
            IsMovingRight = false;
            _verticalVelocity = 0;
            _showWalkingSprite = false;
            _animationCounter = 0;
        }

        /// <summary>
        /// Обновляет позицию игрока, физику и анимацию.
        /// </summary>
        public void Update()
        {
            // Получаем все платформы (в том числе движущиеся)
            var allPlatforms = new List<Rectangle>(_level.Platforms);
            allPlatforms.AddRange(_level.MovingPlatforms.Select(mp => mp.Bounds));

            // Вычисляем новое положение игрока
            Position = new PointF(
                Position.X + (IsMovingLeft ? -Speed : IsMovingRight ? Speed : 0),
                Position.Y + _verticalVelocity);

            // Применяем гравитацию
            _verticalVelocity += Gravity;

            // Прямоугольник ног игрока (для проверки соприкосновения с платформами)
            var feet = new RectangleF(
                Position.X + 5,
                Position.Y + Height,
                Width - 10,
                1);

            // Проверка соприкосновения с платформами
            foreach (var platform in allPlatforms)
            {
                if (feet.IntersectsWith(platform) && _verticalVelocity > 0)
                {
                    // Если игрок "на земле", сбрасываем вертикальную скорость
                    Position = new PointF(Position.X, platform.Y - Height);
                    _verticalVelocity = 0;
                    break;
                }
            }

            // Логика анимации при движении
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

        /// <summary>
        /// Отрисовывает игрока на экране.
        /// </summary>
        /// <param name="g">Контекст отрисовки.</param>
        public void Draw(Graphics g)
        {
            // Выбираем текущий спрайт
            Bitmap currentSprite = _showWalkingSprite ? _walkingSprite : _standingSprite;

            // Если спрайт не загружен — ничего не рисуем
            if (currentSprite == null) return;

            if (IsMovingLeft)
            {
                // Отражаем изображение по горизонтали при движении влево
                g.DrawImage(currentSprite,
                    new RectangleF(Position.X + Width, Position.Y, -Width, Height),
                    new RectangleF(0, 0, currentSprite.Width, currentSprite.Height),
                    GraphicsUnit.Pixel);
            }
            else
            {
                // Обычная отрисовка при движении вправо или без движения
                g.DrawImage(currentSprite,
                    new RectangleF(Position.X, Position.Y, Width, Height),
                    new RectangleF(0, 0, currentSprite.Width, currentSprite.Height),
                    GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// Начинает движение влево.
        /// </summary>
        public void StartMovingLeft() => IsMovingLeft = true;

        /// <summary>
        /// Начинает движение вправо.
        /// </summary>
        public void StartMovingRight() => IsMovingRight = true;

        /// <summary>
        /// Останавливает движение влево.
        /// </summary>
        public void StopMovingLeft() => IsMovingLeft = false;

        /// <summary>
        /// Останавливает движение вправо.
        /// </summary>
        public void StopMovingRight() => IsMovingRight = false;

        /// <summary>
        /// Выполняет прыжок, если игрок находится на земле.
        /// </summary>
        public void Jump()
        {
            // Прыгать можно только при нулевой вертикальной скорости (т.е. стоя на платформе)
            if (_verticalVelocity == 0)
                _verticalVelocity = -JumpForce;
        }

        /// <summary>
        /// Проверяет, упал ли игрок за пределы экрана.
        /// </summary>
        /// <param name="screenHeight">Высота окна.</param>
        /// <returns>true, если игрок упал.</returns>
        public bool HasFallen(int screenHeight) => Position.Y > screenHeight;

        /// <summary>
        /// Освобождает ресурсы, связанные со спрайтами.
        /// </summary>
        public void Dispose()
        {
            _standingSprite?.Dispose();
            _walkingSprite?.Dispose();
        }

        /// <summary>
        /// Возвращает границы игрока (для столкновений и взаимодействий).
        /// </summary>
        /// <returns>Прямоугольник, описывающий игрока.</returns>
        public RectangleF GetBounds()
        {
            return new RectangleF(Position.X, Position.Y, Width, Height);
        }
    }
}
