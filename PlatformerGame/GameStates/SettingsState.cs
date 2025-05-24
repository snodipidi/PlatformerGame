using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    public class SettingsState : IGameState
    {
        private readonly MainForm _form;
        private Button _backButton;

        private Rectangle _musicToggleRect;
        private Rectangle _devModeToggleRect;

        private const int ToggleWidth = 80;
        private const int ToggleHeight = 40;
        private const int ToggleMargin = 8;
        private const int ElementsSpacing = 60;

        public SettingsState(MainForm form)
        {
            _form = form;
            InitializeControls();
            UpdateTogglePositions();
        }

        private void InitializeControls()
        {
            // Кнопка "Назад"
            _backButton = new Button
            {
                Text = "← Назад",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 60),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 50),
                Location = new Point(20, 20),
                Cursor = Cursors.Hand
            };
            _backButton.FlatAppearance.BorderSize = 0;
            _backButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 90);
            _backButton.Click += (s, e) => _form.ShowMainMenu();
        }

        private void UpdateTogglePositions()
        {
            int centerX = _form.ClientSize.Width / 2;
            int startY = _form.ClientSize.Height / 2 - ToggleHeight;

            _devModeToggleRect = new Rectangle(
                centerX + 80,
                startY,
                ToggleWidth,
                ToggleHeight);

            _musicToggleRect = new Rectangle(
                centerX + 80,
                startY + ElementsSpacing,
                ToggleWidth,
                ToggleHeight);
        }

        public void Draw(Graphics g)
        {
            // Фон с градиентом
            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height),
                Color.FromArgb(10, 20, 50),
                Color.FromArgb(40, 60, 100),
                90f))
            {
                g.FillRectangle(brush, 0, 0, _form.ClientSize.Width, _form.ClientSize.Height);
            }

            // Заголовок
            DrawHeader(g);

            // Переключатели
            DrawToggleWithLabel(g, "Режим разработчика", _devModeToggleRect, SoundManager.DeveloperMode);
            DrawToggleWithLabel(g, "Музыка и звук", _musicToggleRect, SoundManager.IsSoundEnabled);
        }

        private void DrawHeader(Graphics g)
        {
            using (var font = new Font("Segoe UI", 48, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Cyan))
            {
                string title = "Настройки";
                var size = g.MeasureString(title, font);
                g.DrawString(title, font, brush, (_form.ClientSize.Width - size.Width) / 2, 60);
            }
        }

        private void DrawToggleWithLabel(Graphics g, string label, Rectangle rect, bool isOn)
        {
            // Надпись
            using (var font = new Font("Segoe UI", 32, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.LightCyan))
            {
                var textSize = g.MeasureString(label, font);
                g.DrawString(label, font, brush,
                    rect.X - textSize.Width - 20,
                    rect.Y + (rect.Height - textSize.Height) / 2);
            }

            // Переключатель
            DrawToggleSwitch(g, rect, isOn);
        }

        private void DrawToggleSwitch(Graphics g, Rectangle rect, bool isOn)
        {
            Color backColor = isOn ? Color.FromArgb(100, 220, 100) : Color.FromArgb(150, 150, 150);
            Color toggleColor = Color.White;
            int radius = rect.Height / 2;

            using (var path = RoundedRect(rect, radius))
            using (var backBrush = new SolidBrush(backColor))
            {
                g.FillPath(backBrush, path);
            }

            int circleDiameter = rect.Height - ToggleMargin * 2;
            int circleX = isOn ? rect.Right - circleDiameter - ToggleMargin : rect.Left + ToggleMargin;
            int circleY = rect.Top + ToggleMargin;

            using (var toggleBrush = new SolidBrush(toggleColor))
            {
                g.FillEllipse(toggleBrush, circleX, circleY, circleDiameter, circleDiameter);
            }
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

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                _form.ShowMainMenu();
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_devModeToggleRect.Contains(e.Location))
            {
                bool newMode = !SoundManager.DeveloperMode;

                if (newMode)
                {
                    var result = MessageBox.Show(
                        "Включение режима разработчика разблокирует все уровни! Продолжить?",
                        "Предупреждение",
                        MessageBoxButtons.YesNo);

                    if (result != DialogResult.Yes) return;
                }

                SoundManager.DeveloperMode = newMode;
                _form.Invalidate();
            }
            else if (_musicToggleRect.Contains(e.Location))
            {
                // Переключаем звук и музыку вместе
                SoundManager.IsSoundEnabled = !SoundManager.IsSoundEnabled;

                if (!SoundManager.IsSoundEnabled)
                {
                    SoundManager.StopMusic();
                }
                else
                {
                    // Можно добавить воспроизведение музыки из меню или игры по необходимости
                    // Например:
                    // SoundManager.PlayMusic("music1.wav");
                }

                _form.Invalidate();
            }
        }

        public void OnEnter()
        {
            _form.Controls.Add(_backButton);
            _backButton.BringToFront();
        }

        public void OnExit()
        {
            _form.Controls.Remove(_backButton);
            _backButton.Dispose();
        }

        public void OnResize(EventArgs e)
        {
            _backButton.Location = new Point(20, 20);
            UpdateTogglePositions();
            _form.Invalidate();
        }

        public void Update()
        {
            // Не требуется
        }
    }
}
