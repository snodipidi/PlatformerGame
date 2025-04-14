using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PlatformerGame.Forms;
using PlatformerGame.GameObjects;

namespace PlatformerGame.GameStates
{
    public class PlayingState : IGameState
    {
        private readonly MainForm _form;
        private readonly Player _player;
        private readonly Level _level;
        private readonly Font _progressFont = new Font("Arial", 12, FontStyle.Bold);

        public PlayingState(MainForm form, Player player, Level level)
        {
            _form = form;
            _player = player;
            _level = level;
        }

        public void Update()
        {
            // Логика обновления вынесена в MainForm.GameLoop
            // Оставляем пустым или можно добавить дополнительную логику
        }

        public void Draw(Graphics g)
        {
            try
            {
                g.TranslateTransform(-_level.CameraOffset, 0);
                _level.Draw(g);
                _player.Draw(g);
                g.ResetTransform();

                // Рисуем индикатор прогресса
                DrawProgressBar(g);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отрисовки: {ex.Message}");
            }
        }

        private void DrawProgressBar(Graphics g)
        {
            // Полоса прогресса
            int barWidth = 200;
            int barHeight = 20;
            int margin = 20;

            Rectangle barRect = new Rectangle(
                _form.ClientSize.Width - barWidth - margin,
                margin,
                barWidth,
                barHeight);

            // Фон полосы
            g.FillRectangle(Brushes.LightGray, barRect);
            g.DrawRectangle(Pens.Black, barRect);

            // Заполненная часть
            float progress = _level.Progress;
            Rectangle filledRect = new Rectangle(
                barRect.X,
                barRect.Y,
                (int)(barRect.Width * progress),
                barRect.Height);

            g.FillRectangle(Brushes.Green, filledRect);

            // Текст с процентами
            string text = $"{progress * 100:0}%";
            var textSize = g.MeasureString(text, _progressFont);
            g.DrawString(text, _progressFont, Brushes.Black,
                barRect.X + barRect.Width / 2 - textSize.Width / 2,
                barRect.Y + barRect.Height / 2 - textSize.Height / 2);

            // Миниатюра флажка в конце полосы
            if (_level.FinishFlag != null && _level.FinishFlag.Width > 0)
            {
                int flagSize = 15;
                g.DrawImage(_level.FinishFlagTexture,
                    barRect.Right - flagSize / 2,
                    barRect.Y - flagSize - 5,
                    flagSize,
                    flagSize * 2);
            }
        }

        public void HandleInput(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    _player.StartMovingLeft();
                    break;
                case Keys.Right:
                    _player.StartMovingRight();
                    break;
                case Keys.Space:
                    _player.Jump();
                    break;
                case Keys.Escape:
                    _form.ShowMainMenu();
                    break;
            }
        }

        public void OnResize(EventArgs e)
        {
            // Для игрового состояния можно добавить логику при необходимости
            // Например, пересчет позиций UI элементов
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            // Можно добавить обработку кликов во время игры
        }

        public void OnEnter()
        {
            _form.Focus();
        }

        public void OnExit()
        {
            // Очистка ресурсов при выходе из состояния
        }
    }
}