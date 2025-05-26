using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    /// <summary>
    /// Состояние экрана Game Over.
    /// </summary>
    public class GameOverState : IGameState
    {
        // Ссылка на основную форму
        private readonly MainForm _form;
        // Прямоугольники кнопок
        private Rectangle _retryButton;
        private Rectangle _menuButton;
        // Шрифты для заголовка и кнопок
        private readonly Font _titleFont = new Font("Arial", 36, FontStyle.Bold);
        private readonly Font _buttonFont = new Font("Arial", 14, FontStyle.Bold);
        // Положение курсора мыши
        private Point _mousePosition;
        // Флаг, чтобы звук проигрывался только один раз
        private bool _soundPlayed = false;
        /// <summary>
        /// Конструктор состояния Game Over.
        /// </summary>
        public GameOverState(MainForm form)
        {
            // Сохраняем ссылку на форму
            _form = form;
            // Обработчик перемещения мыши
            _form.MouseMove += (s, e) =>
            {
                // Сохраняем координаты мыши и перерисовываем форму
                _mousePosition = e.Location;
                _form.Invalidate();
            };
            // Устанавливаем начальные позиции кнопок
            UpdateButtonPositions();
        }

        /// <summary>
        /// Обновление позиций кнопок при изменении размеров окна.
        /// </summary>
        private void UpdateButtonPositions()
        {
            // Вычисляем центр окна
            int centerX = _form.ClientSize.Width / 2;
            int centerY = _form.ClientSize.Height / 2;
            // Позиция кнопки "Начать заново"
            _retryButton = new Rectangle(centerX - 120, centerY - 40, 240, 60);
            // Позиция кнопки "В меню"
            _menuButton = new Rectangle(centerX - 120, centerY + 40, 240, 60);
        }

        /// <summary>
        /// Обработка события изменения размера формы.
        /// </summary>
        public void OnResize(EventArgs e)
        {
            // Обновляем позиции кнопок и перерисовываем форму
            UpdateButtonPositions();
            _form.Invalidate();
        }

        /// <summary>
        /// Отрисовка интерфейса Game Over.
        /// </summary>
        public void Draw(Graphics g)
        {
            // Включаем сглаживание графики
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // Заливаем полупрозрачным тёмно-красным фоном
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 80, 0, 0)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));
            // Текст заголовка
            string title = "ВЫ ПРОИГРАЛИ!";
            // Измеряем размеры текста
            var titleSize = g.MeasureString(title, _titleFont);
            // Координаты отрисовки по центру
            float titleX = (_form.ClientSize.Width - titleSize.Width) / 2;
            float titleY = (_form.ClientSize.Height - titleSize.Height) / 2 - 120;
            // Рисуем тень для текста
            g.DrawString(title, _titleFont, Brushes.Black, titleX + 2, titleY + 2);
            // Рисуем основной текст
            g.DrawString(title, _titleFont, Brushes.Red, titleX, titleY);
            // Отрисовка кнопки "Начать заново"
            DrawButton(g, _retryButton, "Начать заново (R)",
                _retryButton.Contains(_mousePosition) ? Brushes.MediumSeaGreen : Brushes.LightGreen,
                Pens.DarkGreen);
            // Отрисовка кнопки "В меню"
            DrawButton(g, _menuButton, "В меню (M)",
                _menuButton.Contains(_mousePosition) ? Brushes.IndianRed : Brushes.MistyRose,
                Pens.Maroon);
        }

        /// <summary>
        /// Отрисовка одной кнопки с текстом и закруглёнными углами.
        /// </summary>
        private void DrawButton(Graphics g, Rectangle rect, string text, Brush fill, Pen border)
        {
            // Радиус скругления
            int radius = 20;
            // Получаем путь с закруглёнными углами
            using (GraphicsPath path = RoundedRect(rect, radius))
            {
                // Заливаем фон кнопки
                g.FillPath(fill, path);
                // Обводка кнопки
                g.DrawPath(border, path);
            }
            // Центрируем текст по кнопке
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            // Отрисовываем текст кнопки
            g.DrawString(text, _buttonFont, Brushes.Black, rect, format);
        }

        /// <summary>
        /// Создание графического пути с закруглёнными углами.
        /// </summary>
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            // Диаметр дуги
            int diameter = radius * 2;
            // Создаём путь
            var path = new GraphicsPath();
            // Верхний левый угол
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            // Верхний правый угол
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            // Нижний правый угол
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            // Нижний левый угол
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            // Закрываем фигуру
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Обработка ввода с клавиатуры.
        /// </summary>
        public void HandleInput(KeyEventArgs e)
        {
            // R — начать заново
            if (e.KeyCode == Keys.R)
            {
                _form.StartNewGame();
            }
            // M — перейти в главное меню
            else if (e.KeyCode == Keys.M)
            {
                _form.ShowMainMenu();
            }
        }

        /// <summary>
        /// Обработка клика мышью по кнопкам.
        /// </summary>
        public void HandleMouseClick(MouseEventArgs e)
        {
            // Клик по кнопке "Начать заново"
            if (_retryButton.Contains(e.Location))
            {
                _form.StartNewGame();
            }
            // Клик по кнопке "В меню"
            else if (_menuButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
        }

        /// <summary>
        /// Обновление состояния (не используется в этом состоянии).
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Действия при входе в состояние.
        /// </summary>
        public void OnEnter()
        {
            // Проигрываем звук поражения один раз
            if (!_soundPlayed)
            {
                SoundManager.PlayGameOverSound();
                _soundPlayed = true;
            }
        }

        /// <summary>
        /// Очистка ресурсов при выходе из состояния.
        /// </summary>
        public void OnExit()
        {
            _titleFont.Dispose();
            _buttonFont.Dispose();
        }
    }
}
