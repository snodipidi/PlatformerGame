using PlatformerGame.Forms;
using PlatformerGame.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public LevelsState(MainForm form, LevelManager levelManager)
        {
            _form = form;
            _levelManager = levelManager;
            UpdateButtonPositions();
        }

        private void UpdateButtonPositions()
        {
            _levelButtons.Clear();
            int centerX = _form.ClientSize.Width / 2;
            int startY = _form.ClientSize.Height / 4; // Начинаем с 1/4 высоты окна

            foreach (var level in _levelManager.GetAllLevels())
            {
                _levelButtons.Add(new Rectangle(
                    centerX - 150,
                    startY,
                    300,
                    50));
                startY += 70;
            }

            // Кнопка "Назад" внизу по центру
            _backButton = new Rectangle(
                centerX - 100,
                _form.ClientSize.Height - 100,
                200,
                50);
        }

        public void OnResize(EventArgs e)
        {
            UpdateButtonPositions();
            _form.Invalidate(); 
        }

        public void Draw(Graphics g)
        {
            g.Clear(Color.DarkSlateBlue);
            var format = new StringFormat { Alignment = StringAlignment.Center };

            // Заголовок
            string header = "Выберите уровень";
            var headerSize = g.MeasureString(header, _buttonFont);
            g.DrawString(header, _buttonFont, Brushes.White,
                (_form.ClientSize.Width - headerSize.Width) / 2,
                50);

            var levels = _levelManager.GetAllLevels();
            for (int i = 0; i < levels.Count; i++)
            {
                var level = levels[i];
                var rect = _levelButtons[i];

                g.FillRectangle(level.IsLocked ? Brushes.Gray : Brushes.LightSteelBlue, rect);
                g.DrawRectangle(Pens.DarkSlateBlue, rect);

                string text = level.IsLocked ?
                    $"Уровень {level.LevelNumber} (заблокирован)" :
                    $"Уровень {level.LevelNumber}";

                g.DrawString(text, _buttonFont,
                    level.IsLocked ? Brushes.DarkGray : Brushes.Black,
                    rect, format);
            }

            g.FillRectangle(Brushes.LightGray, _backButton);
            g.DrawRectangle(Pens.DarkGray, _backButton);
            g.DrawString("Назад", _buttonFont, Brushes.Black, _backButton, format);
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            var levels = _levelManager.GetAllLevels();

            for (int i = 0; i < _levelButtons.Count; i++)
            {
                if (_levelButtons[i].Contains(e.Location) && !levels[i].IsLocked)
                {
                    _levelManager.SetCurrentLevel(i);
                    _form.StartNewGame(); // Используем публичный метод вместо StartLevel
                    return;
                }
            }

            if (_backButton.Contains(e.Location))
            {
                _form.ShowMainMenu();
            }
        }

        public void Update() { }
        public void HandleInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _form.ShowMainMenu();
            }
        }
        public void OnEnter() { }
        public void OnExit()
        {
            _buttonFont.Dispose();
        }
    }
}