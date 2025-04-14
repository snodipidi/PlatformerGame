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
        private Bitmap _backgroundImage;

        public MainForm()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 600);

            try
            {
                _backgroundImage = new Bitmap("C:\\Users\\msmil\\source\\repos\\PlatformerGame\\PlatformerGame\\Resourses\\back1.png");
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

        public void ShowLevelsMenu()
        {
            ChangeState(new LevelsState(this));
        }


    }
}