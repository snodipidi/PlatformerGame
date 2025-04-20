using System;
using System.Drawing;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    public class LevelCompletedState : IGameState
    {
        private readonly MainForm _form;
        private Rectangle _nextLevelButton;
        private Rectangle _menuButton;
        private readonly Font _titleFont = new Font("Arial", 32, FontStyle.Bold);
        private readonly Font _buttonFont = new Font("Arial", 12, FontStyle.Bold);
        private readonly Font _infoFont = new Font("Arial", 14, FontStyle.Italic);

        public LevelCompletedState(MainForm form)
        {
            _form = form;
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;
            int centerY = _form.ClientSize.Height / 2;

            _nextLevelButton = new Rectangle(
                centerX - 100,
                centerY + 10,
                200,
                50
            );

            _menuButton = new Rectangle(
                centerX - 100,
                centerY + 80, // Увеличил отступ между кнопками
                200,
                50
            );
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
            _form.Invalidate();
        }

        public void Draw(Graphics g)
        {
            // Полупрозрачный темно-синий фон
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0, 80)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            // Заголовок
            string title = "Уровень пройден!";
            var titleSize = g.MeasureString(title, _titleFont);
            g.DrawString(title, _titleFont, Brushes.Gold,
                (_form.ClientSize.Width - titleSize.Width) / 2,
                _form.ClientSize.Height / 2 - 100);

            // Сообщение о следующем уровне
            string info = "Следующий уровень в разработке...";
            var infoSize = g.MeasureString(info, _infoFont);
            g.DrawString(info, _infoFont, Brushes.White,
                (_form.ClientSize.Width - infoSize.Width) / 2,
                _form.ClientSize.Height / 2 - 30);

            // Кнопка "Попробовать снова"
            DrawButton(g, _nextLevelButton, "Попробовать снова (R)", Brushes.LightGreen, Pens.DarkGreen);

            // Кнопка "В меню"
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
            if (e.KeyCode == Keys.R)
            {
                _form.StartNewGame(); // Перезапуск текущего уровня
            }
            else if (e.KeyCode == Keys.M || e.KeyCode == Keys.Escape)
            {
                _form.ShowMainMenu();
            }
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_nextLevelButton.Contains(e.Location))
            {
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