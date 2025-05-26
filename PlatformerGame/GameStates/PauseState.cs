using PlatformerGame.Forms;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PlatformerGame.GameStates
{
    /// <summary>
    /// Состояние паузы игры.
    /// Отвечает за отображение меню паузы и обработку взаимодействия с ним.
    /// </summary>
    public class PauseState : IGameState
    {
        // Ссылка на основную форму приложения.
        private readonly MainForm _form;
        // Предыдущее состояние игры, к которому вернёмся после выхода из паузы.
        public IGameState PreviousState { get; }
        // Прямоугольник кнопки "Продолжить".
        private Rectangle _resumeButton;
        // Прямоугольник кнопки "В меню".
        private Rectangle _menuButton;
        // Прямоугольник нажатой кнопки, если есть.
        private Rectangle? _pressedButton = null;
        // Прямоугольник наведённой кнопки, если есть.
        private Rectangle? _hoveredButton = null;

        /// <summary>
        /// Конструктор состояния паузы.
        /// </summary>
        /// <param name="form">Основная форма игры.</param>
        /// <param name="previousState">Предыдущее состояние игры.</param>
        /// <exception cref="ArgumentNullException">Если form или previousState равны null.</exception>
        public PauseState(MainForm form, IGameState previousState)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            PreviousState = previousState ?? throw new ArgumentNullException(nameof(previousState));
            CalculateButtonPositions();
        }

        /// <summary>
        /// Вычисляет позиции кнопок по центру окна.
        /// </summary>
        private void CalculateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;
            int centerY = _form.ClientSize.Height / 2;

            _resumeButton = new Rectangle(centerX - 100, centerY - 30, 200, 50);
            _menuButton = new Rectangle(centerX - 100, centerY + 40, 200, 50);
        }

        /// <summary>
        /// Отрисовывает меню паузы.
        /// </summary>
        /// <param name="g">Графический контекст для отрисовки.</param>
        public void Draw(Graphics g)
        {
            // Отрисовываем предыдущее состояние как фон
            PreviousState.Draw(g);
            // Затемняем экран полупрозрачным черным слоем
            using (SolidBrush dimBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            {
                g.FillRectangle(dimBrush, _form.ClientRectangle);
            }
            // Рисуем заголовок "ПАУЗА" с неоновым эффектом
            using (var font = new Font("Segoe UI", 36, FontStyle.Bold))
            using (var neonBrush = new SolidBrush(Color.Cyan))
            {
                g.DrawString("ПАУЗА", font, neonBrush,
                    _form.ClientSize.Width / 2 - 100, _form.ClientSize.Height / 4);
            }
            // Отрисовываем кнопки меню паузы
            DrawButton(g, _resumeButton, "Продолжить");
            DrawButton(g, _menuButton, "В меню");
        }

        /// <summary>
        /// Отрисовывает одну кнопку с градиентом и эффектами наведения и нажатия.
        /// </summary>
        /// <param name="g">Графический контекст.</param>
        /// <param name="rect">Прямоугольник кнопки.</param>
        /// <param name="text">Текст кнопки.</param>
        private void DrawButton(Graphics g, Rectangle rect, string text)
        {
            // Проверяем, нажата ли эта кнопка
            bool isPressed = _pressedButton == rect;
            // Проверяем, наведена ли мышь на эту кнопку
            bool isHovered = _hoveredButton == rect;
            // Объявляем переменные цветов для градиента и кисти для текста
            Color topColor, bottomColor;
            Brush textBrush;
            // Если кнопка нажата, выбираем темные синие цвета и белый текст
            if (isPressed)
            {
                topColor = Color.FromArgb(100, 100, 255);
                bottomColor = Color.FromArgb(70, 70, 200);
                textBrush = Brushes.White;
            }
            // Если кнопка наведена, выбираем яркие голубые цвета и черный текст
            else if (isHovered)
            {
                topColor = Color.Cyan;
                bottomColor = Color.DeepSkyBlue;
                textBrush = Brushes.Black;
            }
            // В обычном состоянии используем темно-синие цвета и белый текст
            else
            {
                topColor = Color.FromArgb(30, 30, 60);
                bottomColor = Color.FromArgb(50, 50, 90);
                textBrush = Brushes.White;
            }
            // Создаем путь с закругленными углами для кнопки
            using (GraphicsPath path = RoundedRect(rect, 20))
            // Создаем вертикальный градиент для заливки кнопки
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, topColor, bottomColor, LinearGradientMode.Vertical))
            // Создаем перо для обводки кнопки цветом циана толщиной 2 пикселя
            using (Pen pen = new Pen(Color.Cyan, 2))
            {
                // Заполняем кнопку градиентом
                g.FillPath(brush, path);
                // Рисуем обводку вокруг кнопки
                g.DrawPath(pen, path);
                // Если кнопка наведена, добавляем дополнительный неоновый контур
                if (isHovered)
                {
                    using (Pen glowPen = new Pen(Color.Cyan, 4))
                    {
                        g.DrawPath(glowPen, path);
                    }
                }
                // Создаем формат для выравнивания текста по центру кнопки
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                // Используем жирный шрифт Segoe UI 12pt для текста кнопки
                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                {
                    // Рисуем текст кнопки с выбранным цветом и выравниванием
                    g.DrawString(text, font, textBrush, rect, format);
                }
            }
        }


        /// <summary>
        /// Создает объект GraphicsPath с закругленными углами для прямоугольника.
        /// </summary>
        /// <param name="bounds">Прямоугольник.</param>
        /// <param name="radius">Радиус скругления углов.</param>
        /// <returns>Объект GraphicsPath с формой прямоугольника с закругленными углами.</returns>
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            // Вычисляем диаметр дуги для скругления углов
            int diameter = radius * 2;
            // Создаем новый графический путь
            var path = new GraphicsPath();
            // Начинаем новую фигуру
            path.StartFigure();
            // Добавляем левый верхний скругленный угол (дуга от 180° до 270°)
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            // Добавляем правый верхний скругленный угол (дуга от 270° до 360°)
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            // Добавляем правый нижний скругленный угол (дуга от 0° до 90°)
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            // Добавляем левый нижний скругленный угол (дуга от 90° до 180°)
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            // Замыкаем фигуру, чтобы соединить все дуги
            path.CloseFigure();
            // Возвращаем путь с закругленными углами
            return path;
        }


        /// <summary>
        /// Обрабатывает нажатия клавиш клавиатуры.
        /// </summary>
        /// <param name="e">Аргументы события нажатия клавиши.</param>
        public void HandleInput(KeyEventArgs e)
        {
            // Если нажата клавиша Escape — переключаем паузу
            if (e.KeyCode == Keys.Escape)
            {
                _form.TogglePause();
            }
        }

        /// <summary>
        /// Обрабатывает клики мыши.
        /// </summary>
        /// <param name="e">Аргументы события клика мыши.</param>
        public void HandleMouseClick(MouseEventArgs e)
        {
            // Если нажата кнопка "Продолжить", выключаем паузу
            if (_resumeButton.Contains(e.Location))
            {
                _form.TogglePause();
            }
            // Если нажата кнопка "В меню", переходим в главное меню
            else if (_menuButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
            // Сбрасываем состояние нажатой кнопки
            _pressedButton = null;
        }


        /// <summary>
        /// Обрабатывает нажатия кнопки мыши.
        /// </summary>
        /// <param name="e">Аргументы события нажатия мыши.</param>
        public void OnMouseDown(MouseEventArgs e)
        {
            // Если курсор находится над кнопкой "Продолжить" — отмечаем её как нажатую
            if (_resumeButton.Contains(e.Location))
                _pressedButton = _resumeButton;
            // Если над кнопкой "В меню" — отмечаем её как нажатую
            else if (_menuButton.Contains(e.Location))
                _pressedButton = _menuButton;
            // Перерисовываем форму для отображения изменений
            _form.Invalidate();
        }

        /// <summary>
        /// Обрабатывает отпускание кнопки мыши.
        /// </summary>
        /// <param name="e">Аргументы события отпускания мыши.</param>
        public void OnMouseUp(MouseEventArgs e)
        {
            // Сбрасываем состояние нажатой кнопки
            _pressedButton = null;
            // Перерисовываем форму для обновления визуального состояния
            _form.Invalidate();
        }


        /// <summary>
        /// Обрабатывает перемещение мыши для определения наведения на кнопки.
        /// </summary>
        /// <param name="e">Аргументы события движения мыши.</param>
        public void OnMouseMove(MouseEventArgs e)
        {
            // Сохраняем предыдущее состояние наведённой кнопки
            Rectangle? previousHovered = _hoveredButton;

            // Проверяем, наведена ли мышь на кнопку "Продолжить"
            if (_resumeButton.Contains(e.Location))
                _hoveredButton = _resumeButton;
            // Проверяем, наведена ли мышь на кнопку "В меню"
            else if (_menuButton.Contains(e.Location))
                _hoveredButton = _menuButton;
            // Иначе — ни одна кнопка не под курсором
            else
                _hoveredButton = null;
            // Если состояние наведения изменилось, перерисовываем форму
            if (previousHovered != _hoveredButton)
                _form.Invalidate();
        }


        /// <summary>
        /// Вызывается при входе в состояние паузы.
        /// Здесь не используется.
        /// </summary>
        public void OnEnter() { }

        /// <summary>
        /// Вызывается при выходе из состояния паузы.
        /// Здесь не используется.
        /// </summary>
        public void OnExit() { }

        /// <summary>
        /// Обновляет состояние паузы. Здесь пустая реализация.
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Обрабатывает изменение размера окна — пересчитывает позиции кнопок.
        /// </summary>
        /// <param name="e">Аргументы события изменения размера.</param>
        public void OnResize(EventArgs e) => CalculateButtonPositions();
    }
}
