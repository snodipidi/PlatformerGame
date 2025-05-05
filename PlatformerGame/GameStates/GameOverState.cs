using System;
using System.Drawing;
using System.Windows.Forms;
using PlatformerGame.Forms;

namespace PlatformerGame.GameStates
{
    public class GameOverState : IGameState
    {
        private readonly MainForm _form;
        private Rectangle _retryButton;
        private Rectangle _menuButton; // Новая кнопка для выхода в меню
        private readonly Font _titleFont = new Font("Arial", 32, FontStyle.Bold);
        private readonly Font _buttonFont = new Font("Arial", 12, FontStyle.Bold);

        private void UpdateButtonPosition()
        {
            int centerX = _form.ClientSize.Width / 2;
            int centerY = _form.ClientSize.Height / 2;

            _retryButton = new Rectangle(
                centerX - 100,
                centerY + 10,
                200,
                50
            );

            // Позиционируем кнопку меню под кнопкой рестарта
            _menuButton = new Rectangle(
                centerX - 100,
                centerY + 70, // Отступ 20 пикселей от предыдущей кнопки
                200,
                50
            );
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPosition();
            _form.Invalidate();
        }

        public GameOverState(MainForm form)
        {
            _form = form;
            InitializeUI();
        }

        private void InitializeUI()
        {
            UpdateButtonPosition();
        }

        public void Update() { }

        public void Draw(Graphics g)
        {
            // Полупрозрачный черный фон
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)),
                new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height));

            // Текст "Вы проиграли!"
            string text = "Вы проиграли!";
            var textSize = g.MeasureString(text, _titleFont);
            g.DrawString(text, _titleFont, Brushes.Red,
                (_form.ClientSize.Width - textSize.Width) / 2,
                _form.ClientSize.Height / 2 - 60);

            // Кнопка "Начать заново"
            g.FillRectangle(Brushes.LightGray, _retryButton);
            g.DrawRectangle(Pens.DarkGray, _retryButton);

            // Кнопка "В меню"
            g.FillRectangle(Brushes.LightGray, _menuButton);
            g.DrawRectangle(Pens.DarkGray, _menuButton);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString("Начать заново (R)", _buttonFont, Brushes.Black, _retryButton, format);
            g.DrawString("В меню (M)", _buttonFont, Brushes.Black, _menuButton, format);
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

        public void OnEnter() {
            SoundManager.PlayGameOverSound();
            
        }
        public void OnExit() { }
    }
}