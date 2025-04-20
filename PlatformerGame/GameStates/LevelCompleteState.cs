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

        public LevelCompletedState(MainForm form, LevelManager levelManager)
        {
            _form = form;
            _levelManager = levelManager;
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;
            int centerY = _form.ClientSize.Height / 2;

            _retryButton = new Rectangle(centerX - 100, centerY + 10, 200, 50);
            _nextButton = new Rectangle(centerX - 100, centerY + 70, 200, 50);
            _menuButton = new Rectangle(centerX - 100, centerY + 130, 200, 50);
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
            _form.Invalidate();
        }

        public void Draw(Graphics g)
        {
            // Полупрозрачный фон
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0, 80)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            // Заголовок
            string title = $"Уровень {_levelManager.GetCurrentLevel().LevelNumber} пройден!";
            var titleSize = g.MeasureString(title, _titleFont);
            g.DrawString(title, _titleFont, Brushes.Gold,
                (_form.ClientSize.Width - titleSize.Width) / 2,
                _form.ClientSize.Height / 2 - 60);

            // Кнопки
            DrawButton(g, _retryButton, "Повторить (R)", Brushes.LightGreen, Pens.DarkGreen);

            // Показываем кнопку следующего уровня только если он есть
            if (_levelManager.GetCurrentLevel().LevelNumber < _levelManager.GetAllLevels().Count)
            {
                DrawButton(g, _nextButton, $"Уровень {_levelManager.GetCurrentLevel().LevelNumber + 1} (N)",
                    Brushes.LightBlue, Pens.DarkBlue);
            }

            DrawButton(g, _menuButton, "В меню (M)", Brushes.LightCoral, Pens.DarkRed);
        }

        private void DrawButton(Graphics g, Rectangle rect, string text, Brush fill, Pen border)
        {
            g.FillRectangle(fill, rect);
            g.DrawRectangle(border, rect);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(text, _buttonFont, Brushes.Black, rect, format);
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
                _levelManager.SetCurrentLevel(currentLevelNum + 1); // Увеличиваем номер уровня
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
        public void OnEnter() { }
        public void OnExit()
        {
            _titleFont.Dispose();
            _buttonFont.Dispose();
            _infoFont.Dispose();
        }
    }
}