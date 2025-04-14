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
                centerY + 70,
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
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 100)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            string text = "Уровень пройден!";
            var textSize = g.MeasureString(text, _titleFont);
            g.DrawString(text, _titleFont, Brushes.LightGreen,
                (_form.ClientSize.Width - textSize.Width) / 2,
                _form.ClientSize.Height / 2 - 60);

            // Кнопка "Следующий уровень"
            g.FillRectangle(Brushes.LightGray, _nextLevelButton);
            g.DrawRectangle(Pens.DarkGray, _nextLevelButton);

            // Кнопка "В меню"
            g.FillRectangle(Brushes.LightGray, _menuButton);
            g.DrawRectangle(Pens.DarkGray, _menuButton);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString("Следующий уровень (N)", _buttonFont, Brushes.Black, _nextLevelButton, format);
            g.DrawString("В меню (M)", _buttonFont, Brushes.Black, _menuButton, format);
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.N)
            {
                _form.StartNewGame();
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
        public void OnExit() { }
    }
}