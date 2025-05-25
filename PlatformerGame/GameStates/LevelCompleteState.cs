using System;
using System.Drawing;
using System.Windows.Forms;
using PlatformerGame.Forms;
using PlatformerGame.GameObjects;

namespace PlatformerGame.GameStates
{
    /// <summary>
    /// Состояние, отображающее экран завершения уровня с кнопками для перехода к следующему уровню, повтора и выхода в меню.
    /// </summary>
    public class LevelCompletedState : IGameState
    {
        // Ссылка на главную форму игры
        private readonly MainForm _form;
        // Ссылка на менеджер уровней
        private readonly LevelManager _levelManager;
        // Прямоугольник кнопки "Повторить"
        private Rectangle _retryButton;
        // Прямоугольник кнопки "Следующий уровень"
        private Rectangle _nextButton;
        // Прямоугольник кнопки "В меню"
        private Rectangle _menuButton;
        // Шрифт для заголовка
        private readonly Font _titleFont = new Font("Arial", 32, FontStyle.Bold);
        // Шрифт для кнопок
        private readonly Font _buttonFont = new Font("Arial", 12, FontStyle.Bold);
        // Шрифт для дополнительной информации
        private readonly Font _infoFont = new Font("Arial", 14, FontStyle.Italic);
        // Текущая позиция мыши
        private Point _mousePosition;

        /// <summary>
        /// Конструктор состояния завершения уровня. Подписывается на перемещение мыши.
        /// </summary>
        public LevelCompletedState(MainForm form, LevelManager levelManager)
        {
            // Сохраняем переданную форму
            _form = form;
            // Сохраняем переданный менеджер уровней
            _levelManager = levelManager;
            // Подписываемся на событие перемещения мыши, чтобы обновлять позицию и перерисовывать интерфейс
            _form.MouseMove += (s, e) =>
            {
                // Обновляем координаты мыши
                _mousePosition = e.Location;
                // Перерисовываем форму
                _form.Invalidate();
            };
            // Обновляем позиции кнопок на экране
            UpdateButtonPositions();
        }

        /// <summary>
        /// Обновляет позиции кнопок в зависимости от размеров окна.
        /// </summary>
        private void UpdateButtonPositions()
        {
            // Центр окна по горизонтали
            int centerX = _form.ClientSize.Width / 2;
            // Используем Graphics для вычисления размеров текста заголовка
            using (var g = _form.CreateGraphics())
            {
                // Текст заголовка
                string title = $"Уровень {_levelManager.GetCurrentLevel().LevelNumber} пройден!";
                // Размер текста заголовка
                var titleSize = g.MeasureString(title, _titleFont);
                // Высота одной кнопки
                int buttonHeight = 60;
                // Расстояние между кнопками
                int buttonSpacing = 20;
                // Количество кнопок (2 или 3)
                int buttonsCount = _levelManager.HasNextLevel() ? 3 : 2;
                // Общая высота всех кнопок
                int totalButtonsHeight = buttonsCount * buttonHeight + (buttonsCount - 1) * buttonSpacing;
                // Общая высота заголовка и кнопок
                int totalHeight = (int)(titleSize.Height) + 40 + totalButtonsHeight;
                // Верхняя граница заголовка
                int startY = (_form.ClientSize.Height - totalHeight) / 2;
                // Верхняя граница кнопок
                int buttonsStartY = startY + (int)titleSize.Height + 40;
                // Прямоугольник кнопки "Повторить"
                _retryButton = new Rectangle(centerX - 110, buttonsStartY, 220, buttonHeight);
                // Если есть следующий уровень — рисуем 3 кнопки
                if (_levelManager.HasNextLevel())
                {
                    // Кнопка "Следующий уровень"
                    _nextButton = new Rectangle(centerX - 110, buttonsStartY + buttonHeight + buttonSpacing, 220, buttonHeight);
                    // Кнопка "В меню"
                    _menuButton = new Rectangle(centerX - 110, buttonsStartY + 2 * (buttonHeight + buttonSpacing), 220, buttonHeight);
                }
                // Если это последний уровень — только 2 кнопки
                else
                {
                    // Кнопка "Следующий уровень" отсутствует
                    _nextButton = Rectangle.Empty;
                    // Кнопка "В меню"
                    _menuButton = new Rectangle(centerX - 110, buttonsStartY + buttonHeight + buttonSpacing, 220, buttonHeight);
                }
            }
        }

        /// <summary>
        /// Обрабатывает событие изменения размеров окна.
        /// </summary>
        public void OnResize(EventArgs e)
        {
            // Пересчитываем позиции кнопок
            UpdateButtonPositions();
            // Перерисовываем форму
            _form.Invalidate();
        }

        /// <summary>
        /// Отрисовывает интерфейс состояния.
        /// </summary>
        public void Draw(Graphics g)
        {
            // Включаем сглаживание
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // Отрисовываем полупрозрачный зелёный фон
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 80, 0)), new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));
            // Текст заголовка
            string title = $"Уровень {_levelManager.GetCurrentLevel().LevelNumber} пройден!";
            // Размер заголовка
            var titleSize = g.MeasureString(title, _titleFont);
            // Определяем количество кнопок
            int buttonsCount = _levelManager.HasNextLevel() ? 3 : 2;
            // Высота и отступы
            int buttonHeight = 60;
            int buttonSpacing = 20;
            int totalButtonsHeight = buttonsCount * buttonHeight + (buttonsCount - 1) * buttonSpacing;
            int totalHeight = (int)titleSize.Height + 40 + totalButtonsHeight;
            // Координата Y для заголовка
            float titleY = (_form.ClientSize.Height - totalHeight) / 2;
            // Рисуем заголовок
            g.DrawString(title, _titleFont, Brushes.Gold, (_form.ClientSize.Width - titleSize.Width) / 2, titleY);
            // Рисуем кнопку "Повторить"
            DrawButton(g, _retryButton, "Повторить (R)",
                _retryButton.Contains(_mousePosition) ? Brushes.MediumSeaGreen : Brushes.LightGreen,
                Pens.DarkGreen);
            // Если есть следующий уровень, рисуем кнопку "Следующий уровень"
            if (_levelManager.HasNextLevel())
            {
                DrawButton(g, _nextButton, "Следующий уровень (N)",
                    _nextButton.Contains(_mousePosition) ? Brushes.SkyBlue : Brushes.LightBlue,
                    Pens.DarkBlue);
            }
            // Рисуем кнопку "В меню"
            DrawButton(g, _menuButton, "В меню (М)",
                _menuButton.Contains(_mousePosition) ? Brushes.IndianRed : Brushes.LightCoral,
                Pens.DarkRed);
        }

        /// <summary>
        /// Рисует кнопку с заданными параметрами.
        /// </summary>
        private void DrawButton(Graphics g, Rectangle rect, string text, Brush fill, Pen border)
        {
            // Радиус скругления углов
            int radius = 20;

            // Получаем путь с закруглёнными углами
            using (var path = RoundedRect(rect, radius))
            {
                // Заливаем кнопку
                g.FillPath(fill, path);
                // Рисуем контур
                g.DrawPath(border, path);
            }

            // Центрируем текст
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            // Рисуем текст
            g.DrawString(text, _buttonFont, Brushes.Black, rect, format);
        }

        /// <summary>
        /// Создаёт графический путь с закруглёнными углами.
        /// </summary>
        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            // Диаметр скругления
            int diameter = radius * 2;
            // Новый путь
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            // Добавляем 4 дуги по углам
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            // Закрываем путь
            path.CloseFigure();
            // Возвращаем путь
            return path;
        }

        /// <summary>
        /// Обрабатывает нажатия клавиш.
        /// </summary>
        public void HandleInput(KeyEventArgs e)
        {
            // Если нажата клавиша R — повторяем уровень
            if (e.KeyCode == Keys.R)
            {
                _form.StartNewGame();
            }
            // Если нажата клавиша N — следующий уровень
            else if (e.KeyCode == Keys.N)
            {
                int currentLevelNum = _levelManager.GetCurrentLevel().LevelNumber;
                _levelManager.SetCurrentLevel(currentLevelNum + 1);
                _form.StartNewGame();
            }
            // Если нажата клавиша M или Escape — возвращаемся в меню
            else if (e.KeyCode == Keys.M || e.KeyCode == Keys.Escape)
            {
                _form.ShowMainMenu();
            }
        }

        /// <summary>
        /// Обрабатывает щелчки мыши по кнопкам.
        /// </summary>
        public void HandleMouseClick(MouseEventArgs e)
        {
            // Повторить
            if (_retryButton.Contains(e.Location))
            {
                _form.StartNewGame();
            }
            // Следующий уровень
            else if (_nextButton.Contains(e.Location))
            {
                int currentLevelNum = _levelManager.GetCurrentLevel().LevelNumber;
                _levelManager.SetCurrentLevel(currentLevelNum + 1);
                _form.StartNewGame();
            }
            // В меню
            else if (_menuButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
        }

        /// <summary>
        /// Обновление состояния (не используется).
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Метод вызывается при входе в состояние — воспроизводим звук победы.
        /// </summary>
        public void OnEnter()
        {
            SoundManager.PlayWinSound();
        }

        /// <summary>
        /// Метод вызывается при выходе из состояния — освобождаем ресурсы.
        /// </summary>
        public void OnExit()
        {
            _titleFont.Dispose();
            _buttonFont.Dispose();
            _infoFont.Dispose();
        }
    }
}
