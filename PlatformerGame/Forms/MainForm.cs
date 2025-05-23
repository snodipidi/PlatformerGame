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
        private Bitmap _backgroundImage;
        private readonly LevelManager _levelManager = new LevelManager();

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

            this.Load += (sender, e) =>
            {
                ShowMainMenu();
                this.Focus();
            };
        }

        private void StartLevel(LevelData levelData)
        {
            _gameTimer?.Stop();
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
            StartLevel(currentLevel);
        }

        private void GameLoop()
        {
            if (_currentState is PlayingState)
            {
                _player.Update();
                _level.Update(_player.Position.X);

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

                this.Invalidate();
            }
        }

        public void CompleteLevel()
        {
            _gameTimer?.Stop();
            if (_currentState is PlayingState playingState)
            {
                playingState.StopGameTimer();
            }

            _levelManager.UnlockNextLevel();
            ChangeState(new LevelCompletedState(this, _levelManager));
        }

        public void GameOver()
        {
            _gameTimer?.Stop();
            if (_currentState is PlayingState playingState)
            {
                playingState.StopGameTimer();
            }

            ChangeState(new GameOverState(this));
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

            _currentState?.Draw(e.Graphics);
            base.OnPaint(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (_currentState is PlayingState playingState)
                {
                    TogglePause();
                    e.Handled = true;
                }
                else if (_currentState is PauseState)
                {
                    TogglePause();
                    e.Handled = true;
                }
            }
            else
            {
                _currentState?.HandleInput(e);
            }
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

        public void TogglePause()
        {
            if (_currentState is PauseState pauseState)
            {
                ChangeState(pauseState.PreviousState);
                _gameTimer.Start();
            }
            else if (_currentState is PlayingState)
            {
                _gameTimer.Stop();
                ChangeState(new PauseState(this, _currentState));
            }
        }

        public void ShowSettings()
        {
            _gameTimer?.Stop();
            ChangeState(new SettingsState(this));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_currentState is PauseState pause)
                pause.OnMouseMove(e);
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_currentState is PauseState pause)
                pause.OnMouseDown(e);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_currentState is PauseState pause)
                pause.OnMouseUp(e);
            base.OnMouseUp(e);
        }

    }
}