using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    /// <summary>
    /// Главное меню игры с неоновыми кнопками и анимацией.
    /// Реализует интерфейс состояния игры IGameState.
    /// </summary>
    public class MainMenuState : IGameState
    {
        /// <summary>
        /// Вложенный класс для описания кнопки с неоновым эффектом.
        /// Хранит параметры для отрисовки и анимации подсветки.
        /// </summary>
        private class NeonButton
        {
            // Прямоугольник кнопки (позиция и размер)
            public Rectangle Bounds;
            // Текст кнопки
            public string Text;
            // Основной цвет кнопки
            public Color BaseColor;
            // Цвет свечения при наведении
            public Color GlowColor;
            // Прогресс эффекта наведения (0..1)
            public float HoverProgress;
            // Флаг, указывающий, наведён ли курсор на кнопку
            public bool IsHovered;
        }

        // Ссылка на основную форму приложения
        private readonly MainForm _form;
        // Список кнопок главного меню
        private readonly List<NeonButton> _buttons = new List<NeonButton>();
        // Таймер для анимации главного меню
        private readonly Timer _animationTimer;
        // Фоновое изображение меню
        private Bitmap _background;
        // Глобальное время для анимации эффектов
        private float _globalTime;
        // Расстояние между кнопками по вертикали
        private const int ButtonSpacing = 100;

        /// <summary>
        /// Конструктор состояния главного меню.
        /// </summary>
        /// <param name="form">Ссылка на основную форму игры</param>
        public MainMenuState(MainForm form)
        {
            // Сохраняем ссылку на форму
            _form = form;
            // Создаём и инициализируем фон
            InitializeBackground();
            // Создаём и настраиваем кнопки меню
            InitializeButtons();
            // Настраиваем таймер для анимации (примерно 60 FPS)
            _animationTimer = new Timer { Interval = 16 };
            // Подписываемся на событие тика таймера для обновления анимации
            _animationTimer.Tick += (s, e) =>
            {
                // Увеличиваем глобальное время
                _globalTime += 0.05f;
                // Обновляем состояние кнопок (эффекты наведения)
                UpdateButtonStates();
                // Просим форму перерисовать содержимое
                _form.Invalidate();
            };
            // Запускаем таймер анимации
            _animationTimer.Start();
        }

        /// <summary>
        /// Инициализирует фоновое изображение с градиентом.
        /// </summary>
        private void InitializeBackground()
        {
            // Освобождаем предыдущий фон, если он есть
            _background?.Dispose();
            // Создаём новый битмап размером с клиентскую область формы
            _background = new Bitmap(_form.ClientSize.Width, _form.ClientSize.Height);
            // Рисуем градиентный фон
            using (var g = Graphics.FromImage(_background))
            {
                // Определяем цвета градиента
                var darkBlue = Color.FromArgb(15, 20, 40);
                var purple = Color.FromArgb(60, 0, 80);
                // Создаём линейный градиент от верхнего левого до нижнего правого угла
                using (var brush = new LinearGradientBrush(
                    new Point(0, 0),
                    new Point(_form.ClientSize.Width, _form.ClientSize.Height),
                    darkBlue,
                    purple))
                {
                    // Закрашиваем весь фон этим градиентом
                    g.FillRectangle(brush, new Rectangle(0, 0, _background.Width, _background.Height));
                }
            }
        }

        /// <summary>
        /// Создаёт и задаёт свойства кнопок главного меню.
        /// </summary>
        private void InitializeButtons()
        {
            // Очищаем список кнопок (если были старые)
            _buttons.Clear();
            // Задаём базовые цвета для кнопок
            var colors = new[]
            {
                Color.Cyan,
                Color.Magenta,
                Color.Yellow,
                Color.OrangeRed
            };
            // Названия кнопок меню
            var buttonTitles = new[] { "НОВАЯ ИГРА", "УРОВНИ", "НАСТРОЙКИ", "ВЫХОД" };
            // Вычисляем горизонтальный центр экрана
            int centerX = _form.ClientSize.Width / 2;
            // Начальная вертикальная позиция первой кнопки (примерно 1/3 высоты окна)
            int startY = _form.ClientSize.Height / 3;
            // Создаём кнопки с равным вертикальным отступом
            for (int i = 0; i < buttonTitles.Length; i++)
            {
                _buttons.Add(new NeonButton
                {
                    Bounds = new Rectangle(centerX - 200, startY, 400, 80), // Размер и позиция
                    Text = buttonTitles[i],                               // Текст кнопки
                    BaseColor = colors[i],                                // Основной цвет
                    GlowColor = Color.FromArgb(100, colors[i]),          // Цвет свечения (прозрачный)
                    HoverProgress = 0f                                    // Начальный прогресс свечения — 0
                });
                // Сдвигаем позицию для следующей кнопки вниз на ButtonSpacing пикселей
                startY += ButtonSpacing;
            }
        }

        /// <summary>
        /// Обновляет прогресс эффекта свечения для каждой кнопки в зависимости от наведения мыши.
        /// </summary>
        private void UpdateButtonStates()
        {
            foreach (var btn in _buttons)
            {
                // Увеличиваем HoverProgress, если кнопка наведена, иначе уменьшаем
                btn.HoverProgress = Math.Max(0, Math.Min(1,
                    btn.HoverProgress + (btn.IsHovered ? 0.15f : -0.1f)));
            }
        }

        /// <summary>
        /// Отрисовывает главное меню.
        /// </summary>
        /// <param name="g">Объект графики для рисования</param>
        public void Draw(Graphics g)
        {
            // Рисуем фон
            g.DrawImage(_background, 0, 0);
            // Рисуем анимированный динамический эффект (синее сияние)
            DrawDynamicEffects(g);
            // Отрисовываем каждую кнопку с неоновым эффектом
            foreach (var btn in _buttons)
            {
                DrawNeonButton(g, btn);
            }
            // Рисуем заголовок игры
            DrawTitle(g);
        }

        /// <summary>
        /// Рисует динамический эффект — анимированный синий круг с пульсацией.
        /// </summary>
        /// <param name="g">Графика для рисования</param>
        private void DrawDynamicEffects(Graphics g)
        {
            // Создаём кисть с прозрачным синим цветом для свечения
            using (var effectBrush = new SolidBrush(Color.FromArgb(30, 0, 200, 255)))
            {
                // Размер круга меняется синусоидально по времени для эффекта пульсации
                float size = 150 + (float)Math.Sin(_globalTime) * 50;
                // Рисуем эллипс в центре по горизонтали и на высоте 100 по вертикали
                g.FillEllipse(effectBrush,
                    _form.ClientSize.Width / 2 - size / 2,
                    100 - size / 2,
                    size, size);
            }
        }


        /// <summary>
        /// Рисует одну неоновую кнопку с подсветкой и текстом.
        /// </summary>
        /// <param name="g">Графика для рисования</param>
        /// <param name="btn">Объект кнопки</param>
        private void DrawNeonButton(Graphics g, NeonButton btn)
        {
            // Вычисляем интенсивность свечения (от 0.2 до 1)
            float glow = btn.HoverProgress * 0.8f + 0.2f;
            // Получаем прямоугольник кнопки
            var bounds = btn.Bounds;
            // Создаём путь с закруглёнными углами для эффекта свечения
            using (var path = GetRoundedPath(bounds, 20))
            // Кисть с градиентом от цвета свечения к прозрачному
            using (var glowBrush = new PathGradientBrush(path))
            {
                glowBrush.CenterColor = Color.FromArgb((int)(150 * glow), btn.BaseColor);
                glowBrush.SurroundColors = new[] { Color.Transparent };

                // Закрашиваем путь свечением
                g.FillPath(glowBrush, path);
            }

            // Второй слой — полупрозрачная заливка основной кнопки
            using (var path = GetRoundedPath(bounds, 20))
            using (var brush = new SolidBrush(Color.FromArgb(100, btn.BaseColor)))
            {
                g.FillPath(brush, path);
            }

            // Рисуем обводку кнопки с толщиной, зависящей от свечения
            using (var path = GetRoundedPath(bounds, 20))
            using (var pen = new Pen(btn.BaseColor, 3 + 3 * glow))
            {
                g.DrawPath(pen, path);
            }

            // Рисуем текст кнопки белым жирным шрифтом по центру
            using (var font = new Font("Arial Black", 24, FontStyle.Bold))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(btn.Text, font, Brushes.White, bounds, format);
            }
        }


        /// <summary>
        /// Рисует заголовок игры с тенями и градиентом.
        /// </summary>
        /// <param name="g">Графика для рисования</param>
        private void DrawTitle(Graphics g)
        {
            // Создаём крупный шрифт Impact (жирный)
            using (var font = new Font("Impact", 72, FontStyle.Bold))
            // Градиентная кисть от Cyan слева к Magenta справа
            using (var brush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(_form.ClientSize.Width, 0),
                Color.Cyan,
                Color.Magenta))
            {
                string title = "Platformer Game";
                // Измеряем размер текста для центрирования
                var size = g.MeasureString(title, font);
                // Вычисляем позицию для текста по центру сверху с отступом 50 пикселей
                var pos = new PointF(
                    (_form.ClientSize.Width - size.Width) / 2,
                    50);
                // Рисуем теневой эффект — 10 раз с небольшим смещением по синусоиде и косинусоиде
                for (int i = 0; i < 10; i++)
                {
                    g.DrawString(title, font, Brushes.Black,
                        pos.X + (float)Math.Sin(_globalTime + i) * 3,
                        pos.Y + (float)Math.Cos(_globalTime + i) * 3);
                }
                // Рисуем основной текст с градиентом поверх теней
                g.DrawString(title, font, brush, pos);
            }
        }


        /// <summary>
        /// Создаёт путь с закруглёнными углами для прямоугольника.
        /// </summary>
        /// <param name="bounds">Область прямоугольника</param>
        /// <param name="radius">Радиус скругления углов</param>
        /// <returns>Объект GraphicsPath с закруглённым прямоугольником</returns>
        private GraphicsPath GetRoundedPath(Rectangle bounds, int radius)
        {
            // Создаём новый пустой путь
            var path = new GraphicsPath();
            // Добавляем верхний левый угол — дугу 90 градусов (из 180 в 270)
            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            // Добавляем верхний правый угол — дугу 90 градусов (из 270 в 360)
            path.AddArc(bounds.X + bounds.Width - radius, bounds.Y, radius, radius, 270, 90);
            // Добавляем нижний правый угол — дугу 90 градусов (из 0 в 90)
            path.AddArc(bounds.X + bounds.Width - radius, bounds.Y + bounds.Height - radius,
                radius, radius, 0, 90);
            // Добавляем нижний левый угол — дугу 90 градусов (из 90 в 180)
            path.AddArc(bounds.X, bounds.Y + bounds.Height - radius, radius, radius, 90, 90);
            // Закрываем контур фигуры
            path.CloseFigure();
            // Возвращаем готовый путь
            return path;
        }

        /// <summary>
        /// Обрабатывает нажатия клавиш клавиатуры в главном меню.
        /// </summary>
        /// <param name="e">Аргументы события клавиатуры</param>
        public void HandleInput(KeyEventArgs e)
        {
            // Если нажата клавиша Enter — запускаем новую игру
            if (e.KeyCode == Keys.Enter)
                _form.StartNewGame();
            // Если нажата клавиша Escape — закрываем приложение (только из главного меню)
            else if (e.KeyCode == Keys.Escape)
                _form.Close();
        }

        /// <summary>
        /// Обработчик клика мышью (для подписки на событие)
        /// </summary>
        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            // Вызываем интерфейсный метод с параметром MouseEventArgs
            HandleMouseClick(e);
        }

        /// <summary>
        /// Обрабатывает клик мыши и определяет, была ли нажата одна из кнопок меню.
        /// </summary>
        /// <param name="e">Параметры клика мыши</param>
        public void HandleMouseClick(MouseEventArgs e)
        {
            // Перебираем все кнопки
            foreach (var btn in _buttons)
            {
                // Если точка клика попадает в область кнопки
                if (btn.Bounds.Contains(e.Location))
                {
                    // Выполняем действие, соответствующее тексту кнопки
                    HandleAction(btn.Text);
                    // Прекращаем проверку, чтобы не вызывать несколько действий
                    break;
                }
            }
        }


        /// <summary>
        /// Выполняет действие, соответствующее названию кнопки.
        /// </summary>
        /// <param name="actionName">Текст кнопки</param>
        private void HandleAction(string actionName)
        {
            switch (actionName)
            {
                case "НОВАЯ ИГРА":
                    _form.StartNewGame();
                    break;
                case "УРОВНИ":
                    _form.ShowLevelsMenu();
                    break;
                case "НАСТРОЙКИ":
                    _form.ShowSettings();
                    break;
                case "ВЫХОД":
                    _form.Close();
                    break;
            }
        }


        /// <summary>
        /// Вызывается при входе в состояние главного меню.
        /// Подписывается на события мыши и запускает фоновую музыку.
        /// </summary>
        public void OnEnter()
        {
            // Подписываемся на событие движения мыши для отслеживания наведения на кнопки
            _form.MouseMove += HandleMouseMove;
            // Подписываемся на событие клика мыши для обработки нажатий на кнопки
            _form.MouseClick += HandleMouseClick;
            // Запускаем фоновую музыку для главного меню
            SoundManager.PlayMusic("music1.wav");
        }

        /// <summary>
        /// Вызывается при выходе из состояния главного меню.
        /// Отписывается от событий мыши, освобождает ресурсы и останавливает музыку.
        /// </summary>
        public void OnExit()
        {
            // Останавливаем таймер анимации кнопок
            _animationTimer.Stop();
            // Отписываемся от событий мыши, чтобы не обрабатывать их вне состояния меню
            _form.MouseMove -= HandleMouseMove;
            _form.MouseClick -= HandleMouseClick;
            // Освобождаем ресурсы фона, чтобы избежать утечек памяти
            _background?.Dispose();
            // Останавливаем воспроизведение фоновой музыки
            SoundManager.StopMusic();
        }

        /// <summary>
        /// Вызывается при изменении размера окна.
        /// Пересоздаёт фон и кнопки с учётом новых размеров.
        /// </summary>
        /// <param name="e">Аргументы события изменения размера</param>
        public void OnResize(EventArgs e)
        {
            // Пересоздаём фон с новыми размерами окна
            InitializeBackground();
            // Обновляем позиции и размеры кнопок под новые размеры окна
            InitializeButtons();
        }

        /// <summary>
        /// Метод обновления состояния. В данном классе не используется.
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Обрабатывает событие движения мыши.
        /// Обновляет состояние наведения на кнопки.
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события мыши</param>
        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            // Для каждой кнопки проверяем, находится ли курсор внутри её границ
            foreach (var btn in _buttons)
            {
                btn.IsHovered = btn.Bounds.Contains(e.Location);
            }
        }
    }
}