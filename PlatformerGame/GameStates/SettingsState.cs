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
        /// <param name="form">Главная форма приложения.</param>
        public SettingsState(MainForm form)
        {
            // Сохраняем ссылку на форму
            _form = form;
            // Инициализируем элементы управления
            InitializeControls();
            // Вычисляем позиции переключателей
            UpdateTogglePositions();
        }

        /// <summary>
        /// Инициализирует кнопку "Назад" с заданными стилями и обработчиком клика.
        /// </summary>
        private void InitializeControls()
        {
            // Создаем новую кнопку
            _backButton = new Button
            {
                // Текст кнопки
                Text = "← Назад",
                // Шрифт кнопки
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                // Цвет текста
                ForeColor = Color.White,
                // Цвет фона кнопки
                BackColor = Color.FromArgb(30, 30, 60),
                // Плоский стиль кнопки
                FlatStyle = FlatStyle.Flat,
                // Размер кнопки
                Size = new Size(140, 50),
                // Позиция кнопки на форме
                Location = new Point(20, 20),
                // Вид курсора при наведении
                Cursor = Cursors.Hand
            };
            // Убираем рамку кнопки
            _backButton.FlatAppearance.BorderSize = 0;
            // Цвет фона при наведении
            _backButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 90);
            // Обработчик клика - возврат в главное меню
            _backButton.Click += (s, e) => _form.ShowMainMenu();
        }

        /// <summary>
        /// Вычисляет и обновляет позиции переключателей на экране настроек.
        /// </summary>
        private void UpdateTogglePositions()
        {
            int centerX = _form.ClientSize.Width / 2;                 // Центр окна по горизонтали
            int startY = _form.ClientSize.Height / 2 - ToggleHeight; // Начальная вертикальная позиция для первого переключателя
            // Позиция переключателя "Режим разработчика" с небольшим сдвигом вправо от центра
            _devModeToggleRect = new Rectangle(centerX + 80, startY, ToggleWidth, ToggleHeight);
            // Позиция переключателя "Музыка и звук" с вертикальным отступом от первого переключателя
            _musicToggleRect = new Rectangle(centerX + 80, startY + ElementsSpacing, ToggleWidth, ToggleHeight);
        }


        /// <summary>
        /// Отрисовывает экран настроек с фоновым градиентом, заголовком и переключателями.
        /// </summary>
        /// <param name="g">Графический контекст для рисования.</param>
        public void Draw(Graphics g)
        {
            // Создаем градиентный фон от тёмно-синего к более светлому по вертикали (угол 90 градусов)
            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height),
                Color.FromArgb(10, 20, 50),
                Color.FromArgb(40, 60, 100),
                90f))
            {
                // Заполняем фон градиентом
                g.FillRectangle(brush, 0, 0, _form.ClientSize.Width, _form.ClientSize.Height);
            }
            DrawHeader(g); // Отрисовываем заголовок "Настройки"
            // Отрисовываем переключатель режима разработчика с подписью
            DrawToggleWithLabel(g, "Режим разработчика", _devModeToggleRect, SoundManager.DeveloperMode);
            // Отрисовываем переключатель музыки и звука с подписью
            DrawToggleWithLabel(g, "Музыка и звук", _musicToggleRect, SoundManager.IsSoundEnabled);
        }


        /// <summary>
        /// Рисует заголовок "Настройки" по центру верхней части окна.
        /// </summary>
        /// <param name="g">Графический контекст для рисования.</param>
        private void DrawHeader(Graphics g)
        {
            // Создаем шрифт и кисть для текста заголовка
            using (var font = new Font("Segoe UI", 48, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Cyan))
            {
                string title = "Настройки";
                // Получаем размер текста для центрирования по горизонтали
                var size = g.MeasureString(title, font);
                // Рисуем строку заголовка, центрируя по ширине окна и отступая сверху на 60 пикселей
                g.DrawString(title, font, brush, (_form.ClientSize.Width - size.Width) / 2, 60);
            }
        }


        /// <summary>
        /// Отрисовка переключателя с подписью слева от него.
        /// </summary>
        /// <param name="g">Графический контекст для рисования.</param>
        /// <param name="label">Текст подписи переключателя.</param>
        /// <param name="rect">Прямоугольник, задающий позицию и размеры переключателя.</param>
        /// <param name="isOn">Состояние переключателя: включен (true) или выключен (false).</param>
        private void DrawToggleWithLabel(Graphics g, string label, Rectangle rect, bool isOn)
        {
            // Создаем шрифт и кисть для подписи
            using (var font = new Font("Segoe UI", 32, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.LightCyan))
            {
                // Измеряем размер текста, чтобы выровнять по центру вертикально и с отступом слева
                var textSize = g.MeasureString(label, font);
                g.DrawString(label, font, brush,
                    rect.X - textSize.Width - 20, // Отступ 20 пикселей слева от переключателя
                    rect.Y + (rect.Height - textSize.Height) / 2); // Центрируем по вертикали относительно переключателя
            }
            // Рисуем сам переключатель справа от подписи
            DrawToggleSwitch(g, rect, isOn);
        }


        /// <summary>
        /// Отрисовывает переключатель (toggle switch) в заданном прямоугольнике.
        /// </summary>
        /// <param name="g">Графический объект для рисования.</param>
        /// <param name="rect">Прямоугольник, в котором рисуется переключатель.</param>
        /// <param name="isOn">Состояние переключателя: true — включено, false — выключено.</param>
        private void DrawToggleSwitch(Graphics g, Rectangle rect, bool isOn)
        {
            // Цвет фона: зеленый при включенном состоянии, серый — при выключенном
            Color backColor = isOn ? Color.FromArgb(100, 220, 100) : Color.FromArgb(150, 150, 150);
            // Цвет круга переключателя — белый
            Color toggleColor = Color.White;
            // Радиус скругления равен половине высоты прямоугольника
            int radius = rect.Height / 2;
            // Создаем путь с закругленными углами для фона
            using (var path = RoundedRect(rect, radius))
            using (var backBrush = new SolidBrush(backColor))
            {
                // Заполняем фон переключателя
                g.FillPath(backBrush, path);
            }
            // Диаметр круга — высота минус отступы с двух сторон
            int circleDiameter = rect.Height - ToggleMargin * 2;
            // Координата X круга зависит от состояния переключателя
            int circleX = isOn ? rect.Right - circleDiameter - ToggleMargin : rect.Left + ToggleMargin;
            // Координата Y круга — верх прямоугольника плюс отступ
            int circleY = rect.Top + ToggleMargin;
            // Кисть для заливки круга
            using (var toggleBrush = new SolidBrush(toggleColor))
            {
                // Рисуем круг переключателя
                g.FillEllipse(toggleBrush, circleX, circleY, circleDiameter, circleDiameter);
            }
        }

        /// <summary>
        /// Создает путь для рисования прямоугольника со скругленными углами.
        /// </summary>
        /// <param name="bounds">Прямоугольник, для которого создается путь.</param>
        /// <param name="radius">Радиус скругления углов.</param>
        /// <returns>Объект <see cref="GraphicsPath"/> с описанием скругленного прямоугольника.</returns>
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            // Создаем новый объект GraphicsPath для построения пути
            var path = new GraphicsPath();
            // Вычисляем диаметр дуги, равный удвоенному радиусу скругления
            int diameter = radius * 2;
            // Добавляем верхний левый угол - дуга от 180 до 270 градусов
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            // Добавляем верхний правый угол - дуга от 270 до 360 градусов
            path.AddArc(bounds.X + bounds.Width - diameter, bounds.Y, diameter, diameter, 270, 90);
            // Добавляем нижний правый угол - дуга от 0 до 90 градусов
            path.AddArc(bounds.X + bounds.Width - diameter, bounds.Y + bounds.Height - diameter, diameter, diameter, 0, 90);
            // Добавляем нижний левый угол - дуга от 90 до 180 градусов
            path.AddArc(bounds.X, bounds.Y + bounds.Height - diameter, diameter, diameter, 90, 90);
            // Закрываем путь, чтобы соединить все сегменты
            path.CloseFigure();
            // Возвращаем построенный путь
            return path;
        }

        /// <summary>
        /// Обработка нажатий клавиш клавиатуры.
        /// </summary>
        /// <param name="e">Аргументы события нажатия клавиши.</param>
        public void HandleInput(KeyEventArgs e)
        {
            // Если нажата клавиша Escape, возвращаемся в главное меню
            if (e.KeyCode == Keys.Escape)
                _form.ShowMainMenu();
        }

        /// <summary>
        /// Обработка кликов мыши на элементы управления.
        /// </summary>
        /// <param name="e">Аргументы события мыши.</param>
        public void HandleMouseClick(MouseEventArgs e)
        {
            // Проверяем, попал ли клик в область переключателя режима разработчика
            if (_devModeToggleRect.Contains(e.Location))
            {
                // Инвертируем текущее состояние режима разработчика
                bool newMode = !SoundManager.DeveloperMode;
                if (newMode)
                {
                    // Спрашиваем подтверждение у пользователя на включение режима разработчика
                    var result = MessageBox.Show(
                        "Включение режима разработчика разблокирует все уровни! Продолжить?",
                        "Предупреждение",
                        MessageBoxButtons.YesNo);

                    if (result != DialogResult.Yes) return; // Пользователь отказался — выходим
                }
                // Устанавливаем новое состояние режима разработчика
                SoundManager.DeveloperMode = newMode;
                // Запрашиваем перерисовку формы для обновления интерфейса
                _form.Invalidate();
            }
            // Проверяем, попал ли клик в область переключателя звука
            else if (_musicToggleRect.Contains(e.Location))
            {
                // Переключаем состояние звука (включен/выключен)
                SoundManager.IsSoundEnabled = !SoundManager.IsSoundEnabled;
                if (!SoundManager.IsSoundEnabled)
                    SoundManager.StopMusic(); // Останавливаем музыку, если звук выключен
                else
                {
                    // При включении звука можно добавить воспроизведение музыки
                    // SoundManager.PlayMusic("music1.wav");
                }
                // Запрашиваем перерисовку формы для обновления интерфейса
                _form.Invalidate();
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
            // Удаляем кнопку из коллекции контролов формы
            _form.Controls.Remove(_backButton);
            // Освобождаем ресурсы кнопки
            _backButton.Dispose();
        }

        /// <summary>
        /// Обработка события изменения размера.
        /// </summary>
        /// <param name="e">Аргументы события изменения размера.</param>
        public void OnResize(EventArgs e)
        {
            // Фиксируем положение кнопки "Назад"
            _backButton.Location = new Point(20, 20);
            // Обновляем позицию переключателей в зависимости от нового размера формы
            UpdateTogglePositions();
            // Запрашиваем перерисовку формы
            _form.Invalidate();
        }

        /// <summary>
        /// Метод обновления (не используется в данном состоянии).
        /// </summary>
        public void Update() {}
    }
}
