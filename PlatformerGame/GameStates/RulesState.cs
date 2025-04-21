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
        private readonly Font _titleFont = new Font("Arial", 32, FontStyle.Bold);
        private readonly Font _textFont = new Font("Arial", 14);
        private readonly StringFormat _centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public RulesState(MainForm form)
        {
            _form = form;
            UpdateButtonPosition();
        }

        private void UpdateButtonPosition()
        {
            // Центрируем кнопку по горизонтали
            int buttonWidth = 200;
            _backButton = new Rectangle(
                (_form.ClientSize.Width - buttonWidth) / 2, // Центральное положение
                _form.ClientSize.Height - 100, // 100px от нижнего края
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
            // Полупрозрачный темный фон
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0, 50)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            // Заголовок (центрированный)
            g.DrawString("Правила игры", _titleFont, Brushes.Gold,
                new RectangleF(0, 50, _form.ClientSize.Width, 60),
                _centerFormat);

            // Основной текст (центрированный)
            string rulesText =
                "Управление:\n\n" +
                "← → - Движение влево/вправо\n" +
                "Space (Пробел) - Прыжок\n" +
                "Зажать Space (Пробел) - двойной прыжок\n" +
                "F11 - Полноэкранный режим\n" +
                "Esc - Выход в меню\n\n" +
                "Цель игры:\n" +
                "Достигнуть флажка в конце уровня!";

            RectangleF textRect = new RectangleF(
                _form.ClientSize.Width * 0.1f, // 10% отступ слева
                150,                           // Отступ сверху
                _form.ClientSize.Width * 0.8f, // 80% ширины
                _form.ClientSize.Height - 300); // Высота

            g.DrawString(rulesText, _textFont, Brushes.White, textRect, _centerFormat);

            // Кнопка "Назад" (уже центрирована через UpdateButtonPosition)
            g.FillRectangle(Brushes.LightGray, _backButton);
            g.DrawRectangle(Pens.DarkGray, _backButton);
            g.DrawString("Назад (Esc)", _textFont, Brushes.Black, _backButton, _centerFormat);
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
            _titleFont.Dispose();
            _textFont.Dispose();
            _centerFormat.Dispose();
        }
    }
}