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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка отрисовки: {ex.Message}");
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