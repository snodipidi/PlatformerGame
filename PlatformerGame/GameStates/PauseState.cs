using PlatformerGame.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlatformerGame.GameStates
{
    public class PauseState : IGameState
    {
        private readonly MainForm _form;
        public IGameState PreviousState { get; }
        private Rectangle _resumeButton;
        private Rectangle _menuButton;

        public PauseState(MainForm form, IGameState previousState)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            PreviousState = previousState ?? throw new ArgumentNullException(nameof(previousState));
            CalculateButtonPositions();
        }

        private void CalculateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;
            int centerY = _form.ClientSize.Height / 2;

            _resumeButton = new Rectangle(centerX - 100, centerY - 30, 200, 50);
            _menuButton = new Rectangle(centerX - 100, centerY + 40, 200, 50);
        }

        public void Draw(Graphics g)
        {
            PreviousState.Draw(g);
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            using (var font = new Font("Arial", 32, FontStyle.Bold))
            {
                g.DrawString("ПАУЗА", font, Brushes.White,
                    _form.ClientSize.Width / 2 - 70, _form.ClientSize.Height / 3);
            }

            DrawButton(g, _resumeButton, "Продолжить");
            DrawButton(g, _menuButton, "В меню");
        }

        private void DrawButton(Graphics g, Rectangle rect, string text)
        {
            g.FillRectangle(Brushes.LightGray, rect);
            g.DrawRectangle(Pens.DarkGray, rect);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using (var font = new Font("Arial", 12, FontStyle.Bold))
            {
                g.DrawString(text, font, Brushes.Black, rect, format);
            }
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _form.TogglePause();
            }
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_resumeButton.Contains(e.Location))
            {
                _form.TogglePause();
            }
            else if (_menuButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
        }

        public void OnEnter() { }
        public void OnExit() { }
        public void Update() { }
        public void OnResize(EventArgs e) => CalculateButtonPositions();
    }
}