using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlatformerGame.GameObjects;
using PlatformerGame.GameStates;

namespace PlatformerGame.Forms
{
    public partial class MainForm : Form
    {
        private IGameState _currentState;
        private Player _player;
        private Level _level;
        private GameTimer _gameTimer;
        private bool _isFullScreen = false;
        private FormWindowState _previousWindowState;
        private Rectangle _previousBounds;
        private Bitmap _backgroundImage;
        private readonly LevelManager _levelManager = new LevelManager();
        public IGameState CurrentState => _currentState;

        public MainForm()
        {
            InitializeComponent();
            SoundManager.Initialize();
            this.DoubleBuffered = true;
            this.ClientSize = new Size(800, 600);

            try
            {
                _backgroundImage = new Bitmap("Resourses\\back1.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить фон: {ex.Message}");
                _backgroundImage = null;
            }

            this.DoubleBuffered = true;

            this.Load += (sender, e) =>
            {
                ShowMainMenu();
                this.Focus();
            };
        }


        private void StartLevel(LevelData levelData)
        {
            _level = new Level(ClientSize, levelData);
            _player = new Player(_level.StartPlatform, _level); 

            _gameTimer = new GameTimer(16);
            _gameTimer.Update += GameLoop;
            _gameTimer.Start();

            ChangeState(new PlayingState(this, _player, _level, _levelManager));
        }

        public void ShowMainMenu()
        {
            _gameTimer?.Stop();
            ChangeState(new MainMenuState(this));
        }

        public void StartNewGame()
        {
            var currentLevel = _levelManager.GetCurrentLevel();
            Debug.WriteLine($"Запуск уровня {currentLevel.LevelNumber}"); // Для отладки
            StartLevel(currentLevel);
        }

        private void GameLoop()
        {
            if (_currentState is PlayingState)
            {
                _player.Update();
                _level.Update(_player.Position.X);

                // Проверка всех опасных столкновений
                if (_level.CheckPlayerCollision(_player) || _player.HasFallen(ClientSize.Height))
                {
                    GameOver();
                    return;
                }

                if (_player.GetBounds().IntersectsWith(_level.FinishFlag))
                {
                    CompleteLevel();
                    return;
                }

                if (_level.CheckPlayerCollision(_player) || _player.HasFallen(ClientSize.Height))
                {
                    GameOver();
                    return;
                }

                this.Invalidate();
            }

            if (_currentState is PauseState)
            {
                return;
            }
        }

        public void CompleteLevel()
        {
            _levelManager.UnlockNextLevel();
            ChangeState(new LevelCompletedState(this, _levelManager));
            _gameTimer?.Stop();
        }

        public void GameOver()
        {
            ChangeState(new GameOverState(this));
            _gameTimer?.Stop();
        }

        public void ShowLevelsMenu()
        {
            ChangeState(new LevelsState(this, _levelManager));
        }

        public void ShowRules()
        {
            ChangeState(new RulesState(this));
        }

        public void ChangeState(IGameState newState)
        {
            _currentState?.OnExit();
            _currentState = newState;
            _currentState?.OnEnter();
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_backgroundImage != null)
            {
                e.Graphics.DrawImage(_backgroundImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            else
            {
                e.Graphics.Clear(Color.SkyBlue);
            }

            if (_currentState != null)
            {
                _currentState.Draw(e.Graphics);
            }

            base.OnPaint(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.P)
            {
                TogglePause();
            }
            else _currentState?.HandleInput(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (_currentState is PlayingState)
            {
                if (e.KeyCode == Keys.Left) _player.StopMovingLeft();
                if (e.KeyCode == Keys.Right) _player.StopMovingRight();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            _currentState?.HandleMouseClick(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _currentState?.OnResize(e);
            this.Invalidate();
        }

        private void ToggleFullScreen()
        {
            _isFullScreen = !_isFullScreen;
            if (_isFullScreen)
            {
                _previousWindowState = this.WindowState;
                _previousBounds = this.Bounds;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = _previousWindowState;
                this.Bounds = _previousBounds;
            }
            this.Invalidate();
        }
        public void TogglePause()
        {
            if (_currentState is PauseState)
            {
                // Если уже на паузе - продолжаем
                var pauseState = (PauseState)_currentState;
                ChangeState(pauseState.PreviousState);
            }
            else if (_currentState is PlayingState)
            {
                // Если в игре - ставим на паузу
                ChangeState(new PauseState(this, _currentState));
            }
        }
        public void ShowSettings()
        {
            _gameTimer?.Stop();   // Остановим игру, если была запущена
            ChangeState(new SettingsState(this));
        }

    }

}