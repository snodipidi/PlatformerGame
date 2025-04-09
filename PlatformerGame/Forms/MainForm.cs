using System;
using System.Drawing;
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

        public MainForm()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 600);
            this.DoubleBuffered = true;

            // Инициализация главного меню при запуске
            this.Load += (sender, e) =>
            {
                ShowMainMenu();
                this.Focus();
            };
        }

        public void ShowMainMenu()
        {
            // Остановка игрового таймера, если он был запущен
            _gameTimer?.Stop();

            // Создаем новое состояние меню
            ChangeState(new MainMenuState(this));
        }

        public void StartNewGame()
        {
            // Инициализация игровых объектов
            _level = new Level(ClientSize);
            _player = new Player(_level.StartPlatform);

            // Настройка таймера
            _gameTimer = new GameTimer(16);
            _gameTimer.Update += GameLoop;
            _gameTimer.Start();

            // Переход в игровое состояние
            ChangeState(new PlayingState(this, _player, _level));
        }

        private void GameLoop()
        {
            if (_currentState is PlayingState)
            {
                _player.Update(_level.Platforms);
                _level.Update(_player.Position.X);

                if (_player.HasFallen(ClientSize.Height))
                {
                    GameOver();
                }
            }
            this.Invalidate();
        }


        public void ChangeState(IGameState newState)
        {
            _currentState?.OnExit();
            _currentState = newState;
            _currentState?.OnEnter();
            this.Invalidate();
        }

        public void GameOver()
        {
            ChangeState(new GameOverState(this));
            _gameTimer?.Stop();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_currentState == null)
            {
                e.Graphics.Clear(Color.Black);
                e.Graphics.DrawString("Loading...", Font, Brushes.White, 10, 10);
                return;
            }

            try
            {
                e.Graphics.Clear(Color.SkyBlue);
                _currentState.Draw(e.Graphics);
            }
            catch
            {
                e.Graphics.Clear(Color.Black);
                e.Graphics.DrawString("Game Content", Font, Brushes.White, 10, 10);
            }

            base.OnPaint(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
                ToggleFullScreen();
            else
                _currentState?.HandleInput(e);
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

            if (_isFullScreen && WindowState != FormWindowState.Maximized)
                WindowState = FormWindowState.Maximized;

            // Сообщаем текущему состоянию об изменении размера
            if (_currentState is MainMenuState menuState)
            {
                menuState.OnResize(e);
            }

            Invalidate();
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
    }
}