using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using PlatformerGame.Forms;
using System.IO;

namespace PlatformerGame.GameStates
{
    public class RulesState : IGameState
    {
        private readonly MainForm _form;
        private Rectangle _backButton;
        private Bitmap _rulesImage;
        private readonly StringFormat _centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public RulesState(MainForm form)
        {
            _form = form;
            LoadRulesImage();
        }

        private void LoadRulesImage()
        {
            try
            {
                // Путь к изображению в папке Resourses
                string imagePath = Path.Combine(Application.StartupPath, "Resourses", "rules2.png");

                if (File.Exists(imagePath))
                {
                    _rulesImage = new Bitmap(imagePath);
                }
                else
                {
                    throw new FileNotFoundException("Файл rules2.png не найден");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения правил: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _rulesImage = null;
            }
        }

        public void Draw(Graphics g)
        {
            // Заливка фона (на случай если изображение не загрузится)
            g.Clear(Color.FromArgb(30, 30, 40));

            if (_rulesImage != null)
            {
                // Режим высококачественного масштабирования
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                // Рассчитываем размеры для сохранения пропорций
                float imageRatio = (float)_rulesImage.Width / _rulesImage.Height;
                float screenRatio = (float)_form.ClientSize.Width / _form.ClientSize.Height;

                Rectangle destRect;

                if (imageRatio > screenRatio)
                {
                    // Ширина изображения определяет масштаб
                    int height = (int)(_form.ClientSize.Width / imageRatio);
                    destRect = new Rectangle(
                        0,
                        (_form.ClientSize.Height - height) / 2,
                        _form.ClientSize.Width,
                        height);
                }
                else
                {
                    // Высота изображения определяет масштаб
                    int width = (int)(_form.ClientSize.Height * imageRatio);
                    destRect = new Rectangle(
                        (_form.ClientSize.Width - width) / 2,
                        0,
                        width,
                        _form.ClientSize.Height);
                }

                // Рисуем изображение
                g.DrawImage(_rulesImage, destRect);
            }
            else
            {
                // Fallback текст если изображение не загрузилось
                string errorText = "Изображение правил не загружено\n(rules2.png в папке Resourses)";
                g.DrawString(errorText,
                    new Font("Arial", 20, FontStyle.Bold),
                    Brushes.White,
                    new RectangleF(0, 0, _form.ClientSize.Width, _form.ClientSize.Height),
                    _centerFormat);
            }

            // Рисуем кнопку "Назад"
            DrawBackButton(g);
        }

        private void DrawBackButton(Graphics g)
        {
            _backButton = new Rectangle(
                (_form.ClientSize.Width - 200) / 2,
                _form.ClientSize.Height - 80,
                200,
                50);

            // Стиль кнопки
            using (var brush = new LinearGradientBrush(
                _backButton,
                Color.LightGray,
                Color.DarkGray,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, _backButton);
            }

            g.DrawRectangle(Pens.Black, _backButton);

            // Текст кнопки
            g.DrawString("Назад (Esc)",
                new Font("Arial", 12, FontStyle.Bold),
                Brushes.Black,
                _backButton,
                _centerFormat);
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_backButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _form.ShowMainMenu();
            }
        }

        public void Update() { }

        public void OnEnter()
        {
            // При входе в состояние перезагружаем изображение
            _rulesImage?.Dispose();
            LoadRulesImage();
        }

        public void OnExit()
        {
            _rulesImage?.Dispose();
            _centerFormat.Dispose();
        }

        public void OnResize(EventArgs e)
        {
            _form.Invalidate();
        }
    }
}