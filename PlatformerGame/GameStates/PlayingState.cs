using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using PlatformerGame.Forms;
using PlatformerGame.GameObjects;

namespace PlatformerGame.GameStates
{
    public class PlayingState : IGameState
    {
        private readonly MainForm _form;
        private readonly Player _player;
        private readonly Level _level;
        private readonly LevelManager _levelManager;
        private readonly Font _levelFont = new Font("Arial", 16, FontStyle.Bold);
        private readonly Font _progressFont = new Font("Arial", 12, FontStyle.Bold);

        public PlayingState(MainForm form, Player player, Level level, LevelManager levelManager)
        {
            _form = form;
            _player = player;
            _level = level;
            _levelManager = levelManager;
        }

        private void DrawHUD(Graphics g) // Добавляем метод DrawHUD
        {
            // Пример реализации HUD
            using (var font = new Font("Arial", 16, FontStyle.Bold))
            {
                g.DrawString($"Уровень: {_levelManager.GetCurrentLevel().LevelNumber}",
                    font, Brushes.White, 20, 20);
            }
        }

        public void Draw(Graphics g)
        {
            try
            {
                g.TranslateTransform(-_level.CameraOffset, 0);
                _level.Draw(g);
                _player.Draw(g);
                g.ResetTransform();

                // Если игра на паузе - не рисуем HUD
                if (!(_form.CurrentState is PauseState))
                {
                    // Отрисовка HUD (очки, время и т.д.)
                    DrawHUD(g);
                }
                DrawProgressBar(g);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отрисовки: {ex.Message}");
            }

        }

        private void DrawProgressBar(Graphics g)
        {
            int barWidth = 200;
            int barHeight = 15;
            int margin = 10;

            Rectangle barRect = new Rectangle(
                _form.ClientSize.Width - barWidth - margin,
                margin,
                barWidth,
                barHeight);

            // Фон прогресс-бара
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
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.P)
            {
                return;
            }
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

        public void Update() { }
        public void HandleMouseClick(MouseEventArgs e) { }
        public void OnEnter() { }
        public void OnExit()
        {
            _levelFont.Dispose();
            _progressFont.Dispose();
        }
        public void OnResize(EventArgs e) { }
    }
}