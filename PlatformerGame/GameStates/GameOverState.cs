using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    public class GameOverState : IGameState
    {
        private readonly MainForm _form;
        private Rectangle _retryButton;
        private Rectangle _menuButton;
        private readonly Font _titleFont = new Font("Arial", 36, FontStyle.Bold);
        private readonly Font _buttonFont = new Font("Arial", 14, FontStyle.Bold);
        private Point _mousePosition;

        public GameOverState(MainForm form)
        {
            _form = form;
            _form.MouseMove += (s, e) => { _mousePosition = e.Location; _form.Invalidate(); };
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;
            int centerY = _form.ClientSize.Height / 2;

            _retryButton = new Rectangle(centerX - 120, centerY - 40, 240, 60);
            _menuButton = new Rectangle(centerX - 120, centerY + 40, 240, 60);
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
            _form.Invalidate();
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 80, 0, 0)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            string title = "ВЫ ПРОИГРАЛИ!";
            var titleSize = g.MeasureString(title, _titleFont);
            float titleX = (_form.ClientSize.Width - titleSize.Width) / 2;
            float titleY = (_form.ClientSize.Height - titleSize.Height) / 2 - 120;

            g.DrawString(title, _titleFont, Brushes.Black, titleX + 2, titleY + 2);
            g.DrawString(title, _titleFont, Brushes.Red, titleX, titleY);

            DrawButton(g, _retryButton, "Начать заново (R)",
                _retryButton.Contains(_mousePosition) ? Brushes.MediumSeaGreen : Brushes.LightGreen,
                Pens.DarkGreen);

            DrawButton(g, _menuButton, "В меню (M)",
                _menuButton.Contains(_mousePosition) ? Brushes.IndianRed : Brushes.MistyRose,
                Pens.Maroon);
        }

        private void DrawButton(Graphics g, Rectangle rect, string text, Brush fill, Pen border)
        {
            int radius = 20;
            using (GraphicsPath path = RoundedRect(rect, radius))
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

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                _form.StartNewGame();
            }
            else if (e.KeyCode == Keys.M)
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
            else if (_menuButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
        }

        public void Update() { }

        public void OnEnter()
        {
            SoundManager.PlayGameOverSound();
        }

        public void OnExit()
        {
            _titleFont.Dispose();
            _buttonFont.Dispose();
        }
    }
}
