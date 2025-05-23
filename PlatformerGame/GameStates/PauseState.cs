using PlatformerGame.Forms;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PlatformerGame.GameStates
{
    public class PauseState : IGameState
    {
        private readonly MainForm _form;
        public IGameState PreviousState { get; }
        private Rectangle _resumeButton;
        private Rectangle _menuButton;

        private Rectangle? _pressedButton = null;
        private Rectangle? _hoveredButton = null;

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

            // Затемнение фона
            using (SolidBrush dimBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            {
                g.FillRectangle(dimBrush, _form.ClientRectangle);
            }

            // Заголовок
            using (var font = new Font("Segoe UI", 36, FontStyle.Bold))
            using (var neonBrush = new SolidBrush(Color.Cyan))
            {
                g.DrawString("ПАУЗА", font, neonBrush,
                    _form.ClientSize.Width / 2 - 100, _form.ClientSize.Height / 4);
            }

            DrawButton(g, _resumeButton, "Продолжить");
            DrawButton(g, _menuButton, "В меню");
        }

        private void DrawButton(Graphics g, Rectangle rect, string text)
        {
            bool isPressed = _pressedButton == rect;
            bool isHovered = _hoveredButton == rect;

            // Цвета кнопки
            Color topColor, bottomColor;
            Brush textBrush;

            if (isPressed)
            {
                topColor = Color.FromArgb(100, 100, 255);
                bottomColor = Color.FromArgb(70, 70, 200);
                textBrush = Brushes.White;
            }
            else if (isHovered)
            {
                topColor = Color.Cyan;
                bottomColor = Color.DeepSkyBlue;
                textBrush = Brushes.Black;
            }
            else
            {
                topColor = Color.FromArgb(30, 30, 60); // тёмно-синий
                bottomColor = Color.FromArgb(50, 50, 90);
                textBrush = Brushes.White;
            }

            using (GraphicsPath path = RoundedRect(rect, 20))
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, topColor, bottomColor, LinearGradientMode.Vertical))
            using (Pen pen = new Pen(Color.Cyan, 2))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);

                // Неоновый эффект при наведении
                if (isHovered)
                {
                    using (Pen glowPen = new Pen(Color.Cyan, 4))
                    {
                        g.DrawPath(glowPen, path);
                    }
                }

                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                {
                    g.DrawString(text, font, textBrush, rect, format);
                }
            }
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();

            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
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

            _pressedButton = null;
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            if (_resumeButton.Contains(e.Location))
                _pressedButton = _resumeButton;
            else if (_menuButton.Contains(e.Location))
                _pressedButton = _menuButton;

            _form.Invalidate();
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            _pressedButton = null;
            _form.Invalidate();
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            Rectangle? previousHovered = _hoveredButton;

            if (_resumeButton.Contains(e.Location))
                _hoveredButton = _resumeButton;
            else if (_menuButton.Contains(e.Location))
                _hoveredButton = _menuButton;
            else
                _hoveredButton = null;

            if (previousHovered != _hoveredButton)
                _form.Invalidate();
        }

        public void OnEnter() { }
        public void OnExit() { }
        public void Update() { }
        public void OnResize(EventArgs e) => CalculateButtonPositions();
    }
}
