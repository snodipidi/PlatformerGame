using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlatformerGame.GameObjects;
using PlatformerGame.Forms;
using System.Collections.Generic;

namespace PlatformerGame.GameStates
{
    /// <summary>
    /// Состояние игры, в котором происходит основной игровой процесс.
    /// </summary>
    public class PlayingState : IGameState
    {
        /// <summary>
        /// Внутренний класс для описания кнопок паузы.
        /// </summary>
        private class PauseButton
        {
            // Прямоугольная область, занимаемая кнопкой
            public Rectangle Bounds;
            // Текст на кнопке
            public string Text;
            // Прогресс наведения мыши (для анимации)
            public float HoverProgress;
            // Флаг, указывает, находится ли курсор над кнопкой
            public bool IsHovered;
        }

        // Ссылка на основную форму приложения
        private readonly MainForm _form;
        // Ссылка на объект игрока
        private readonly Player _player;
        // Ссылка на текущий уровень
        private readonly Level _level;
        // Менеджер уровней
        private readonly LevelManager _levelManager;
        // Шрифт для отображения интерфейса игрока
        private Font _hudFont;
        // Шрифт для меню паузы
        private Font _pauseFont;
        // Список кнопок меню паузы
        private readonly List<PauseButton> _pauseButtons = new List<PauseButton>();
        // Флаг: активна ли пауза
        private bool _isPaused;
        // Таймер игрового времени
        private GameTimer _gameTimer;
        // Область, в которой отображается иконка паузы
        private Rectangle _pauseIconBounds;
        // Флаг: освобождены ли ресурсы
        private bool _disposed;
        // Флаг: были ли инициализированы шрифты
        private bool _fontsInitialized;
        // Цвет кнопки по умолчанию
        private readonly Color _buttonBaseColor = Color.FromArgb(80, 100, 200);
        // Цвет кнопки при наведении курсора
        private readonly Color _buttonHoverColor = Color.FromArgb(120, 140, 240);

        /// <summary>
        /// Конструктор состояния игры PlayingState.
        /// Инициализирует необходимые поля, шрифты, кнопки паузы и игровой таймер.
        /// </summary>
        /// <param name="form">Основная форма приложения.</param>
        /// <param name="player">Объект игрока.</param>
        /// <param name="level">Текущий уровень.</param>
        /// <param name="levelManager">Менеджер уровней.</param>
        public PlayingState(MainForm form, Player player, Level level, LevelManager levelManager)
        {
            // Проверяем, что форма не равна null, иначе выбрасываем исключение
            _form = form ?? throw new ArgumentNullException(nameof(form));
            // Проверяем, что объект игрока не равен null, иначе выбрасываем исключение
            _player = player ?? throw new ArgumentNullException(nameof(player));
            // Проверяем, что уровень не равен null, иначе выбрасываем исключение
            _level = level ?? throw new ArgumentNullException(nameof(level));
            // Проверяем, что менеджер уровней не равен null, иначе выбрасываем исключение
            _levelManager = levelManager ?? throw new ArgumentNullException(nameof(levelManager));
            // Инициализируем шрифты для отображения интерфейса и меню паузы
            InitializeFonts();
            // Создаём и настраиваем кнопки меню паузы
            InitializePauseButtons();
            // Настраиваем игровой таймер
            SetupGameTimer();
        }

        /// <summary>
        /// Инициализирует шрифты для HUD и меню паузы, если они ещё не инициализированы.
        /// </summary>
        private void InitializeFonts()
        {
            // Если шрифты уже инициализированы, выходим из метода
            if (_fontsInitialized) return;
            // Создаём шрифт для HUD — Arial, размер 14, жирный
            _hudFont = new Font("Arial", 14, FontStyle.Bold);
            // Создаём шрифт для меню паузы — Arial, размер 24, жирный
            _pauseFont = new Font("Arial", 24, FontStyle.Bold);
            // Отмечаем, что шрифты инициализированы
            _fontsInitialized = true;
        }


        /// <summary>
        /// Настраивает и запускает игровой таймер с интервалом обновления 16 мс.
        /// </summary>
        private void SetupGameTimer()
        {
            // Создаём новый таймер с интервалом 16 миллисекунд (~60 FPS)
            _gameTimer = new GameTimer(16);
            // Подписываемся на событие обновления таймера — основной игровой цикл
            _gameTimer.Update += GameLoop;
            // Запускаем таймер
            _gameTimer.Start();
        }

        /// <summary>
        /// Инициализирует кнопки меню паузы и задаёт их начальное расположение.
        /// </summary>
        private void InitializePauseButtons()
        {
            // Добавляем кнопку "ПРОДОЛЖИТЬ"
            _pauseButtons.Add(new PauseButton { Text = "ПРОДОЛЖИТЬ" });
            // Добавляем кнопку "В МЕНЮ"
            _pauseButtons.Add(new PauseButton { Text = "В МЕНЮ" });
            // Обновляем позиции кнопок на экране
            UpdateButtonsPosition();
        }

        /// <summary>
        /// Обновляет позицию кнопок паузы на экране, располагая их по центру формы вертикально с отступами.
        /// </summary>
        private void UpdateButtonsPosition()
        {
            try
            {
                // Ширина кнопки
                int buttonWidth = 300;
                // Высота кнопки
                int buttonHeight = 60;
                // Начальная координата Y для первой кнопки, примерно по центру формы с небольшим сдвигом вверх
                int startY = _form.ClientSize.Height / 2 - 80;
                // Для каждой кнопки в списке
                foreach (var btn in _pauseButtons)
                {
                    // Устанавливаем размеры и позицию кнопки по центру по X и на текущем Y
                    btn.Bounds = new Rectangle(
                        _form.ClientSize.Width / 2 - buttonWidth / 2,
                        startY,
                        buttonWidth,
                        buttonHeight);
                    // Смещаем Y вниз для следующей кнопки с отступом 30 пикселей
                    startY += buttonHeight + 30;
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку в отладочную консоль
                Debug.WriteLine($"Ошибка обновления позиций кнопок: {ex.Message}");
            }
        }

        /// <summary>
        /// Основной игровой цикл, обновляет состояние игрока и уровня, проверяет коллизии и завершение уровня.
        /// </summary>
        private void GameLoop()
        {
            // Если игра на паузе или таймер не инициализирован, ничего не делаем
            if (_isPaused || _gameTimer == null) return;
            try
            {
                // Проверяем, что игрок и уровень существуют
                if (_player == null || _level == null) return;
                // Обновляем состояние игрока
                _player.Update();
                // Обновляем уровень с учётом позиции игрока по оси X
                _level.Update(_player.Position.X);
                // Проверяем коллизию игрока с ловушками или падение за границу окна
                if (_level.CheckPlayerCollision(_player) || _player.HasFallen(_form.ClientSize.Height))
                {
                    // Завершаем игру — проигрыш
                    _form.GameOver();
                    return;
                }
                // Проверяем, пересёк ли игрок финишный флаг
                if (_player.GetBounds().IntersectsWith(_level.FinishFlag))
                {
                    // Завершаем уровень — победа
                    _form.CompleteLevel();
                    return;
                }
                // Запрашиваем перерисовку формы
                _form.Invalidate();
            }
            catch (Exception ex)
            {
                // Логируем исключения игрового цикла
                Debug.WriteLine($"Ошибка в игровом цикле: {ex.Message}");
            }
        }


        /// <summary>
        /// Отрисовывает весь игровой экран: мир, HUD, иконку паузы и меню паузы при необходимости.
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки.</param>
        public void Draw(Graphics g)
        {
            try
            {
                // Отрисовываем игровой мир
                DrawGameWorld(g);
                // Отрисовываем элементы интерфейса (HUD)
                DrawHUD(g);
                // Отрисовываем иконку паузы
                DrawPauseIcon(g);

                // Если игра на паузе, отрисовываем меню паузы
                if (_isPaused)
                {
                    DrawPauseMenu(g);
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибки отрисовки
                Debug.WriteLine($"Ошибка отрисовки: {ex.Message}");
            }
        }

        /// <summary>
        /// Отрисовывает игровой мир и игрока с учётом смещения камеры.
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки.</param>
        private void DrawGameWorld(Graphics g)
        {
            try
            {
                // Сдвигаем систему координат по X для имитации движения камеры
                g.TranslateTransform(-_level.CameraOffset, 0);
                // Отрисовываем уровень
                _level.Draw(g);
                // Отрисовываем игрока
                _player.Draw(g);
                // Сбрасываем трансформации
                g.ResetTransform();
            }
            catch (Exception ex)
            {
                // Логируем ошибки отрисовки игрового мира
                Debug.WriteLine($"Ошибка отрисовки игрового мира: {ex.Message}");
            }
        }

        /// <summary>
        /// Отрисовывает элементы HUD (интерфейса), например уровень и прогресс.
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки.</param>
        private void DrawHUD(Graphics g)
        {
            try
            {
                // Инициализируем шрифты, если ещё не инициализированы
                InitializeFonts();

                if (_hudFont != null)
                {
                    // Отрисовываем номер текущего уровня в левом верхнем углу
                    g.DrawString($"Уровень: {_levelManager.GetCurrentLevel().LevelNumber}",
                        _hudFont, Brushes.White, 20, 20);
                }
                // Отрисовываем индикатор прогресса уровня
                DrawProgressBar(g);
            }
            catch (Exception ex)
            {
                // Логируем ошибки отрисовки HUD
                Debug.WriteLine($"Ошибка отрисовки HUD: {ex.Message}");
                // Пытаемся переинициализировать шрифты при ошибке
                ReinitializeFonts();
            }
        }

        /// <summary>
        /// Переинициализирует шрифты, сбрасывая флаг и вызывая инициализацию.
        /// </summary>
        private void ReinitializeFonts()
        {
            // Сбрасываем флаг инициализации шрифтов
            _fontsInitialized = false;
            // Инициализируем шрифты заново
            InitializeFonts();
        }

        /// <summary>
        /// Отрисовывает индикатор прогресса уровня в виде прогресс-бара.
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки.</param>
        private void DrawProgressBar(Graphics g)
        {
            try
            {
                // Задаём размеры и отступы прогресс-бара
                int barWidth = 300;
                int barHeight = 12;
                int margin = 20;

                // Прямоугольник для прогресс-бара в правом верхнем углу
                var barRect = new Rectangle(
                    _form.ClientSize.Width - barWidth - margin - 40,
                    margin,
                    barWidth,
                    barHeight);

                // Отрисовываем фон прогресс-бара
                g.FillRectangle(Brushes.DarkSlateGray, barRect);

                // Рассчитываем заполненную часть по прогрессу уровня
                float progress = _level.Progress;
                var filledRect = new Rectangle(
                    barRect.X,
                    barRect.Y,
                    (int)(barRect.Width * progress),
                    barRect.Height);

                // Создаём градиентный кисть для заполненной части
                using (var brush = new LinearGradientBrush(
                    filledRect,
                    Color.LimeGreen,
                    Color.DarkGreen,
                    0f))
                {
                    // Отрисовываем заполненную часть прогресс-бара
                    g.FillRectangle(brush, filledRect);
                }

                // Отрисовываем рамку вокруг прогресс-бара
                g.DrawRectangle(Pens.Black, barRect);
            }
            catch (Exception ex)
            {
                // Логируем ошибку отрисовки прогресс-бара
                Debug.WriteLine($"Ошибка отрисовки прогресс-бара: {ex.Message}");
            }
        }

        /// <summary>
        /// Отрисовывает иконку паузы или воспроизведения в правом верхнем углу экрана.
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки.</param>
        private void DrawPauseIcon(Graphics g)
        {
            try
            {
                // Размер иконки паузы
                int size = 32;
                // Отступы от краёв окна по горизонтали и вертикали
                int horizontalMargin = 20;
                int verticalMargin = 12;
                // Прямоугольник для иконки в правом верхнем углу формы
                _pauseIconBounds = new Rectangle(
                    _form.ClientSize.Width - size - horizontalMargin,
                    verticalMargin,
                    size,
                    size);

                // Включаем сглаживание для плавных линий
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Создаём кисть для рисования белым цветом с толщиной 1.5
                using (var pen = new Pen(Color.White, 1.5f))
                {
                    if (_isPaused)
                    {
                        // Если игра на паузе, рисуем треугольник Play (воспроизведение)
                        float triangleSize = size * 0.6f;

                        // Точки треугольника, центрированные внутри иконки
                        PointF[] triangle = {
                    new PointF(_pauseIconBounds.Left + (size - triangleSize) / 2 + 2,
                               _pauseIconBounds.Top + (size - triangleSize) / 2),
                    new PointF(_pauseIconBounds.Left + (size - triangleSize) / 2 + 2,
                               _pauseIconBounds.Bottom - (size - triangleSize) / 2),
                    new PointF(_pauseIconBounds.Right - (size - triangleSize) / 2 - 2,
                               _pauseIconBounds.Top + size / 2)
                };

                        // Заполняем треугольник белым цветом
                        g.FillPolygon(Brushes.White, triangle);
                    }
                    else
                    {
                        // Если игра активна, рисуем две вертикальные полоски (иконка паузы)

                        // Ширина каждой полоски — 22% от размера иконки
                        float barWidth = size * 0.22f;
                        // Расстояние между полосками — 18% от размера иконки
                        float gap = size * 0.18f;
                        // Общая ширина двух полосок с промежутком
                        float totalWidth = 2 * barWidth + gap;
                        // Координата X начала отрисовки полосок, центрирование по горизонтали
                        float startX = _pauseIconBounds.Left + (size - totalWidth) / 2;

                        // Прямоугольник левой полоски с небольшими вертикальными отступами
                        var leftRect = new RectangleF(
                            startX,
                            _pauseIconBounds.Top + size * 0.15f,
                            barWidth,
                            size * 0.7f);

                        // Прямоугольник правой полоски с промежутком справа от левой
                        var rightRect = new RectangleF(
                            startX + barWidth + gap,
                            _pauseIconBounds.Top + size * 0.15f,
                            barWidth,
                            size * 0.7f);

                        // Рисуем полоски с закруглёнными углами
                        using (var path = RoundedRect(leftRect, 1.5f))
                        using (var path2 = RoundedRect(rightRect, 1.5f))
                        {
                            // Заполняем закруглённые прямоугольники белым цветом
                            g.FillPath(Brushes.White, path);
                            g.FillPath(Brushes.White, path2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку при отрисовке иконки
                Debug.WriteLine($"Ошибка отрисовки иконки: {ex.Message}");
            }
        }

        /// <summary>
        /// Создаёт объект GraphicsPath с закруглёнными углами.
        /// </summary>
        /// <param name="bounds">Прямоугольник, вокруг которого создаётся путь.</param>
        /// <param name="radius">Радиус скругления углов.</param>
        /// <returns>Путь с закруглёнными углами.</returns>
        private static GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {
            var path = new GraphicsPath();
            // Диаметр дуг для углов равен двойному радиусу
            float diameter = radius * 2;
            // Верхний левый угол — дуга с углом 180-270 градусов
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            // Верхний правый угол — дуга с углом 270-360 градусов
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            // Нижний правый угол — дуга с углом 0-90 градусов
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            // Нижний левый угол — дуга с углом 90-180 градусов
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            // Закрываем фигуру для создания замкнутого контура
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Отрисовывает меню паузы с затемнением, заголовком и кнопками.
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки.</param>
        private void DrawPauseMenu(Graphics g)
        {
            try
            {
                // Рисуем полупрозрачный чёрный фон на весь экран (затемнение)
                g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)),
                    new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

                // Создаём крупный жирный шрифт для заголовка "ПАУЗА"
                using (var font = new Font("Arial", 48, FontStyle.Bold))
                {
                    string text = "ПАУЗА";
                    // Измеряем размеры текста для центрирования по горизонтали
                    var size = g.MeasureString(text, font);
                    // Рисуем текст белым цветом, по центру по ширине и на четверти высоты экрана
                    g.DrawString(text, font, Brushes.White,
                        (_form.ClientSize.Width - size.Width) / 2,
                        _form.ClientSize.Height / 4);
                }

                // Отрисовываем все кнопки меню паузы
                foreach (var btn in _pauseButtons)
                {
                    DrawPauseButton(g, btn);
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку отрисовки меню паузы
                Debug.WriteLine($"Ошибка отрисовки меню паузы: {ex.Message}");
            }
        }


        private void DrawPauseButton(Graphics g, PauseButton btn)
        {
            try
            {
                int cornerRadius = 15;
                Rectangle rect = btn.Bounds;

                // Фон с градиентом
                using (var path = RoundedRect(rect, cornerRadius))
                using (var brush = new LinearGradientBrush(
                    rect,
                    btn.IsHovered ? Color.FromArgb(255, 80, 180, 220) : Color.FromArgb(255, 40, 90, 130),
                    btn.IsHovered ? Color.FromArgb(255, 40, 120, 180) : Color.FromArgb(255, 20, 60, 100),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Рамка
                using (var path = RoundedRect(rect, cornerRadius))
                using (var pen = new Pen(btn.IsHovered ? Color.Cyan : Color.DeepSkyBlue, 3))
                {
                    g.DrawPath(pen, path);
                }

                // Текст с тенью
                using (var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                using (var font = new Font("Arial", 20, FontStyle.Bold)) // Используем Arial вместо Impact
                {
                    // Тень
                    g.DrawString(btn.Text, font, Brushes.Black,
                        new Rectangle(rect.X + 2, rect.Y + 2, rect.Width, rect.Height),
                        format);

                    // Основной текст
                    g.DrawString(btn.Text, font, Brushes.White, rect, format);
                }

                // Эффект подсветки
                if (btn.IsHovered)
                {
                    using (var path = RoundedRect(rect, cornerRadius))
                    using (var glowBrush = new PathGradientBrush(path)
                    {
                        CenterColor = Color.FromArgb(80, 200, 255, 255),
                        SurroundColors = new[] { Color.Transparent }
                    })
                    {
                        g.FillPath(glowBrush, path);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отрисовки кнопки: {ex.Message}");
            }
        }

        /// <summary>
        /// Создаёт объект GraphicsPath с закруглёнными углами.
        /// </summary>
        /// <param name="bounds">Прямоугольник, вокруг которого создаётся путь.</param>
        /// <param name="radius">Радиус скругления углов.</param>
        /// <returns>Путь с закруглёнными углами.</returns>
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            // Диаметр дуг для углов равен двойному радиусу
            int diameter = radius * 2;
            // Верхний левый угол — дуга с углом 180-270 градусов
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            // Верхний правый угол — дуга с углом 270-360 градусов
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            // Нижний правый угол — дуга с углом 0-90 градусов
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            // Нижний левый угол — дуга с углом 90-180 градусов
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            // Закрываем фигуру для создания замкнутого контура
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Обрабатывает нажатия клавиш управления игроком.
        /// </summary>
        /// <param name="e">Аргументы события клавиатуры.</param>
        public void HandleInput(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // Начать движение влево
                case Keys.Left:
                    _player.StartMovingLeft();
                    break;
                // Начать движение вправо
                case Keys.Right:
                    _player.StartMovingRight();
                    break;
                // Прыжок
                case Keys.Space:
                    _player.Jump();
                    break;
            }
        }

        /// <summary>
        /// Обрабатывает щелчки мыши, включая взаимодействие с кнопками паузы и иконкой паузы.
        /// </summary>
        /// <param name="e">Аргументы события мыши.</param>
        public void HandleMouseClick(MouseEventArgs e)
        {
            // Если игра на паузе, проверяем клик по кнопкам паузы
            if (_isPaused)
            {
                // Проходим по всем кнопкам паузы
                foreach (var btn in _pauseButtons)
                {
                    // Проверяем, содержит ли кнопка точку клика
                    if (btn.Bounds.Contains(e.Location))
                    {
                        // Выполняем действие, связанное с кнопкой
                        HandlePauseAction(btn.Text);
                        return;
                    }
                }
            }
            // Проверяем, кликнули ли по иконке паузы (для переключения состояния паузы)
            if (_pauseIconBounds.Contains(e.Location))
            {
                _form.TogglePause();
            }
        }

        /// <summary>
        /// Выполняет действие, связанное с выбранной кнопкой меню паузы.
        /// </summary>
        /// <param name="buttonText">Текст кнопки паузы.</param>
        private void HandlePauseAction(string buttonText)
        {
            switch (buttonText)
            {
                // Продолжить игру — переключить паузу
                case "ПРОДОЛЖИТЬ":
                    _form.TogglePause();
                    break;
                // Выйти в главное меню — остановить таймер и показать меню
                case "В МЕНЮ":
                    _gameTimer.Stop();
                    _form.ShowMainMenu();
                    break;
            }
        }

        /// <summary>
        /// Останавливает игровой таймер, если он существует.
        /// </summary>
        public void StopGameTimer()
        {
            // Остановка таймера, если он инициализирован
            _gameTimer?.Stop();
        }

        /// <summary>
        /// Выполняется при входе в состояние игры (PlayingState).
        /// Подписывается на события мыши и запускает музыку.
        /// </summary>
        public void OnEnter()
        {
            // Подписка на событие перемещения мыши
            _form.MouseMove += HandleMouseMove;
            // Подписка на событие клика мыши с обёрткой (Wrapper)
            _form.MouseClick += HandleMouseClickWrapper;
            // Запуск фоновой музыки для игрового процесса
            SoundManager.PlayMusic("music2.wav");
        }


        /// <summary>
        /// Вызывается при выходе из состояния игры.
        /// Останавливает таймер, освобождает ресурсы, отписывается от событий и останавливает музыку.
        /// </summary>
        public void OnExit()
        {
            // Остановка игрового таймера
            StopGameTimer();
            // Очистка и освобождение ресурсов
            DisposeResources();
            // Отписка от события перемещения мыши
            _form.MouseMove -= HandleMouseMove;
            // Отписка от события клика мыши
            _form.MouseClick -= HandleMouseClickWrapper;
            // Остановка фоновой музыки
            SoundManager.StopMusic();
        }

        /// <summary>
        /// Освобождает используемые ресурсы, если они ещё не освобождены.
        /// </summary>
        private void DisposeResources()
        {
            // Если ресурсы уже освобождены, ничего не делать
            if (_disposed) return;
            // Не освобождаем шрифты полностью, но сбрасываем флаг инициализации
            _fontsInitialized = false;
            // Отмечаем ресурсы как освобождённые
            _disposed = true;
        }

        /// <summary>
        /// Вызывается при изменении размера окна.
        /// Обновляет позиции кнопок и перерисовывает форму.
        /// </summary>
        /// <param name="e">Аргументы события изменения размера.</param>
        public void OnResize(EventArgs e)
        {
            // Обновляем позицию кнопок паузы
            UpdateButtonsPosition();
            // Запрашиваем перерисовку формы
            _form.Invalidate();
        }

        /// <summary>
        /// Обновляет состояние анимации наведения для кнопок паузы.
        /// </summary>
        public void Update()
        {
            // Проходим по каждой кнопке паузы
            foreach (var btn in _pauseButtons)
            {
                // Изменяем значение HoverProgress плавно, в диапазоне от 0 до 1,
                // увеличивая если кнопка наведена, уменьшая если нет
                btn.HoverProgress = Math.Max(0, Math.Min(1,
                    btn.HoverProgress + (btn.IsHovered ? 0.1f : -0.1f)));
            }
        }


        /// <summary>
        /// Обрабатывает перемещение мыши.
        /// Обновляет состояние наведения для кнопок паузы при активной паузе.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события перемещения мыши.</param>
        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            // Проверяем, если игра на паузе
            if (_isPaused)
            {
                // Проходим по всем кнопкам паузы
                foreach (var btn in _pauseButtons)
                {
                    // Сохраняем предыдущее состояние наведения
                    bool wasHovered = btn.IsHovered;
                    // Проверяем, находится ли курсор внутри кнопки
                    btn.IsHovered = btn.Bounds.Contains(e.Location);

                    // Если состояние наведения изменилось, перерисовываем форму
                    if (btn.IsHovered != wasHovered)
                        _form.Invalidate();
                }
            }
        }

        /// <summary>
        /// Обертка для обработки клика мыши.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события клика мыши.</param>
        private void HandleMouseClickWrapper(object sender, MouseEventArgs e)
        {
            // Вызываем основной обработчик клика
            HandleMouseClick(e);
        }

        /// <summary>
        /// Деструктор класса PlayingState.
        /// Вызывает освобождение ресурсов.
        /// </summary>
        ~PlayingState()
        {
            // Освобождение ресурсов при сборке мусора
            DisposeResources();
        }
    }
}