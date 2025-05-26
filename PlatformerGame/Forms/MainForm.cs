using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlatformerGame.GameObjects;
using PlatformerGame.GameStates;

namespace PlatformerGame.Forms
{
    /// <summary>
    /// Главная форма игры.
    /// Отвечает за управление состояниями игры, обработку ввода, отображение графики и запуск уровней.
    /// </summary>
    public partial class MainForm : Form
    {
        // Текущее состояние игры (менеджер состояний)
        private IGameState _currentState;
        // Игрок
        private Player _player;
        // Текущий уровень
        private Level _level;
        // Таймер игрового цикла
        private GameTimer _gameTimer;
        // Фоновое изображение
        private Bitmap _backgroundImage;
        // Менеджер уровней
        private readonly LevelManager _levelManager = new LevelManager();

        /// <summary>
        /// Конструктор формы MainForm.
        /// </summary>
        public MainForm()
        {
            // Инициализация компонентов формы
            InitializeComponent();
            // Инициализация звукового менеджера
            SoundManager.Initialize();
            // Включение двойной буферизации для плавной отрисовки
            this.DoubleBuffered = true;
            // Установка размера окна
            this.ClientSize = new Size(800, 600); 
            try
            {
                // Попытка загрузить фоновое изображение из файла
                _backgroundImage = new Bitmap("Resourses\\back1.png");
            }
            catch (Exception ex)
            {
                // В случае ошибки — показать сообщение и задать null
                MessageBox.Show($"Не удалось загрузить фон: {ex.Message}");
                _backgroundImage = null;
            }
            // Обработчик события загрузки формы
            this.Load += (sender, e) =>
            {
                // Показать главное меню при загрузке формы
                ShowMainMenu();
                // Установить фокус на форму
                this.Focus();
            };

        }

        /// <summary>
        /// Запускает уровень с указанными данными.
        /// </summary>
        /// <param name="levelData">Данные уровня для инициализации.</param>
        private void StartLevel(LevelData levelData)
        {
            // Останавливаем текущий таймер игры, если он есть
            _gameTimer?.Stop();
            // Создаем новый уровень с размерами формы и данными уровня
            _level = new Level(ClientSize, levelData);
            // Создаем игрока на стартовой платформе уровня
            _player = new Player(_level.StartPlatform, _level);
            // Инициализируем игровой таймер с интервалом 16 мс (~60 FPS)
            _gameTimer = new GameTimer(16);
            // Подписываемся на событие обновления таймера
            _gameTimer.Update += GameLoop;
            // Запускаем таймер
            _gameTimer.Start();
            // Меняем состояние игры на PlayingState
            ChangeState(new PlayingState(this, _player, _level, _levelManager));
        }

        /// <summary>
        /// Показывает главное меню игры.
        /// </summary>
        public void ShowMainMenu()
        {
            // Останавливаем таймер игры, если он есть
            _gameTimer?.Stop();
            // Меняем состояние игры на главное меню
            ChangeState(new MainMenuState(this));
        }

        /// <summary>
        /// Запускает новую игру, загружая текущий уровень из менеджера уровней.
        /// </summary>
        public void StartNewGame()
        {
            // Получаем текущий уровень из менеджера
            var currentLevel = _levelManager.GetCurrentLevel();
            // Запускаем уровень
            StartLevel(currentLevel);
        }

        /// <summary>
        /// Главный игровой цикл, вызываемый таймером.
        /// </summary>
        private void GameLoop()
        {
            // Проверяем, что текущее состояние — игра
            if (_currentState is PlayingState)
            {
                // Обновляем состояние игрока
                _player.Update();
                // Обновляем уровень, учитывая позицию игрока по X
                _level.Update(_player.Position.X);
                // Проверяем столкновение игрока с объектами уровня или падение
                if (_level.CheckPlayerCollision(_player) || _player.HasFallen(ClientSize.Height))
                {
                    // Если игрок погиб, запускаем обработку проигрыша
                    GameOver();
                    return;
                }
                // Проверяем достижение финишного флага
                if (_player.GetBounds().IntersectsWith(_level.FinishFlag))
                {
                    // Завершаем уровень
                    CompleteLevel();
                    return;
                }
                // Запрашиваем перерисовку формы
                this.Invalidate();
            }
        }

        /// <summary>
        /// Завершает текущий уровень и переходит к следующему состоянию.
        /// Если достигнут последний уровень (номер 5), переходит к финальному экрану победы.
        /// Иначе разблокирует следующий уровень и показывает экран завершения уровня.
        /// Останавливает игровой таймер.
        /// </summary>
        public void CompleteLevel()
        {
            // Проверяем, является ли текущий уровень последним (номер 5)
            if (_levelManager.GetCurrentLevel().LevelNumber == 5)
            {
                // Переход к состоянию финальной победы
                ChangeState(new FinalWinState(this));
            }
            else
            {
                // Разблокируем следующий уровень
                _levelManager.UnlockNextLevel();
                // Переход к состоянию завершения уровня
                ChangeState(new LevelCompletedState(this, _levelManager));
            }
            // Останавливаем таймер игры
            _gameTimer?.Stop();
        }

        /// <summary>
        /// Обрабатывает ситуацию проигрыша игрока.
        /// Останавливает игровой таймер и переходит в состояние "Игра окончена".
        /// </summary>
        public void GameOver()
        {
            // Останавливаем игровой таймер, если он есть
            _gameTimer?.Stop();
            // Если текущее состояние — игра, останавливаем таймер внутри PlayingState
            if (_currentState is PlayingState playingState)
            {
                playingState.StopGameTimer();
            }
            // Переходим в состояние "Игра окончена"
            ChangeState(new GameOverState(this));
        }

        /// <summary>
        /// Показывает меню выбора уровней, меняя текущее состояние на LevelsState.
        /// </summary>
        public void ShowLevelsMenu()
        {
            // Меняем состояние на LevelsState с передачей текущего экземпляра и менеджера уровней
            ChangeState(new LevelsState(this, _levelManager));
        }

        /// <summary>
        /// Меняет текущее состояние игры.
        /// </summary>
        /// <param name="newState">Новое состояние игры.</param>
        public void ChangeState(IGameState newState)
        {
            // Вызываем выход из текущего состояния, если оно задано
            _currentState?.OnExit();
            // Устанавливаем новое состояние
            _currentState = newState;
            // Вызываем вход в новое состояние, если оно задано
            _currentState?.OnEnter();
            // Запрашиваем перерисовку формы
            this.Invalidate();
        }

        /// <summary>
        /// Переопределение метода отрисовки формы.
        /// </summary>
        /// <param name="e">Аргументы события отрисовки.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Если задано фоновое изображение
            if (_backgroundImage != null)
            {
                // Рисуем фоновое изображение с масштабированием на весь клиент
                e.Graphics.DrawImage(_backgroundImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            else
            {
                // Иначе заливаем фон сплошным цветом (небесно-голубым)
                e.Graphics.Clear(Color.SkyBlue);
            }
            // Вызываем отрисовку текущего состояния, если оно задано
            _currentState?.Draw(e.Graphics);
            // Вызываем базовый метод отрисовки
            base.OnPaint(e);
        }

        /// <summary>
        /// Обработка события нажатия клавиши.
        /// </summary>
        /// <param name="e">Аргументы события нажатия клавиши.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Если нажата клавиша Escape
            if (e.KeyCode == Keys.Escape)
            {
                // Если текущее состояние — игра в процессе
                if (_currentState is PlayingState playingState)
                {
                    TogglePause(); // Переключаем паузу
                    e.Handled = true; // Помечаем событие как обработанное
                }
                // Если текущее состояние — пауза
                else if (_currentState is PauseState)
                {
                    TogglePause(); // Переключаем паузу
                    e.Handled = true; // Помечаем событие как обработанное
                }
            }
            else
            {
                // Передаем обработку клавиш в текущее состояние, если оно задано
                _currentState?.HandleInput(e);
            }
        }

        /// <summary>
        /// Обработка события отпускания клавиши.
        /// </summary>
        /// <param name="e">Аргументы события отпускания клавиши.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // Если текущее состояние — игра в процессе
            if (_currentState is PlayingState)
            {
                // Если отпущена клавиша влево — останавливаем движение игрока влево
                if (e.KeyCode == Keys.Left) _player.StopMovingLeft();
                // Если отпущена клавиша вправо — останавливаем движение игрока вправо
                if (e.KeyCode == Keys.Right) _player.StopMovingRight();
            }
        }

        /// <summary>
        /// Обработка события клика мыши.
        /// </summary>
        /// <param name="e">Аргументы события клика мыши.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Передаем обработку клика мыши в текущее состояние, если оно задано
            _currentState?.HandleMouseClick(e);
        }

        /// <summary>
        /// Обработка события изменения размера формы.
        /// </summary>
        /// <param name="e">Аргументы события изменения размера.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Передаем событие текущему состоянию
            _currentState?.OnResize(e);
            // Запрашиваем перерисовку формы
            this.Invalidate();
        }

        /// <summary>
        /// Переключение состояния паузы игры.
        /// </summary>
        public void TogglePause()
        {
            // Если текущее состояние — пауза
            if (_currentState is PauseState pauseState)
            {
                // Возвращаемся к предыдущему состоянию
                ChangeState(pauseState.PreviousState);
                // Запускаем таймер игры
                _gameTimer.Start();
            }
            // Если текущее состояние — игра в процессе
            else if (_currentState is PlayingState)
            {
                // Останавливаем таймер игры
                _gameTimer.Stop();
                // Переходим в состояние паузы
                ChangeState(new PauseState(this, _currentState));
            }
        }

        /// <summary>
        /// Показать экран настроек.
        /// </summary>
        public void ShowSettings()
        {
            // Останавливаем таймер игры, если он есть
            _gameTimer?.Stop();
            // Переходим в состояние настроек
            ChangeState(new SettingsState(this));
        }

        /// <summary>
        /// Обработка движения мыши.
        /// </summary>
        /// <param name="e">Аргументы события движения мыши.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Передаем событие паузе, если текущее состояние пауза
            if (_currentState is PauseState pause)
                pause.OnMouseMove(e);
            // Вызываем базовую реализацию
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Обработка нажатия кнопки мыши.
        /// </summary>
        /// <param name="e">Аргументы события нажатия мыши.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Передаем событие паузе, если текущее состояние пауза
            if (_currentState is PauseState pause)
                pause.OnMouseDown(e);
            // Вызываем базовую реализацию
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Обработка отпускания кнопки мыши.
        /// </summary>
        /// <param name="e">Аргументы события отпускания мыши.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Передаем событие паузе, если текущее состояние пауза
            if (_currentState is PauseState pause)
                pause.OnMouseUp(e);
            // Вызываем базовую реализацию
            base.OnMouseUp(e);
        }
    }
}