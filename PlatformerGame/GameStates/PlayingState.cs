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
    public class PlayingState : IGameState
    {
        private class PauseButton
        {
            public Rectangle Bounds;
            public string Text;
            public float HoverProgress;
            public bool IsHovered;
        }

        private readonly MainForm _form;
        private readonly Player _player;
        private readonly Level _level;
        private readonly LevelManager _levelManager;
        private Font _hudFont;
        private Font _pauseFont;
        private readonly List<PauseButton> _pauseButtons = new List<PauseButton>();
        private bool _isPaused;
        private GameTimer _gameTimer;
        private Rectangle _pauseIconBounds;
        private bool _disposed;
        private bool _fontsInitialized;

        private readonly Color _buttonBaseColor = Color.FromArgb(80, 100, 200);
        private readonly Color _buttonHoverColor = Color.FromArgb(120, 140, 240);

        public PlayingState(MainForm form, Player player, Level level, LevelManager levelManager)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _level = level ?? throw new ArgumentNullException(nameof(level));
            _levelManager = levelManager ?? throw new ArgumentNullException(nameof(levelManager));

            InitializeFonts();
            InitializePauseButtons();
            SetupGameTimer();
        }

        private void InitializeFonts()
        {
            if (_fontsInitialized) return;

            _hudFont = new Font("Arial", 14, FontStyle.Bold);
            _pauseFont = new Font("Arial", 24, FontStyle.Bold);
            _fontsInitialized = true;
        }

        private void SetupGameTimer()
        {
            _gameTimer = new GameTimer(16);
            _gameTimer.Update += GameLoop;
            _gameTimer.Start();
        }

        private void InitializePauseButtons()
        {
            _pauseButtons.Add(new PauseButton { Text = "ПРОДОЛЖИТЬ" });
            _pauseButtons.Add(new PauseButton { Text = "В МЕНЮ" });
            UpdateButtonsPosition();
        }

        private void UpdateButtonsPosition()
        {
            try
            {
                int buttonWidth = 300;
                int buttonHeight = 60;
                int startY = _form.ClientSize.Height / 2 - 80;

                foreach (var btn in _pauseButtons)
                {
                    btn.Bounds = new Rectangle(
                        _form.ClientSize.Width / 2 - buttonWidth / 2,
                        startY,
                        buttonWidth,
                        buttonHeight);
                    startY += buttonHeight + 30;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка обновления позиций кнопок: {ex.Message}");
            }
        }

        private void GameLoop()
        {
            if (_isPaused) return;

            try
            {
                _player.Update();
                _level.Update(_player.Position.X);

                if (_level.CheckPlayerCollision(_player) || _player.HasFallen(_form.ClientSize.Height))
                {
                    _form.GameOver();
                    return;
                }

                if (_player.GetBounds().IntersectsWith(_level.FinishFlag))
                {
                    _form.CompleteLevel();
                    return;
                }

                _form.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в игровом цикле: {ex.Message}");
            }
        }

        public void Draw(Graphics g)
        {
            try
            {
                DrawGameWorld(g);
                DrawHUD(g);
                DrawPauseIcon(g); // Добавлен отдельный вызов для иконки паузы

                if (_isPaused)
                {
                    DrawPauseMenu(g);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отрисовки: {ex.Message}");
            }
        }

        private void DrawGameWorld(Graphics g)
        {
            try
            {
                g.TranslateTransform(-_level.CameraOffset, 0);
                _level.Draw(g);
                _player.Draw(g);
                g.ResetTransform();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отрисовки игрового мира: {ex.Message}");
            }
        }

        private void DrawHUD(Graphics g)
        {
            try
            {
                InitializeFonts();

                if (_hudFont != null)
                {
                    g.DrawString($"Уровень: {_levelManager.GetCurrentLevel().LevelNumber}",
                        _hudFont, Brushes.White, 20, 20);
                }

                DrawProgressBar(g); // Восстановлен вызов прогресс-бара
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отрисовки HUD: {ex.Message}");
                ReinitializeFonts();
            }
        }

        private void ReinitializeFonts()
        {
            _fontsInitialized = false;
            InitializeFonts();
        }

        private void DrawProgressBar(Graphics g)
        {
            try
            {
                int barWidth = 300;
                int barHeight = 12;
                int margin = 20;

                var barRect = new Rectangle(
                    _form.ClientSize.Width - barWidth - margin - 40,
                    margin,
                    barWidth,
                    barHeight);

                // Фон прогресс-бара
                g.FillRectangle(Brushes.DarkSlateGray, barRect);

                // Заполненная часть
                float progress = _level.Progress;
                var filledRect = new Rectangle(
                    barRect.X,
                    barRect.Y,
                    (int)(barRect.Width * progress),
                    barRect.Height);

                using (var brush = new LinearGradientBrush(
                    filledRect,
                    Color.LimeGreen,
                    Color.DarkGreen,
                    0f))
                {
                    g.FillRectangle(brush, filledRect);
                }

                // Рамка
                g.DrawRectangle(Pens.Black, barRect);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отрисовки прогресс-бара: {ex.Message}");
            }
        }

        private void DrawPauseIcon(Graphics g)
        {
            try
            {
                int size = 24;
                int margin = 20;
                _pauseIconBounds = new Rectangle(
                    _form.ClientSize.Width - size - margin,
                    margin,
                    size,
                    size);

                if (_isPaused)
                {
                    // Иконка "Play"
                    Point[] triangle = {
                        new Point(_pauseIconBounds.Left + 4, _pauseIconBounds.Top + 4),
                        new Point(_pauseIconBounds.Right - 4, _pauseIconBounds.Top + size / 2),
                        new Point(_pauseIconBounds.Left + 4, _pauseIconBounds.Bottom - 4)
                    };
                    g.FillPolygon(Brushes.White, triangle);
                }
                else
                {
                    // Иконка "Pause"
                    g.FillRectangle(Brushes.White, _pauseIconBounds.X + 4, _pauseIconBounds.Y + 4, 4, 16);
                    g.FillRectangle(Brushes.White, _pauseIconBounds.X + 14, _pauseIconBounds.Y + 4, 4, 16);
                }

                // Рамка иконки
                g.DrawRectangle(Pens.White, _pauseIconBounds);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отрисовки иконки паузы: {ex.Message}");
            }
        }

        private void DrawPauseMenu(Graphics g)
        {
            try
            {
                // Затемнение
                g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)),
                    new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

                // Заголовок
                using (var font = new Font("Arial", 48, FontStyle.Bold))
                {
                    string text = "ПАУЗА";
                    var size = g.MeasureString(text, font);
                    g.DrawString(text, font, Brushes.White,
                        (_form.ClientSize.Width - size.Width) / 2,
                        _form.ClientSize.Height / 4);
                }

                // Кнопки
                foreach (var btn in _pauseButtons)
                {
                    DrawPauseButton(g, btn);
                }
            }
            catch (Exception ex)
            {
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

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        public void HandleInput(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    _player.StartMovingLeft();
                    break;
                case Keys.Right:
                    _player.StartMovingRight();
                    break;
                case Keys.Space:
                    _player.Jump();
                    break;
            }
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_isPaused)
            {
                foreach (var btn in _pauseButtons)
                {
                    if (btn.Bounds.Contains(e.Location))
                    {
                        HandlePauseAction(btn.Text);
                        return;
                    }
                }
            }

            if (_pauseIconBounds.Contains(e.Location))
            {
                _form.TogglePause();
            }
        }

        private void HandlePauseAction(string buttonText)
        {
            switch (buttonText)
            {
                case "ПРОДОЛЖИТЬ":
                    _form.TogglePause();
                    break;
                case "В МЕНЮ":
                    _gameTimer.Stop();
                    _form.ShowMainMenu();
                    break;
            }
        }

        public void OnEnter()
        {
            _form.MouseMove += HandleMouseMove;
            _form.MouseClick += HandleMouseClickWrapper;
        }

        public void OnExit()
        {
            DisposeResources();
            _form.MouseMove -= HandleMouseMove;
            _form.MouseClick -= HandleMouseClickWrapper;
        }

        private void DisposeResources()
        {
            if (_disposed) return;

            // Не освобождаем шрифты полностью
            _fontsInitialized = false;
            _disposed = true;
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonsPosition();
            _form.Invalidate();
        }

        public void Update()
        {
            foreach (var btn in _pauseButtons)
            {
                btn.HoverProgress = Math.Max(0, Math.Min(1,
                    btn.HoverProgress + (btn.IsHovered ? 0.1f : -0.1f)));
            }
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPaused)
            {
                foreach (var btn in _pauseButtons)
                {
                    bool wasHovered = btn.IsHovered;
                    btn.IsHovered = btn.Bounds.Contains(e.Location);

                    if (btn.IsHovered != wasHovered)
                        _form.Invalidate();
                }
            }
        }

        private void HandleMouseClickWrapper(object sender, MouseEventArgs e)
        {
            HandleMouseClick(e);
        }

        ~PlayingState()
        {
            DisposeResources();
        }
    }
}