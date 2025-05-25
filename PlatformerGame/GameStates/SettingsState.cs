using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    /// <summary>
    /// Состояние экрана настроек игры, содержит переключатели и кнопку "Назад".
    /// </summary>
    public class SettingsState : IGameState
    {
        // Ссылка на основную форму
        private readonly MainForm _form;

        // Кнопка "Назад"
        private Button _backButton;

        // Прямоугольники для отрисовки переключателей
        private Rectangle _musicToggleRect;
        private Rectangle _devModeToggleRect;

        // Константы для размеров и отступов переключателей
        private const int ToggleWidth = 80;
        private const int ToggleHeight = 40;
        private const int ToggleMargin = 8;
        private const int ElementsSpacing = 60;

        /// <summary>
        /// Конструктор состояния настроек.
        /// </summary>
        public SettingsState(MainForm form)
        {
            _form = form;                    // Сохраняем ссылку на форму
            InitializeControls();            // Инициализируем элементы управления
            UpdateTogglePositions();         // Вычисляем позиции переключателей
        }

        // Инициализация кнопки "Назад"
        private void InitializeControls()
        {
            _backButton = new Button
            {
                Text = "← Назад",                                                // Текст кнопки
                Font = new Font("Segoe UI", 18, FontStyle.Regular),              // Шрифт
                ForeColor = Color.White,                                         // Цвет текста
                BackColor = Color.FromArgb(30, 30, 60),                          // Фон
                FlatStyle = FlatStyle.Flat,                                      // Стиль кнопки
                Size = new Size(140, 50),                                        // Размер
                Location = new Point(20, 20),                                    // Положение
                Cursor = Cursors.Hand                                            // Курсор при наведении
            };
            _backButton.FlatAppearance.BorderSize = 0;                           // Без рамки
            _backButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 90); // Цвет при наведении
            _backButton.Click += (s, e) => _form.ShowMainMenu();                // Переход в главное меню
        }

        // Вычисление позиций переключателей
        private void UpdateTogglePositions()
        {
            int centerX = _form.ClientSize.Width / 2;                   // Центр по X
            int startY = _form.ClientSize.Height / 2 - ToggleHeight;   // Начальная Y-позиция

            _devModeToggleRect = new Rectangle(centerX + 80, startY, ToggleWidth, ToggleHeight); // Режим разработчика
            _musicToggleRect = new Rectangle(centerX + 80, startY + ElementsSpacing, ToggleWidth, ToggleHeight); // Музыка
        }

        /// <summary>
        /// Отрисовка экрана.
        /// </summary>
        public void Draw(Graphics g)
        {
            // Фон — градиент от тёмно-синего к более светлому
            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height),
                Color.FromArgb(10, 20, 50),
                Color.FromArgb(40, 60, 100),
                90f))
            {
                g.FillRectangle(brush, 0, 0, _form.ClientSize.Width, _form.ClientSize.Height);
            }

            DrawHeader(g); // Отрисовка заголовка

            // Отрисовка переключателей с подписями
            DrawToggleWithLabel(g, "Режим разработчика", _devModeToggleRect, SoundManager.DeveloperMode);
            DrawToggleWithLabel(g, "Музыка и звук", _musicToggleRect, SoundManager.IsSoundEnabled);
        }

        // Рисует заголовок "Настройки"
        private void DrawHeader(Graphics g)
        {
            using (var font = new Font("Segoe UI", 48, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Cyan))
            {
                string title = "Настройки";
                var size = g.MeasureString(title, font); // Получаем размер текста
                g.DrawString(title, font, brush, (_form.ClientSize.Width - size.Width) / 2, 60);
            }
        }

        // Отрисовка переключателя с подписью
        private void DrawToggleWithLabel(Graphics g, string label, Rectangle rect, bool isOn)
        {
            using (var font = new Font("Segoe UI", 32, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.LightCyan))
            {
                var textSize = g.MeasureString(label, font); // Размер надписи
                g.DrawString(label, font, brush,
                    rect.X - textSize.Width - 20,
                    rect.Y + (rect.Height - textSize.Height) / 2);
            }

            DrawToggleSwitch(g, rect, isOn); // Сам переключатель
        }

        // Отрисовка самого переключателя
        private void DrawToggleSwitch(Graphics g, Rectangle rect, bool isOn)
        {
            Color backColor = isOn ? Color.FromArgb(100, 220, 100) : Color.FromArgb(150, 150, 150); // Цвет фона
            Color toggleColor = Color.White; // Цвет круга
            int radius = rect.Height / 2;    // Радиус скругления

            using (var path = RoundedRect(rect, radius))
            using (var backBrush = new SolidBrush(backColor))
            {
                g.FillPath(backBrush, path); // Заливка фона
            }

            int circleDiameter = rect.Height - ToggleMargin * 2; // Диаметр круга
            int circleX = isOn ? rect.Right - circleDiameter - ToggleMargin : rect.Left + ToggleMargin;
            int circleY = rect.Top + ToggleMargin;

            using (var toggleBrush = new SolidBrush(toggleColor))
            {
                g.FillEllipse(toggleBrush, circleX, circleY, circleDiameter, circleDiameter); // Рисуем круг
            }
        }

        // Скруглённый прямоугольник
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.X + bounds.Width - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.X + bounds.Width - diameter, bounds.Y + bounds.Height - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Y + bounds.Height - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Обработка нажатий клавиш.
        /// </summary>
        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) // Возврат в меню по Esc
                _form.ShowMainMenu();
        }

        /// <summary>
        /// Обработка кликов мыши.
        /// </summary>
        public void HandleMouseClick(MouseEventArgs e)
        {
            // Тоггл разработчика
            if (_devModeToggleRect.Contains(e.Location))
            {
                bool newMode = !SoundManager.DeveloperMode;

                if (newMode)
                {
                    var result = MessageBox.Show(
                        "Включение режима разработчика разблокирует все уровни! Продолжить?",
                        "Предупреждение",
                        MessageBoxButtons.YesNo);

                    if (result != DialogResult.Yes) return; // Пользователь отказался
                }

                SoundManager.DeveloperMode = newMode;
                _form.Invalidate(); // Перерисовка
            }
            // Тоггл звука
            else if (_musicToggleRect.Contains(e.Location))
            {
                SoundManager.IsSoundEnabled = !SoundManager.IsSoundEnabled;

                if (!SoundManager.IsSoundEnabled)
                    SoundManager.StopMusic(); // Выключаем музыку
                else
                {
                    // Здесь можно добавить SoundManager.PlayMusic("music1.wav");
                }

                _form.Invalidate(); // Перерисовка
            }
        }

        /// <summary>
        /// Добавление кнопки при входе в состояние.
        /// </summary>
        public void OnEnter()
        {
            _form.Controls.Add(_backButton);
            _backButton.BringToFront();
        }

        /// <summary>
        /// Удаление кнопки при выходе из состояния.
        /// </summary>
        public void OnExit()
        {
            _form.Controls.Remove(_backButton);
            _backButton.Dispose();
        }

        /// <summary>
        /// Обработка события изменения размера.
        /// </summary>
        public void OnResize(EventArgs e)
        {
            _backButton.Location = new Point(20, 20); // Фиксируем положение
            UpdateTogglePositions();                 // Обновляем позицию переключателей
            _form.Invalidate();                      // Перерисовка
        }

        /// <summary>
        /// Метод обновления (не используется в данном состоянии).
        /// </summary>
        public void Update()
        {
            // Ничего не обновляем
        }
    }
}
