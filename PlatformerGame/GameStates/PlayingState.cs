// PlayingState.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlatformerGame.GameObjects;
using PlatformerGame.Forms;

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
        private readonly Font _hudFont;
        private readonly Font _pauseFont;
        private readonly List<PauseButton> _pauseButtons = new List<PauseButton>();
        private bool _isPaused;
        private GameTimer _gameTimer;
        private Bitmap _backgroundBlur;
        private Rectangle _pauseIconBounds;

        private readonly Color _buttonBaseColor = Color.FromArgb(80, 100, 200);
        private readonly Color _buttonHoverColor = Color.FromArgb(120, 140, 240);

        public PlayingState(MainForm form, Player player, Level level, LevelManager levelManager)
        {
            _form = form;
            _player = player;
            _level = level;
            _levelManager = levelManager;

            _hudFont = new Font("Arial", 14, FontStyle.Bold);
            _pauseFont = new Font("Arial", 24, FontStyle.Bold);

            InitializePauseButtons();
            SetupGameTimer();
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

        private void GameLoop()
        {
            if (!_isPaused)
            {
                UpdateGameLogic();
            }
            _form.Invalidate();
        }

        private void UpdateGameLogic()
        {
            _player.Update(_level.Platforms);
            _level.Update(_player.Position.X);

            if (_level.CheckPlayerCollision(_player) || _player.HasFallen(_form.ClientSize.Height))
            {
                _form.GameOver();
                return;
            }

            if (_player.GetBounds().IntersectsWith(_level.FinishFlag))
            {
                _form.CompleteLevel();
            }
        }

        public void Draw(Graphics g)
        {
            DrawGameWorld(g);
            DrawHUD(g);

            if (_isPaused)
            {
                DrawPauseMenu(g);
            }
        }

        private void DrawGameWorld(Graphics g)
        {
            g.TranslateTransform(-_level.CameraOffset, 0);
            _level.Draw(g);
            _player.Draw(g);
            g.ResetTransform();
        }

        private void DrawHUD(Graphics g)
        {
            DrawProgressBar(g);
            g.DrawString($"Уровень: {_levelManager.GetCurrentLevel().LevelNumber}",
                _hudFont, Brushes.White, 20, 20);
        }

        private void DrawProgressBar(Graphics g)
        {
            int barWidth = 300;
            int barHeight = 12;
            int margin = 20;

            var barRect = new Rectangle(
                _form.ClientSize.Width - barWidth - margin - 40,
                margin,
                barWidth,
                barHeight);

            g.FillRectangle(Brushes.DarkSlateGray, barRect);

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

            g.DrawRectangle(Pens.Black, barRect);

            // Рисуем иконку паузы
            int size = 24;
            _pauseIconBounds = new Rectangle(barRect.Right + 10, barRect.Y - 6, size, size);

            if (_isPaused)
            {
                // Треугольник (иконка воспроизведения)
                Point[] triangle =
                {
                    new Point(_pauseIconBounds.Left + 4, _pauseIconBounds.Top + 4),
                    new Point(_pauseIconBounds.Right - 4, _pauseIconBounds.Top + _pauseIconBounds.Height / 2),
                    new Point(_pauseIconBounds.Left + 4, _pauseIconBounds.Bottom - 4)
                };
                g.FillPolygon(Brushes.White, triangle);
            }
            else
            {
                // Две вертикальные полосы
                g.FillRectangle(Brushes.White, _pauseIconBounds.X + 4, _pauseIconBounds.Y + 4, 4, 16);
                g.FillRectangle(Brushes.White, _pauseIconBounds.X + 14, _pauseIconBounds.Y + 4, 4, 16);
            }
        }

        private void DrawPauseMenu(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            using (var font = new Font("Arial", 48, FontStyle.Bold))
            {
                string text = "ПАУЗА";
                var size = g.MeasureString(text, font);
                g.DrawString(text, font, Brushes.White,
                    (_form.ClientSize.Width - size.Width) / 2,
                    _form.ClientSize.Height / 4);
            }

            foreach (var btn in _pauseButtons)
            {
                DrawPauseButton(g, btn);
            }
        }

        private void DrawPauseButton(Graphics g, PauseButton btn)
        {
            Color bgColor = Color.FromArgb(
                (int)(200 + 55 * btn.HoverProgress),
                btn.IsHovered ? _buttonHoverColor : _buttonBaseColor);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, btn.Bounds);
            }

            using (var pen = new Pen(Color.White, 2 + 3 * btn.HoverProgress))
            {
                g.DrawRectangle(pen, btn.Bounds);
            }

            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(btn.Text, _pauseFont, Brushes.White, btn.Bounds, format);
            }

            if (btn.IsHovered)
            {
                using (var glowPen = new Pen(Color.FromArgb(50, 255, 255, 255), 20))
                {
                    g.DrawRectangle(glowPen, btn.Bounds);
                }
            }
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.P)
            {
                TogglePause();
            }
            else if (!_isPaused)
            {
                HandleGameInput(e);
            }
        }

        private void HandleGameInput(KeyEventArgs e)
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
                TogglePause();
            }
        }

        private void HandlePauseAction(string buttonText)
        {
            switch (buttonText)
            {
                case "ПРОДОЛЖИТЬ":
                    TogglePause();
                    break;
                case "В МЕНЮ":
                    _gameTimer.Stop();
                    _form.ShowMainMenu();
                    break;
            }
        }

        public void HandleMouseMove(object sender, MouseEventArgs e)
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

        private void TogglePause()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                _gameTimer.Stop();
                CaptureBackground();
            }
            else
            {
                _gameTimer.Start();
            }
        }

        private void CaptureBackground()
        {
            _backgroundBlur = new Bitmap(_form.ClientSize.Width, _form.ClientSize.Height);
            using (var g = Graphics.FromImage(_backgroundBlur))
            {
                DrawGameWorld(g);
                ApplyBlurEffect(g);
            }
        }

        private void ApplyBlurEffect(Graphics g)
        {
            // Размытие (если нужно реализовать)
        }

        public void OnEnter()
        {
            _form.MouseMove += HandleMouseMove;
            _form.MouseClick += HandleMouseClickWrapper;
        }

        public void OnExit()
        {
            _gameTimer.Stop();
            _hudFont.Dispose();
            _pauseFont.Dispose();
            _backgroundBlur?.Dispose();
            _form.MouseMove -= HandleMouseMove;
            _form.MouseClick -= HandleMouseClickWrapper;
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

        private void HandleMouseClickWrapper(object sender, MouseEventArgs e)
        {
            HandleMouseClick(e);
        }
    }
}
