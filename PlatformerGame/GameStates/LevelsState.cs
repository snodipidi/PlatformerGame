using PlatformerGame.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlatformerGame.GameStates
{
    public class LevelsState : IGameState
    {
        private readonly MainForm _form;
        private Rectangle _backButton;
        private readonly Font _font = new Font("Arial", 24, FontStyle.Bold);

        public LevelsState(MainForm form)
        {
            _form = form;
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            _backButton = new Rectangle(
                _form.ClientSize.Width / 2 - 100,
                _form.ClientSize.Height - 100,
                200,
                50
            );
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
        }

        public void Draw(Graphics g)
        {
            // Фон
            g.Clear(Color.DarkSlateBlue);

            // Текст
            string message = "Уровни будут добавлены в будущем!";
            var size = g.MeasureString(message, _font);
            g.DrawString(message, _font, Brushes.White,
                (_form.ClientSize.Width - size.Width) / 2,
                _form.ClientSize.Height / 3);

            // Кнопка назад
            g.FillRectangle(Brushes.LightGray, _backButton);
            g.DrawRectangle(Pens.DarkGray, _backButton);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_backButton.Contains(e.Location))
            {
                _form.ChangeState(new MainMenuState(_form));
            }
        }

        // Остальные методы интерфейса (можно оставить пустыми)
        public void Update() { }
        public void HandleInput(KeyEventArgs e) { }
        public void OnEnter() { }
        public void OnExit() { }
    }
}