using PlatformerGame.Forms;
using PlatformerGame.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PlatformerGame.GameStates
{
    /// <summary>
    /// Состояние выбора уровня в игре.
    /// Отображает список уровней, кнопку возврата и прогресс игрока.
    /// </summary>
    public class LevelsState : IGameState
    {
        // Ссылка на основную форму
        private readonly MainForm _form;
        // Менеджер уровней
        private readonly LevelManager _levelManager;
        // Прямоугольник кнопки "Назад"
        private Rectangle _backButton;
        // Список прямоугольников-кнопок для каждого уровня
        private List<Rectangle> _levelButtons = new List<Rectangle>();
        // Шрифт для текста кнопок
        private readonly Font _buttonFont = new Font("Arial", 12, FontStyle.Bold);
        // Индекс кнопки, на которую наведен курсор
        private int _hoveredIndex = -1;
        // Флаг, наведен ли курсор на кнопку "Назад"
        private bool _hoveringBack = false;
        // Прямоугольник фона прогресс-бара
        private Rectangle _progressBarRect;
        // Прямоугольник заливки прогресс-бара
        private Rectangle _progressFillRect;
        // Высота прогресс-бара
        private const int ProgressBarHeight = 25;
        // Отступы прогресс-бара от краёв
        private const int ProgressBarMargin = 20;

        /// <summary>
        /// Конструктор состояния уровней
        /// </summary>
        public LevelsState(MainForm form, LevelManager levelManager)
        {
            // Сохраняем ссылки
            _form = form;
            _levelManager = levelManager;

            // Инициализируем позиции кнопок
            UpdateButtonPositions();

            // Подписываемся на события мыши
            _form.MouseMove += (s, e) => HandleMouseMove(e);
            _form.MouseClick += (s, e) => HandleMouseClick(e);
        }

        /// <summary>
        /// Обновляет позиции кнопок уровней и других элементов на экране
        /// </summary>
        private void UpdateButtonPositions()
        {
            // Центр по горизонтали
            int centerX = _form.ClientSize.Width / 2;

            // Очищаем старые позиции
            _levelButtons.Clear();

            // Начальная вертикальная позиция
            int startY = _form.ClientSize.Height / 4 - 50;

            // Создаем прямоугольники-кнопки для всех уровней
            foreach (var level in _levelManager.GetAllLevels())
            {
                _levelButtons.Add(new Rectangle(centerX - 150, startY, 300, 50));
                startY += 70; // Отступ между кнопками
            }

            // Прямоугольник кнопки "Назад"
            _backButton = new Rectangle(
                centerX - 100,
                _form.ClientSize.Height - 180,
                200,
                50
            );

            // Прямоугольник прогресс-бара
            _progressBarRect = new Rectangle(
                ProgressBarMargin,
                _form.ClientSize.Height - 100,
                _form.ClientSize.Width - ProgressBarMargin * 2,
                ProgressBarHeight
            );
        }

        /// <summary>
        /// Обработка события изменения размера окна
        /// </summary>
        public void OnResize(EventArgs e)
        {
            // Пересчитываем позиции элементов
            UpdateButtonPositions();

            // Перерисовываем форму
            _form.Invalidate();
        }

        /// <summary>
        /// Отрисовка состояния
        /// </summary>
        public void Draw(Graphics g)
        {
            // Градиентный фон от темно-синего к черному
            using (var backgroundBrush = new LinearGradientBrush(
                _form.ClientRectangle,
                Color.MidnightBlue,
                Color.Black,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(backgroundBrush, _form.ClientRectangle);
            }

            // Заголовок
            string header = "ВЫБЕРИТЕ УРОВЕНЬ";
            using (var headerFont = new Font("Arial", 30, FontStyle.Bold))
            {
                var headerRect = new Rectangle(0, 20, _form.ClientSize.Width, 60);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                // Тень заголовка
                using (var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                {
                    g.DrawString(header, headerFont, shadowBrush, new PointF((_form.ClientSize.Width - g.MeasureString(header, headerFont).Width) / 2 + 3, 23));
                }

                // Цветной градиент текста
                using (var textBrush = new LinearGradientBrush(
                    headerRect,
                    Color.LightSkyBlue,
                    Color.MediumPurple,
                    LinearGradientMode.Vertical))
                {
                    g.DrawString(header, headerFont, textBrush, headerRect, format);
                }
            }

            // Формат центрирования текста
            var formatCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            // Получаем уровни
            var levels = _levelManager.GetAllLevels();

            // Отрисовка каждой кнопки уровня
            for (int i = 0; i < levels.Count; i++)
            {
                var level = levels[i];
                var rect = _levelButtons[i];
                bool hovered = i == _hoveredIndex;

                // Цвета в зависимости от состояния
                Color baseColor = level.IsLocked ? Color.Gray : (hovered ? Color.FromArgb(255, 80, 240) : Color.FromArgb(173, 216, 230));
                Color borderColor = hovered ? Color.Cyan : Color.DarkSlateBlue;

                // Отрисовка кнопки с рамкой
                using (var path = RoundedRect(rect, 10))
                using (var brush = new SolidBrush(baseColor))
                using (var pen = new Pen(borderColor, 2))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }

                // Текст кнопки
                string text = level.IsLocked ?
                    $"УРОВЕНЬ {level.LevelNumber} (ЗАБЛОКИРОВАН)" :
                    $"УРОВЕНЬ {level.LevelNumber}";

                using (var font = new Font("Arial", 14, FontStyle.Bold))
                using (var textBrush = new SolidBrush(level.IsLocked ? Color.DarkGray : (hovered ? Color.White : Color.Black)))
                {
                    g.DrawString(text, font, textBrush, rect, formatCenter);
                }
            }

            // Цвет кнопки "Назад"
            Color backColor = _hoveringBack ? Color.FromArgb(255, 80, 240) : Color.LightGray;
            Color backBorder = _hoveringBack ? Color.Cyan : Color.DarkGray;

            // Отрисовка кнопки "Назад"
            using (var path = RoundedRect(_backButton, 10))
            using (var brush = new SolidBrush(backColor))
            using (var pen = new Pen(backBorder, 2))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            // Текст на кнопке "Назад"
            using (var font = new Font("Arial", 14, FontStyle.Bold))
            {
                g.DrawString("НАЗАД", font, Brushes.Black, _backButton, formatCenter);
            }

            // Отрисовка прогресс-бара
            DrawProgressBar(g);
        }

        /// <summary>
        /// Отрисовывает прогресс-бар внизу экрана
        /// </summary>
        private void DrawProgressBar(Graphics g)
        {
            int width = _form.ClientSize.Width - ProgressBarMargin * 2;

            // Задаём прямоугольник фона
            _progressBarRect = new Rectangle(
                ProgressBarMargin,
                _form.ClientSize.Height - ProgressBarHeight - ProgressBarMargin,
                width,
                ProgressBarHeight);

            // Заливка фона
            g.FillRectangle(Brushes.DarkSlateGray, _progressBarRect);

            // Получаем прогресс (0–1)
            float rawProgress = _levelManager.GetTotalProgress();
            float progress = rawProgress < 0f ? 0f : rawProgress > 1f ? 1f : rawProgress;

            // Вычисляем ширину заполнения
            int fillWidth = Math.Max((int)(_progressBarRect.Width * progress), 1);

            // Прямоугольник заливки
            _progressFillRect = new Rectangle(
                _progressBarRect.X,
                _progressBarRect.Y,
                fillWidth,
                _progressBarRect.Height);

            // Отрисовка заливки — либо градиент, либо сплошной
            if (fillWidth > 1)
            {
                using (var brush = new LinearGradientBrush(
                    _progressFillRect,
                    Color.LimeGreen,
                    Color.DarkGreen,
                    0f))
                {
                    g.FillRectangle(brush, _progressFillRect);
                }
            }
            else
            {
                g.FillRectangle(Brushes.LimeGreen, _progressFillRect);
            }

            // Рамка вокруг прогресс-бара
            g.DrawRectangle(Pens.Black, _progressBarRect);

            // Текст прогресса (в процентах)
            using (var font = new Font("Arial", 14, FontStyle.Bold))
            {
                string text = $"Прогресс: {(int)(progress * 100)}%";
                var textSize = g.MeasureString(text, font);
                g.DrawString(text, font, Brushes.White,
                    _progressBarRect.X + (_progressBarRect.Width - textSize.Width) / 2,
                    _progressBarRect.Y - textSize.Height - 5);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие мыши
        /// </summary>
        public void HandleMouseClick(MouseEventArgs e)
        {
            var levels = _levelManager.GetAllLevels();

            // Проверяем нажатие на уровни
            for (int i = 0; i < _levelButtons.Count; i++)
            {
                if (_levelButtons[i].Contains(e.Location) && !levels[i].IsLocked)
                {
                    _levelManager.SetCurrentLevel(levels[i].LevelNumber);
                    _form.StartNewGame();
                    return;
                }
            }

            // Проверка нажатия кнопки "Назад"
            if (_backButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
        }

        /// <summary>
        /// Обрабатывает движение мыши
        /// </summary>
        public void HandleMouseMove(MouseEventArgs e)
        {
            // Сброс наведения
            _hoveredIndex = -1;
            _hoveringBack = false;

            // Проверка наведения на кнопки уровней
            for (int i = 0; i < _levelButtons.Count; i++)
            {
                if (_levelButtons[i].Contains(e.Location))
                {
                    _hoveredIndex = i;
                    break;
                }
            }

            // Проверка наведения на кнопку "Назад"
            if (_backButton.Contains(e.Location))
            {
                _hoveringBack = true;
            }

            // Перерисовка
            _form.Invalidate();
        }

        /// <summary>
        /// Обновление логики (не требуется)
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Обработка ввода с клавиатуры
        /// </summary>
        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _form.ShowMainMenu();
            }
        }

        /// <summary>
        /// Выполняется при входе в состояние
        /// </summary>
        public void OnEnter()
        {
            _levelButtons.Clear();
            UpdateButtonPositions();
        }

        /// <summary>
        /// Выполняется при выходе из состояния
        /// </summary>
        public void OnExit()
        {
            _buttonFont.Dispose();
        }

        /// <summary>
        /// Создает округлённый прямоугольник (с закруглёнными углами) на основе указанных границ и радиуса.
        /// Используется для отрисовки сглаженных рамок или фонов.
        /// </summary>
        /// <param name="bounds">Границы прямоугольника</param>
        /// <param name="radius">Радиус скругления углов</param>
        /// <returns>Объект GraphicsPath, представляющий округлённый прямоугольник</returns>
        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            // Вычисляем диаметр закруглённого угла как удвоенный радиус
            int diameter = radius * 2;
            // Создаём новый путь для рисования кривых и прямых линий
            var path = new GraphicsPath();
            // Добавляем дугу для верхнего левого угла (слева сверху, 180° до 270°)
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            // Добавляем дугу для верхнего правого угла (справа сверху, 270° до 360°)
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            // Добавляем дугу для нижнего правого угла (справа снизу, 0° до 90°)
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            // Добавляем дугу для нижнего левого угла (слева снизу, 90° до 180°)
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            // Замыкаем путь, соединяя последнюю и первую точки
            path.CloseFigure();
            // Возвращаем готовый путь округлённого прямоугольника
            return path;
        }

    }
}
