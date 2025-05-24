using PlatformerGame.Forms;
using PlatformerGame.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PlatformerGame.GameStates
{
    public class LevelsState : IGameState
    {
        private readonly MainForm _form;
        private readonly LevelManager _levelManager;
        private Rectangle _backButton;
        private List<Rectangle> _levelButtons = new List<Rectangle>();
        private readonly Font _buttonFont = new Font("Arial", 12, FontStyle.Bold);
        private int _hoveredIndex = -1;
        private bool _hoveringBack = false;
        private Rectangle _progressBarRect;
        private Rectangle _progressFillRect;
        private const int ProgressBarHeight = 25;
        private const int ProgressBarMargin = 20;

        public LevelsState(MainForm form, LevelManager levelManager)
        {
            _form = form;
            _levelManager = levelManager;
            UpdateButtonPositions();

            // Подписка на события
            _form.MouseMove += (s, e) => HandleMouseMove(e);
            _form.MouseClick += (s, e) => HandleMouseClick(e);
        }

        private void UpdateButtonPositions()
        {
            int centerX = _form.ClientSize.Width / 2;
            _levelButtons.Clear();

            // Стартовая позиция для кнопок уровней
            int startY = _form.ClientSize.Height / 4 - 50;

            foreach (var level in _levelManager.GetAllLevels())
            {
                _levelButtons.Add(new Rectangle(centerX - 150, startY, 300, 50));
                startY += 70;
            }

            // Новые координаты для кнопки "Назад" (выше прогресс-бара)
            _backButton = new Rectangle(
                centerX - 100,
                _form.ClientSize.Height - 180,
                200,
                50
            );

            // Позиция прогресс-бара (ниже кнопки "Назад")
            _progressBarRect = new Rectangle(
                ProgressBarMargin,
                _form.ClientSize.Height - 100, 
                _form.ClientSize.Width - ProgressBarMargin * 2,
                ProgressBarHeight
            );
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
            _form.Invalidate();
        }

        public void Draw(Graphics g)
        {
            // Градиентный фон
            using (var backgroundBrush = new LinearGradientBrush(
                _form.ClientRectangle,
                Color.MidnightBlue,
                Color.Black,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(backgroundBrush, _form.ClientRectangle);
            }

            // Заголовок — с вертикальным градиентом и тенью
            string header = "ВЫБЕРИТЕ УРОВЕНЬ";
            using (var headerFont = new Font("Arial", 30, FontStyle.Bold))
            {
                var headerRect = new Rectangle(0, 20, _form.ClientSize.Width, 60);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                // Тень заголовка (смещённая вниз и вправо)
                using (var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                {
                    g.DrawString(header, headerFont, shadowBrush, new PointF((_form.ClientSize.Width - g.MeasureString(header, headerFont).Width) / 2 + 3, 23));
                }

                // Градиент для текста (сверху голубой, снизу фиолетовый)
                using (var textBrush = new LinearGradientBrush(
                    headerRect,
                    Color.LightSkyBlue,
                    Color.MediumPurple,
                    LinearGradientMode.Vertical))
                {
                    g.DrawString(header, headerFont, textBrush, headerRect, format);
                }
            }

            var formatCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var levels = _levelManager.GetAllLevels();

            for (int i = 0; i < levels.Count; i++)
            {
                var level = levels[i];
                var rect = _levelButtons[i];
                bool hovered = i == _hoveredIndex;

                // Цвет кнопки с плавным переходом на hover
                Color baseColor = level.IsLocked ? Color.Gray : (hovered ? Color.FromArgb(255, 80, 240) : Color.FromArgb(173, 216, 230));
                Color borderColor = hovered ? Color.Cyan : Color.DarkSlateBlue;

                using (var path = RoundedRect(rect, 10))
                using (var brush = new SolidBrush(baseColor))
                using (var pen = new Pen(borderColor, 2))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }

                // Текст уровня — жирный капс, без обводки
                string text = level.IsLocked ?
                    $"УРОВЕНЬ {level.LevelNumber} (ЗАБЛОКИРОВАН)" :
                    $"УРОВЕНЬ {level.LevelNumber}";

                using (var font = new Font("Arial", 14, FontStyle.Bold))
                using (var textBrush = new SolidBrush(level.IsLocked ? Color.DarkGray : (hovered ? Color.White : Color.Black)))
                {
                    g.DrawString(text, font, textBrush, rect, formatCenter);
                }
            }

            // Кнопка Назад
            Color backColor = _hoveringBack ? Color.FromArgb(255, 80, 240) : Color.LightGray;
            Color backBorder = _hoveringBack ? Color.Cyan : Color.DarkGray;

            using (var path = RoundedRect(_backButton, 10))
            using (var brush = new SolidBrush(backColor))
            using (var pen = new Pen(backBorder, 2))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            using (var font = new Font("Arial", 14, FontStyle.Bold))
            {
                g.DrawString("НАЗАД", font, Brushes.Black, _backButton, formatCenter);
            }
            DrawProgressBar(g);
        }

        private void DrawProgressBar(Graphics g)
        {
            int width = _form.ClientSize.Width - ProgressBarMargin * 2;
            _progressBarRect = new Rectangle(
                ProgressBarMargin,
                _form.ClientSize.Height - ProgressBarHeight - ProgressBarMargin,
                width,
                ProgressBarHeight);

            // Фон
            g.FillRectangle(Brushes.DarkSlateGray, _progressBarRect);

            float rawProgress = _levelManager.GetTotalProgress();
            float progress = rawProgress < 0f ? 0f : rawProgress > 1f ? 1f : rawProgress;
            int fillWidth = Math.Max((int)(_progressBarRect.Width * progress), 1); 

            _progressFillRect = new Rectangle(
                _progressBarRect.X,
                _progressBarRect.Y,
                fillWidth,
                _progressBarRect.Height);

            // Фикс для градиента при малой ширине
            if (fillWidth > 1)
            {
                using (var brush = new LinearGradientBrush(
                    _progressFillRect,
                    Color.LimeGreen,
                    Color.DarkGreen,
                    0f))
                {
                    g.FillRectangle(brush, _progressFillRect);
                }
            }
            else
            {
                g.FillRectangle(Brushes.LimeGreen, _progressFillRect);
            }

            // Остальной код без изменений
            g.DrawRectangle(Pens.Black, _progressBarRect);

            using (var font = new Font("Arial", 14, FontStyle.Bold))
            {
                string text = $"Прогресс: {(int)(progress * 100)}%";
                var textSize = g.MeasureString(text, font);
                g.DrawString(text, font, Brushes.White,
                    _progressBarRect.X + (_progressBarRect.Width - textSize.Width) / 2,
                    _progressBarRect.Y - textSize.Height - 5);
            }
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            var levels = _levelManager.GetAllLevels();

            for (int i = 0; i < _levelButtons.Count; i++)
            {
                if (_levelButtons[i].Contains(e.Location) && !levels[i].IsLocked)
                {
                    _levelManager.SetCurrentLevel(levels[i].LevelNumber);
                    _form.StartNewGame();
                    return;
                }
            }

            if (_backButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
        }


        public void HandleMouseMove(MouseEventArgs e)
        {
            _hoveredIndex = -1;
            _hoveringBack = false;

            for (int i = 0; i < _levelButtons.Count; i++)
            {
                if (_levelButtons[i].Contains(e.Location))
                {
                    _hoveredIndex = i;
                    break;
                }
            }

            if (_backButton.Contains(e.Location))
            {
                _hoveringBack = true;
            }

            _form.Invalidate();
        }

        public void Update() { }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _form.ShowMainMenu();
            }
        }

        public void OnEnter()
        {
            // Принудительно обновляем список уровней
            _levelButtons.Clear();
            UpdateButtonPositions();
        }

        public void OnExit()
        {
            _buttonFont.Dispose();
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
