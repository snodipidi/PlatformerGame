using System;
using System.Drawing;
using System.Windows.Forms;
using PlatformerGame.Forms;

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
            try
            {
                // Загружаем изображение правил
                _rulesImage = new Bitmap("C:\\Users\\msmil\\source\\repos\\PlatformerGame\\PlatformerGame\\Resourses\\rules.png");
            }
            catch
            {
                // Если изображение не загрузилось, можно добавить fallback
                _rulesImage = null;
            }
            UpdateButtonPosition();
        }

        private void UpdateButtonPosition()
        {
            int buttonWidth = 200;
            _backButton = new Rectangle(
                (_form.ClientSize.Width - buttonWidth) / 2,
                _form.ClientSize.Height - 100,
                buttonWidth,
                50);
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPosition();
            _form.Invalidate();
        }

        public void Draw(Graphics g)
        {
            // Полупрозрачный фон
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0, 50)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            // Объявляем переменные до блока if
            int width = 0;
            int height = 0;
            bool imageLoaded = _rulesImage != null;

            if (imageLoaded)
            {
                // Рассчитываем пропорции
                float imageAspect = (float)_rulesImage.Width / _rulesImage.Height;
                float screenAspect = (float)_form.ClientSize.Width / _form.ClientSize.Height;

                // Выбираем способ масштабирования
                if (imageAspect > screenAspect)
                {
                    // Ширина ограничивающий фактор
                    width = (int)(_form.ClientSize.Width * 0.9f);
                    height = (int)(width / imageAspect);
                }
                else
                {
                    // Высота ограничивающий фактор
                    height = (int)(_form.ClientSize.Height * 0.7f);
                    width = (int)(height * imageAspect);
                }

                // Позиционируем по центру
                Rectangle imageRect = new Rectangle(
                    (_form.ClientSize.Width - width) / 2,
                    20, // Отступ сверху
                    width,
                    height);

                // Рисуем сглаженное изображение
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(_rulesImage, imageRect);
            }
            else
            {
                // Fallback текст
                string errorText = "Изображение правил не загружено";
                g.DrawString(errorText, new Font("Arial", 14), Brushes.White,
                    new RectangleF(0, 50, _form.ClientSize.Width, 40), _centerFormat);
            }

            // Кнопка "Назад" (теперь переменные width и height доступны)
            int buttonY = imageLoaded
                ? Math.Min(_form.ClientSize.Height - 100, 20 + height + 40)
                : _form.ClientSize.Height - 100;

            _backButton = new Rectangle(
                (_form.ClientSize.Width - 200) / 2,
                buttonY,
                200,
                50);

            g.FillRectangle(Brushes.LightGray, _backButton);
            g.DrawRectangle(Pens.DarkGray, _backButton);
            g.DrawString("Назад (Esc)", new Font("Arial", 12), Brushes.Black, _backButton, _centerFormat);
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
            _form.Focus();
        }

        public void OnExit()
        {
            _rulesImage?.Dispose();
            _centerFormat.Dispose();
            
        }
    }
}