using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    public class MainMenuState : IGameState
    {
        private class NeonButton
        {
            public Rectangle Bounds;
            public string Text;
            public Color BaseColor;
            public Color GlowColor;
            public float HoverProgress;
            public bool IsHovered;
        }

        private readonly MainForm _form;
        private readonly List<NeonButton> _buttons = new List<NeonButton>();
        private readonly Timer _animationTimer;
        private readonly Random _random = new Random();
        private Bitmap _background;
        private float _globalTime;
        private const int ButtonSpacing = 100;

        public MainMenuState(MainForm form)
        {
            _form = form;
            InitializeBackground();
            InitializeButtons();
            _animationTimer = new Timer { Interval = 16 };
            _animationTimer.Tick += (s, e) =>
            {
                _globalTime += 0.05f;
                UpdateButtonStates();
                _form.Invalidate();
            };
            _animationTimer.Start();
        }

        private void InitializeBackground()
        {
            _background?.Dispose();
            _background = new Bitmap(_form.ClientSize.Width, _form.ClientSize.Height);
            using (var g = Graphics.FromImage(_background))
            {
                var darkBlue = Color.FromArgb(15, 20, 40);
                var purple = Color.FromArgb(60, 0, 80);

                using (var brush = new LinearGradientBrush(
                    new Point(0, 0),
                    new Point(_form.ClientSize.Width, _form.ClientSize.Height),
                    darkBlue,
                    purple))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, _background.Width, _background.Height));
                }
            }
        }

        private void InitializeButtons()
        {
            _buttons.Clear();

            var colors = new[]
            {
                Color.Cyan,
                Color.Magenta,
                Color.Yellow,
                Color.OrangeRed
            };

            var buttonTitles = new[] { "НОВАЯ ИГРА", "УРОВНИ", "НАСТРОЙКИ", "ВЫХОД" };
            int centerX = _form.ClientSize.Width / 2;
            int startY = _form.ClientSize.Height / 3;

            for (int i = 0; i < buttonTitles.Length; i++)
            {
                _buttons.Add(new NeonButton
                {
                    Bounds = new Rectangle(centerX - 200, startY, 400, 80),
                    Text = buttonTitles[i],
                    BaseColor = colors[i],
                    GlowColor = Color.FromArgb(100, colors[i]),
                    HoverProgress = 0f
                });
                startY += ButtonSpacing;
            }
        }

        private void UpdateButtonStates()
        {
            foreach (var btn in _buttons)
            {
                btn.HoverProgress = Math.Max(0, Math.Min(1,
                    btn.HoverProgress + (btn.IsHovered ? 0.15f : -0.1f)));
            }
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(_background, 0, 0);
            DrawDynamicEffects(g);

            foreach (var btn in _buttons)
            {
                DrawNeonButton(g, btn);
            }

            DrawTitle(g);
        }

        private void DrawDynamicEffects(Graphics g)
        {
            using (var effectBrush = new SolidBrush(Color.FromArgb(30, 0, 200, 255)))
            {
                float size = 150 + (float)Math.Sin(_globalTime) * 50;
                g.FillEllipse(effectBrush,
                    _form.ClientSize.Width / 2 - size / 2,
                    100 - size / 2,
                    size, size);
            }
        }

        private void DrawNeonButton(Graphics g, NeonButton btn)
        {
            float glow = btn.HoverProgress * 0.8f + 0.2f;
            var bounds = btn.Bounds;

            using (var path = GetRoundedPath(bounds, 20))
            using (var glowBrush = new PathGradientBrush(path))
            {
                glowBrush.CenterColor = Color.FromArgb((int)(150 * glow), btn.BaseColor);
                glowBrush.SurroundColors = new[] { Color.Transparent };
                g.FillPath(glowBrush, path);
            }

            using (var path = GetRoundedPath(bounds, 20))
            using (var brush = new SolidBrush(Color.FromArgb(100, btn.BaseColor)))
            {
                g.FillPath(brush, path);
            }

            using (var path = GetRoundedPath(bounds, 20))
            using (var pen = new Pen(btn.BaseColor, 3 + 3 * glow))
            {
                g.DrawPath(pen, path);
            }

            using (var font = new Font("Arial Black", 24, FontStyle.Bold))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                // Отображаем только 1 раз текст без дополнительного "шума"
                g.DrawString(btn.Text, font, Brushes.White, bounds, format);
            }
        }


        private void DrawTitle(Graphics g)
        {
            using (var font = new Font("Impact", 72, FontStyle.Bold))
            using (var brush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(_form.ClientSize.Width, 0),
                Color.Cyan,
                Color.Magenta))
            {
                string title = "Platformer Game";
                var size = g.MeasureString(title, font);
                var pos = new PointF(
                    (_form.ClientSize.Width - size.Width) / 2,
                    50);

                for (int i = 0; i < 10; i++)
                {
                    g.DrawString(title, font, Brushes.Black,
                        pos.X + (float)Math.Sin(_globalTime + i) * 3,
                        pos.Y + (float)Math.Cos(_globalTime + i) * 3);
                }

                g.DrawString(title, font, brush, pos);
            }
        }


        private GraphicsPath GetRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            path.AddArc(bounds.X + bounds.Width - radius, bounds.Y, radius, radius, 270, 90);
            path.AddArc(bounds.X + bounds.Width - radius, bounds.Y + bounds.Height - radius,
                radius, radius, 0, 90);
            path.AddArc(bounds.X, bounds.Y + bounds.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                _form.StartNewGame();
            else if (e.KeyCode == Keys.Escape)
                _form.Close(); // Оставляем закрытие по ESC только в главном меню
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            HandleMouseClick(e); // вызывает реализацию интерфейса
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            foreach (var btn in _buttons)
            {
                if (btn.Bounds.Contains(e.Location))
                {
                    HandleAction(btn.Text);
                    break;
                }
            }
        }




        private void HandleAction(string buttonText)
        {
            switch (buttonText)
            {
                case "НОВАЯ ИГРА":
                    _form.StartNewGame();
                    break;
                case "УРОВНИ":
                    _form.ShowLevelsMenu();
                    break;
                case "ВЫХОД":
                    _form.Close();
                    break;
                case "НАСТРОЙКИ":
                    _form.ShowSettings();
                    break;
            }
        }


        public void OnEnter()
        {
            _form.MouseMove += HandleMouseMove;
            _form.MouseClick += HandleMouseClick;
        }

        public void OnExit()
        {
            _animationTimer.Stop();
            _form.MouseMove -= HandleMouseMove;
            _form.MouseClick -= HandleMouseClick;
            _background?.Dispose();
        }

        public void OnResize(EventArgs e)
        {
            InitializeBackground();
            InitializeButtons();
        }

        public void Update() { }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            foreach (var btn in _buttons)
            {
                btn.IsHovered = btn.Bounds.Contains(e.Location);
            }
        }
    }
}