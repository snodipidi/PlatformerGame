using System;
using System.Drawing;
using System.Windows.Forms;
using PlatformerGame.Forms;
using PlatformerGame.GameObjects;

namespace PlatformerGame.GameStates
{
    public class LevelCompletedState : IGameState
    {
        private readonly MainForm _form;
        private readonly LevelManager _levelManager;
        private Rectangle _retryButton;
        private Rectangle _nextButton;
        private Rectangle _menuButton;
        private readonly Font _titleFont = new Font("Arial", 32, FontStyle.Bold);
        private readonly Font _buttonFont = new Font("Arial", 12, FontStyle.Bold);
        private readonly Font _infoFont = new Font("Arial", 14, FontStyle.Italic);
        private Point _mousePosition;

        public LevelCompletedState(MainForm form, LevelManager levelManager)
        {
            _form = form;
            _levelManager = levelManager;
            _form.MouseMove += (s, e) =>
            {
                _mousePosition = e.Location;
                _form.Invalidate();
            };
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;

            using (var g = _form.CreateGraphics())
            {
                string title = $"Уровень {_levelManager.GetCurrentLevel().LevelNumber} пройден!";
                var titleSize = g.MeasureString(title, _titleFont);

                int buttonHeight = 60;
                int buttonSpacing = 20;
                int buttonsCount = _levelManager.HasNextLevel() ? 3 : 2;
                int totalButtonsHeight = buttonsCount * buttonHeight + (buttonsCount - 1) * buttonSpacing;

                // Общая высота блока с надписью и кнопками
                int totalHeight = (int)(titleSize.Height) + 40 + totalButtonsHeight;

                // Верхняя точка блока, чтобы всё было по центру по вертикали
                int startY = (_form.ClientSize.Height - totalHeight) / 2;

                // Кнопки позиционируем относительно startY + высота заголовка + отступ
                int buttonsStartY = startY + (int)titleSize.Height + 40;

                _retryButton = new Rectangle(centerX - 110, buttonsStartY, 220, buttonHeight);

                if (_levelManager.HasNextLevel())
                {
                    _nextButton = new Rectangle(centerX - 110, buttonsStartY + buttonHeight + buttonSpacing, 220, buttonHeight);
                    _menuButton = new Rectangle(centerX - 110, buttonsStartY + 2 * (buttonHeight + buttonSpacing), 220, buttonHeight);
                }
                else
                {
                    _nextButton = Rectangle.Empty;
                    _menuButton = new Rectangle(centerX - 110, buttonsStartY + buttonHeight + buttonSpacing, 220, buttonHeight);
                }
            }
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
            _form.Invalidate();
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Зеленый полупрозрачный фон
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 80, 0)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            string title = $"Уровень {_levelManager.GetCurrentLevel().LevelNumber} пройден!";
            var titleSize = g.MeasureString(title, _titleFont);

            // Вычисляем Y для заголовка так, чтобы весь блок (заголовок + кнопки) был по центру
            int buttonsCount = _levelManager.HasNextLevel() ? 3 : 2;
            int buttonHeight = 60;
            int buttonSpacing = 20;
            int totalButtonsHeight = buttonsCount * buttonHeight + (buttonsCount - 1) * buttonSpacing;
            int totalHeight = (int)titleSize.Height + 40 + totalButtonsHeight;

            float titleY = (_form.ClientSize.Height - totalHeight) / 2;

            g.DrawString(title, _titleFont, Brushes.Gold,
                (_form.ClientSize.Width - titleSize.Width) / 2,
                titleY);

            DrawButton(g, _retryButton, "Повторить (R)",
                _retryButton.Contains(_mousePosition) ? Brushes.MediumSeaGreen : Brushes.LightGreen,
                Pens.DarkGreen);

            if (_levelManager.HasNextLevel())
            {
                DrawButton(g, _nextButton, $"Следующий уровень (N)",
                    _nextButton.Contains(_mousePosition) ? Brushes.SkyBlue : Brushes.LightBlue,
                    Pens.DarkBlue);
            }

            DrawButton(g, _menuButton, "В меню (М)",
                _menuButton.Contains(_mousePosition) ? Brushes.IndianRed : Brushes.LightCoral,
                Pens.DarkRed);
        }

        private void DrawButton(Graphics g, Rectangle rect, string text, Brush fill, Pen border)
        {
            int radius = 20;
            using (var path = RoundedRect(rect, radius))
            {
                g.FillPath(fill, path);
                g.DrawPath(border, path);
            }

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(text, _buttonFont, Brushes.Black, rect, format);
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R) // Повторить
            {
                _form.StartNewGame();
            }
            else if (e.KeyCode == Keys.N) // Следующий уровень
            {
                int currentLevelNum = _levelManager.GetCurrentLevel().LevelNumber;
                _levelManager.SetCurrentLevel(currentLevelNum + 1);
                _form.StartNewGame();
            }
            else if (e.KeyCode == Keys.M || e.KeyCode == Keys.Escape)
            {
                _form.ShowMainMenu();
            }
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_retryButton.Contains(e.Location))
            {
                _form.StartNewGame();
            }
            else if (_nextButton.Contains(e.Location))
            {
                int currentLevelNum = _levelManager.GetCurrentLevel().LevelNumber;
                _levelManager.SetCurrentLevel(currentLevelNum + 1);
                _form.StartNewGame();
            }
            else if (_menuButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
        }

        public void Update() { }

        public void OnEnter()
        {
            SoundManager.PlayWinSound();
        }

        public void OnExit()
        {
            _titleFont.Dispose();
            _buttonFont.Dispose();
            _infoFont.Dispose();
        }
    }
}
