using PlatformerGame.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlatformerGame.GameStates
{
    public class MainMenuState : IGameState
    {
        private readonly MainForm _form;
        private Rectangle _startButton;
        private Rectangle _levelsButton;
        private Rectangle _exitButton;
        private readonly Font _titleFont;
        private readonly Font _buttonFont;
        private readonly StringFormat _textFormat;
         private Rectangle _rulesButton;

        public MainMenuState(MainForm form)
        {
            _form = form;
            _titleFont = new Font("Arial", 48, FontStyle.Bold);
            _buttonFont = new Font("Arial", 16, FontStyle.Bold);
            _textFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            UpdateButtonPositions();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            UpdateButtonPositions();
            _form.Invalidate();
        }

        private void UpdateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;
            int centerY = _form.ClientSize.Height / 2;

            _startButton = new Rectangle(centerX - 100, centerY - 80, 200, 50);
            _levelsButton = new Rectangle(centerX - 100, centerY - 20, 200, 50);
            _rulesButton = new Rectangle(centerX - 100, centerY + 40, 200, 50);
            _exitButton = new Rectangle(centerX - 100, centerY + 100, 200, 50);
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
            _form.Invalidate(); 
        }

        public void Draw(Graphics g)
        {
            {
                string title = "Platformer Game";
                var titleSize = g.MeasureString(title, _titleFont);
                g.DrawString(title, _titleFont, Brushes.White,
                    (_form.ClientSize.Width - titleSize.Width) / 2,
                    100);

                DrawButton(g, _startButton, "Начать игру", Brushes.LightGreen, Pens.DarkGreen);
                DrawButton(g, _levelsButton, "Уровни", Brushes.LightBlue, Pens.DarkBlue);
                DrawButton(g, _rulesButton, "Правила", Brushes.LightGoldenrodYellow, Pens.DarkGoldenrod); 
                DrawButton(g, _exitButton, "Выход", Brushes.LightCoral, Pens.DarkRed);
            }
        }

        private void DrawButton(Graphics g, Rectangle rect, string text, Brush fill, Pen border)
        {
            g.FillRectangle(fill, rect);
            g.DrawRectangle(border, rect);
            g.DrawString(text, _buttonFont, Brushes.Black, rect, _textFormat);
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_startButton.Contains(e.Location))
            {
                _form.StartNewGame();
            }
            else if (_levelsButton.Contains(e.Location))
            {
                _form.ShowLevelsMenu();
            }
            else if (_rulesButton.Contains(e.Location)) 
            {
                _form.ShowRules();
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
            else if (e.KeyCode == Keys.Escape)
            {
                _form.Close();
            }
            else if (e.KeyCode == Keys.L)
            {
                _form.ShowLevelsMenu();
            }
            else if (e.KeyCode == Keys.H) // H - help (правила)
            {
                _form.ShowRules();
            }
        }

        public void Update() { }

        public void OnEnter()
        {
            UpdateButtonPositions();
        }

        public void OnExit()
        {
            // Отписываемся от события при выходе из состояния
            _form.Resize -= Form_Resize;

            _titleFont.Dispose();
            _buttonFont.Dispose();
            _textFormat.Dispose();
        }
    }
}