using System;
using System.Drawing;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    public class GameOverState : IGameState
    {
        private readonly MainForm _form;
        private Rectangle _retryButton;
        private readonly Font _titleFont = new Font("Arial", 32, FontStyle.Bold);
        private readonly Font _buttonFont = new Font("Arial", 12, FontStyle.Bold);

        private void UpdateButtonPosition()
        {
            _retryButton = new Rectangle(
                _form.ClientSize.Width / 2 - 100,
                _form.ClientSize.Height / 2 + 10,
                200,
                50
            );
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPosition();
            _form.Invalidate();
        }

        public GameOverState(MainForm form)
        {
            _form = form;
            InitializeUI();
        }

        private void InitializeUI()
        {
            _retryButton = new Rectangle(
                _form.ClientSize.Width / 2 - 100,
                _form.ClientSize.Height / 2 + 10,
                200,
                50);
        }

        public void Update() { }

        public void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            string text = "Вы проиграли!";
            var textSize = g.MeasureString(text, _titleFont);
            g.DrawString(text, _titleFont, Brushes.Red,
                (_form.ClientSize.Width - textSize.Width) / 2,
                _form.ClientSize.Height / 2 - 60);

            g.FillRectangle(Brushes.LightGray, _retryButton);
            g.DrawRectangle(Pens.DarkGray, _retryButton);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString("Начать заново (R)", _buttonFont, Brushes.Black, _retryButton, format);
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                _form.StartNewGame();
            }
            else if (e.KeyCode == Keys.Escape)
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
        }

        public void OnEnter() { }
        public void OnExit() { }
    }
}