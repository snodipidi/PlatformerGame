using System;
using System.Drawing;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    public class MainMenuState : IGameState
    {
        private readonly MainForm _form;
        private Rectangle _startButton;
        private Rectangle _exitButton;
        private readonly Font _titleFont = new Font("Arial", 48, FontStyle.Bold);
        private readonly Font _buttonFont = new Font("Arial", 16, FontStyle.Bold);

        public MainMenuState(MainForm form)
        {
            _form = form;
            InitializeUI();
        }

        private void InitializeUI()
        {
            _startButton = new Rectangle(_form.ClientSize.Width / 2 - 100, 250, 200, 50);
            _exitButton = new Rectangle(_form.ClientSize.Width / 2 - 100, 320, 200, 50);
        }

        public void Update() { }

        public void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0, 0)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            string title = "Platformer Game";
            var titleSize = g.MeasureString(title, _titleFont);
            g.DrawString(title, _titleFont, Brushes.White,
                (_form.ClientSize.Width - titleSize.Width) / 2, 100);

            g.FillRectangle(Brushes.LightGreen, _startButton);
            g.DrawRectangle(Pens.DarkGreen, _startButton);

            g.FillRectangle(Brushes.LightCoral, _exitButton);
            g.DrawRectangle(Pens.DarkRed, _exitButton);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString("Играть", _buttonFont, Brushes.Black, _startButton, format);
            g.DrawString("Выход", _buttonFont, Brushes.Black, _exitButton, format);
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_startButton.Contains(e.Location))
            {
                _form.StartNewGame();
            }
            else if (_exitButton.Contains(e.Location))
            {
                _form.Close();
            }
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _form.StartNewGame();
            }
        }

        public void OnEnter()
        {
            UpdateButtonPositions();
        }


        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;

            _startButton = new Rectangle(
                centerX - 100,
                250,
                200,
                50
            );

            _exitButton = new Rectangle(
                centerX - 100,
                320,
                200,
                50
            );

        }
        public void OnExit() { }
    }
}