using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    public class FinalWinState : IGameState
    {
        private readonly MainForm _form;
        private Rectangle _menuButton;
        private Rectangle _exitButton;
        private float _animationPhase;
        private readonly Bitmap _victoryImage;
        public static bool IsSoundEnabled { get; set; } = true;

        public FinalWinState(MainForm form)
        {
            _form = form;
            try
            {
                _victoryImage = new Bitmap("Resourses\\victory.png");
            }
            catch
            {
                _victoryImage = null;
            }
        }

        public void Draw(Graphics g)
        {
            // Анимированный градиентный фон
            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height),
                Color.FromArgb((int)(Math.Sin(_animationPhase) * 50 + 50), 50, 200, 50),
                Color.FromArgb((int)(Math.Cos(_animationPhase) * 50 + 50), 0, 100, 0),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, 0, 0, _form.ClientSize.Width, _form.ClientSize.Height);
            }

            // Изображение победы
            if (_victoryImage != null)
            {
                int imageWidth = _form.ClientSize.Width / 2;
                int imageHeight = (int)(_victoryImage.Height * ((float)imageWidth / _victoryImage.Width));
                g.DrawImage(_victoryImage,
                    (_form.ClientSize.Width - imageWidth) / 2,
                    50,
                    imageWidth,
                    imageHeight);
            }
            else
            {
                using (var font = new Font("Impact", 72, FontStyle.Bold))
                {
                    string text = "ПОБЕДА!";
                    var size = g.MeasureString(text, font);
                    g.DrawString(text, font, Brushes.Gold,
                        (_form.ClientSize.Width - size.Width) / 2, 100);
                }
            }

            // Конфетти
            DrawConfetti(g);

            // Кнопки
            DrawButton(g, _menuButton, "В главное меню", Brushes.Gold, Pens.DarkGoldenrod);
            DrawButton(g, _exitButton, "Выход", Brushes.OrangeRed, Pens.DarkRed);
        }

        private void DrawConfetti(Graphics g)
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                using (var brush = new SolidBrush(color))
                {
                    int size = rand.Next(5, 15);
                    float x = (float)(rand.NextDouble() * _form.ClientSize.Width);
                    float y = (float)(rand.NextDouble() * _form.ClientSize.Height
                        + _animationPhase * 100 % _form.ClientSize.Height);
                    g.FillEllipse(brush, x, y, size, size);
                }
            }
        }

        private void DrawButton(Graphics g, Rectangle rect, string text,
            Brush fill, Pen border)
        {
            // Фон
            using (var path = RoundedRect(rect, 20))
            {
                g.FillPath(fill, path);
                g.DrawPath(border, path);
            }

            // Текст
            using (var font = new Font("Arial", 20, FontStyle.Bold))
            {
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(text, font, Brushes.Black, rect, format);
            }
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                _form.ShowMainMenu();
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_menuButton.Contains(e.Location))
                _form.ShowMainMenu();

            if (_exitButton.Contains(e.Location))
                _form.Close();
        }

        public void OnEnter()
        {
            UpdateButtonPositions();
        }

        public static void PlayVictorySound()
        {
            if (!IsSoundEnabled) return;
            // Добавьте свой звук победы
            SystemSounds.Exclamation.Play(); // временная заглушка
        }

        public void OnExit()
        {
            _victoryImage?.Dispose();
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
        }

        public void Update()
        {
            _animationPhase += 0.05f;
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;
            _menuButton = new Rectangle(
                centerX - 150,
                _form.ClientSize.Height - 200,
                300, 50);

            _exitButton = new Rectangle(
                centerX - 150,
                _form.ClientSize.Height - 130,
                300, 50);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.X + bounds.Width - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.X + bounds.Width - diameter, bounds.Y + bounds.Height - diameter,
                       diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Y + bounds.Height - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}