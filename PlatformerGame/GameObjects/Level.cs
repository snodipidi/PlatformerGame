using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Linq;

namespace PlatformerGame.GameObjects
{
    /// <summary>
    /// Класс Level представляет уровень игры с платформами, ловушками, врагами и прочими элементами.
    /// </summary>
    public class Level
    {
        /// <summary>
        /// Начальная платформа, с которой игрок стартует.
        /// </summary>
        public Rectangle StartPlatform { get; }

        /// <summary>
        /// Список всех платформ уровня.
        /// </summary>
        public List<Rectangle> Platforms { get; } = new List<Rectangle>();

        /// <summary>
        /// Смещение камеры относительно начала уровня.
        /// </summary>
        public int CameraOffset { get; set; }

        /// <summary>
        /// Прямоугольник, обозначающий финишный флаг.
        /// </summary>
        public Rectangle FinishFlag { get; private set; }

        /// <summary>
        /// Флаг, показывающий, завершён ли уровень.
        /// </summary>
        public bool IsLevelCompleted { get; private set; }

        /// <summary>
        /// Общая длина уровня в пикселях.
        /// </summary>
        public int TotalLength { get; private set; }

        /// <summary>
        /// Текстура финишного флага.
        /// </summary>
        public Bitmap FinishFlagTexture => _finishFlagTexture;

        /// <summary>
        /// Стартовая позиция игрока по оси X.
        /// </summary>
        public float StartPosition => StartPlatform.X + 50;

        /// <summary>
        /// Список всех ловушек (шипов) на уровне.
        /// </summary>
        public List<Rectangle> Traps { get; } = new List<Rectangle>();

        /// <summary>
        /// Список всех врагов на уровне.
        /// </summary>
        public List<Enemy> Enemies { get; } = new List<Enemy>();

        /// <summary>
        /// Список врагов, движущихся по колоннам вверх-вниз.
        /// </summary>
        public List<ColumnEnemy> ColumnEnemies { get; } = new List<ColumnEnemy>();

        /// <summary>
        /// Список всех движущихся платформ.
        /// </summary>
        public List<MovingPlatform> MovingPlatforms { get; } = new List<MovingPlatform>();

        /// <summary>
        /// Генератор случайных чисел.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// X-координата конца последней платформы.
        /// </summary>
        private int lastPlatformX;

        /// <summary>
        /// Размеры экрана (ширина и высота).
        /// </summary>
        private readonly Size screenSize;

        /// <summary>
        /// Текстура блока (платформы).
        /// </summary>
        private readonly Bitmap _blockTexture;

        /// <summary>
        /// Текстура финишного флага.
        /// </summary>
        private readonly Bitmap _finishFlagTexture;

        /// <summary>
        /// Ширина зоны финиша.
        /// </summary>
        private const int FinishAreaWidth = 300;

        /// <summary>
        /// Текущий прогресс игрока в уровне (0.0 - 1.0).
        /// </summary>
        public float Progress
        {
            get
            {
                // Если уровень слишком короткий, считаем прогресс завершённым
                if (TotalLength <= StartPosition) return 1f;
                // Общая дистанция от старта до финиша
                float totalDistance = FinishFlag.X - StartPosition;
                // Пройденная дистанция (учитываем смещение камеры)
                float traveledDistance = CameraOffset + (screenSize.Width / 3) - StartPosition;
                // Вычисляем прогресс
                float progress = traveledDistance / totalDistance;
                return Math.Min(1f, Math.Max(0f, progress));
            }
        }

        /// <summary>
        /// Инициализирует новый уровень с заданными размерами экрана и параметрами уровня.
        /// В зависимости от номера уровня, создаёт уникальные платформы, ловушки, врагов и другие препятствия.
        /// </summary>
        /// <param name="screenSize">Размер экрана, на котором отображается уровень.</param>
        /// <param name="data">Объект данных уровня, включающий номер уровня, сложность и количество платформ.</param>
        public Level(Size screenSize, LevelData data = null)
        {
            // Пытаемся загрузить текстуры блоков и флажка финиша
            try
            {
                _blockTexture = new Bitmap("Resourses\\block.png");
                _finishFlagTexture = new Bitmap("Resourses\\finish_flag.png");
            }
            // Если не удалось — оставляем null
            catch
            {
                _blockTexture = null;
                _finishFlagTexture = null;
            }
            // Сохраняем размер экрана
            this.screenSize = screenSize;
            // Устанавливаем общую длину уровня (по умолчанию — 3000)
            TotalLength = data?.Length ?? 3000;
            // Стартовая платформа
            StartPlatform = new Rectangle(0, screenSize.Height - 100, 300, 20);
            Platforms.Add(StartPlatform);
            lastPlatformX = StartPlatform.Right;

            if (data?.LevelNumber == 2) // Генерация для 2 уровня
            {
                int groundY = screenSize.Height - 100;
                int platformHeight = 20;
                int platformWidth = 300; // Ширина всех платформ
                int trapWidth = 100;    // Ширина ловушки
                int trapHeight = 25;    // Высота ловушки

                // 1. Стартовая безопасная зона
                Platforms.Add(new Rectangle(300, groundY - 50, platformWidth, platformHeight));
                Platforms.Add(new Rectangle(650, groundY - 100, platformWidth, platformHeight));

                // 2. Первая платформа с ловушкой по центру
                int platform1Y = groundY - 150;
                Platforms.Add(new Rectangle(1000, platform1Y, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1000 + (platformWidth - trapWidth) / 2, // Центрирование
                    platform1Y - trapHeight,
                    trapWidth,
                    trapHeight));

                // 3. Вторая платформа с ловушкой по центру
                int platform2Y = groundY - 130;
                Platforms.Add(new Rectangle(1600, platform2Y, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1600 + (platformWidth - trapWidth) / 2,
                    platform2Y - trapHeight,
                    trapWidth,
                    trapHeight));

                // 4. Третья платформа с ловушкой по центру
                int platform3Y = groundY - 170;
                Platforms.Add(new Rectangle(2200, platform3Y, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    2200 + (platformWidth - trapWidth) / 2,
                    platform3Y - trapHeight,
                    trapWidth,
                    trapHeight));

                // 5. Финальные безопасные платформы
                Platforms.Add(new Rectangle(2800, groundY - 100, platformWidth, platformHeight));
                Platforms.Add(new Rectangle(3200, groundY - 80, platformWidth, platformHeight));

                TotalLength = 3800;
                lastPlatformX = TotalLength;
            }

            else
            {
                // Обычная генерация для других уровней
                int platformCount = data?.PlatformCount ?? 20;
                int difficultyLevel = data?.Difficulty ?? 1;

                for (int i = 0; i < platformCount; i++)
                    GeneratePlatform(difficultyLevel);
            }

            GenerateFinalPlatforms(data?.Difficulty ?? 1);
            CreateFinishFlag();

            if (data?.LevelNumber == 3)
            {
                int groundY = screenSize.Height - 100;
                int platformHeight = 20;
                int spikesHeight = 25;

                // 1. Стартовая зона (2 платформы)
                Platforms.Add(new Rectangle(300, groundY - 50, 200, platformHeight));
                Platforms.Add(new Rectangle(550, groundY - 100, 180, platformHeight));

                // 2. Первый враг (единственный)
                int enemyPlatformY = groundY - 120;
                Platforms.Add(new Rectangle(900, enemyPlatformY, 220, platformHeight));
                Enemies.Add(new Enemy(
                    950,                    // X позиция
                    enemyPlatformY,         // Y верхнего края платформы
                    150,                    // Диапазон движения
                    3));                    // Скорость

                // 3. Первые шипы (удлиненная платформа)
                int spikes1PlatformY = groundY - 100;
                int spikes1PlatformWidth = 250; // Увеличили ширину
                Platforms.Add(new Rectangle(1300, spikes1PlatformY, spikes1PlatformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1300 + (spikes1PlatformWidth - 100) / 2, // Центрируем шипы
                    spikes1PlatformY - spikesHeight,
                    100,  // Ширина шипов
                    spikesHeight));

                // 4. Вторые шипы (удлиненная платформа)
                int spikes2PlatformY = groundY - 100;
                int spikes2PlatformWidth = 300; // Увеличили ширину
                Platforms.Add(new Rectangle(1700, spikes2PlatformY, spikes2PlatformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1700 + (spikes2PlatformWidth - 100) / 2,
                    spikes2PlatformY - spikesHeight,
                    100,  // Ширина шипов
                    spikesHeight));

                // 5. Широкие платформы для передышки
                Platforms.Add(new Rectangle(2200, groundY - 120, 350, platformHeight));
                Platforms.Add(new Rectangle(2650, groundY - 80, 300, platformHeight));

                // 6. Финишная платформа
                Platforms.Add(new Rectangle(3050, groundY - 60, 250, platformHeight));

                TotalLength = 3500; // Немного увеличили длину
                lastPlatformX = TotalLength;
            }

            if (data?.LevelNumber == 4) // Генерация 4 уровня с фиксированными платформами
            {
                int groundY = screenSize.Height - 100;
                int platformHeight = 20;
                int spikesHeight = 25;
                int platformWidth = 200;

                // 1. Стартовая зона (3 платформы)
                Platforms.Add(new Rectangle(300, groundY - 50, platformWidth, platformHeight));
                Platforms.Add(new Rectangle(600, groundY - 100, platformWidth, platformHeight));
                Platforms.Add(new Rectangle(900, groundY - 80, platformWidth, platformHeight));

                // 2. Первая колонна с платформой
                int column1X = 1200;
                Platforms.Add(new Rectangle(column1X, groundY - 120, platformWidth, platformHeight));
                ColumnEnemies.Add(new ColumnEnemy(
                    column1X + platformWidth / 2, // Центр платформы
                    groundY - 120,
                    30,  // Ширина колонны
                    150, // Высота колонны
                    120, // Диапазон движения
                    2    // Скорость
                ));

                // 3. Шипы после первой колонны
                Platforms.Add(new Rectangle(1600, groundY - 100, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    1600 + (platformWidth - 100) / 2, // Центрированные шипы
                    groundY - 100 - spikesHeight,
                    100, // Ширина шипов
                    spikesHeight
                ));

                // 4. Враг на отдельной платформе
                int enemyPlatformX = 2000;
                Platforms.Add(new Rectangle(enemyPlatformX, groundY - 150, platformWidth, platformHeight));
                Enemies.Add(new Enemy(
                    enemyPlatformX + 20, // Позиция X
                    groundY - 150,      // Y платформы
                    100,                // Диапазон движения
                    2                   // Скорость
                ));

                // 5. Вторая колонна с платформой
                int column2X = 2500;
                Platforms.Add(new Rectangle(column2X, groundY - 80, platformWidth, platformHeight));
                ColumnEnemies.Add(new ColumnEnemy(
                    column2X + platformWidth / 2,
                    groundY - 80,
                    35, 180, 150, 3 // Более высокая и быстрая колонна
                ));

                // 6. Финальные платформы с шипами
                Platforms.Add(new Rectangle(3000, groundY - 100, platformWidth, platformHeight));
                Traps.Add(new Rectangle(
                    3000 + (platformWidth - 120) / 2,
                    groundY - 100 - spikesHeight,
                    120,
                    spikesHeight
                ));
                Platforms.Add(new Rectangle(3400, groundY - 60, 250, platformHeight));

                TotalLength = 3800;
                lastPlatformX = TotalLength;
            }

            if (data?.LevelNumber == 5)
            {
                int groundY = screenSize.Height - 100;
                int platformHeight = 20;
                int trapHeight = 25; // Высота ловушек-шипов

                // 1. Стартовые платформы (как в оригинале)
                Platforms.Add(new Rectangle(0, groundY, 300, platformHeight));
                Platforms.Add(new Rectangle(400, groundY - 100, 180, platformHeight));

                // 2. Движущиеся платформы (оригинальные параметры)
                MovingPlatforms.Add(new MovingPlatform(
                    600, groundY - 150,
                    120, platformHeight,
                    200, 3, true)); // Вертикальная

                MovingPlatforms.Add(new MovingPlatform(
                    900, groundY - 100,
                    150, platformHeight,
                    300, 4, false)); // Горизонтальная

                // 3. Центральный блок
                Platforms.Add(new Rectangle(1200, groundY - 80, 200, platformHeight));

                // 4. Враг (оригинальные параметры)
                Enemies.Add(new Enemy(
                    1350, groundY - 80,
                    150, 3));

                // 5. Колонна (слегка уменьшенная)
                int columnX = 1600;
                Platforms.Add(new Rectangle(columnX - 40, groundY - 20, 100, platformHeight));
                ColumnEnemies.Add(new ColumnEnemy(
                    columnX, groundY - 180,
                    30, 150, 120, 2));

                // 6. Новая ловушка после колонны
                Platforms.Add(new Rectangle(columnX + 250, groundY - 120, 200, platformHeight));
                Traps.Add(new Rectangle(
                    columnX + 250 + 50, // Центр платформы
                    groundY - 120 - trapHeight,
                    100, trapHeight)); // Шипы шириной 100px

                // 7. Финишные платформы
                Platforms.Add(new Rectangle(2100, groundY - 100, 250, platformHeight));
                Platforms.Add(new Rectangle(2500, groundY - 60, 250, platformHeight));

                TotalLength = 3000;
                lastPlatformX = TotalLength;
                CreateFinishFlag();
            }
        }

        /// <summary>
        /// Генерирует одну платформу на уровне с учётом сложности.
        /// </summary>
        /// <param name="difficulty">Уровень сложности, влияющий на расстояния и размеры платформ.</param>
        private void GeneratePlatform(int difficulty)
        {
            // Случайная ширина платформы, уменьшается с ростом сложности
            int width = random.Next(80, 150 - difficulty * 10);
            // Случайное расстояние до следующей платформы, уменьшается с ростом сложности
            int x = lastPlatformX + random.Next(100, 250 - difficulty * 20);
            // Случайная вертикальная позиция платформы
            int y = random.Next(screenSize.Height / 2, screenSize.Height - 50);
            // Добавляем новую платформу в список
            Platforms.Add(new Rectangle(x, y, width, 20));
            // Обновляем позицию последней платформы
            lastPlatformX = x + width;
        }

        /// <summary>
        /// Генерирует финальную платформу и несколько прыжковых перед финишем.
        /// </summary>
        /// <param name="difficulty">Уровень сложности, не влияет на финальные платформы, но требуется для совместимости.</param>
        private void GenerateFinalPlatforms(int difficulty)
        {
            // Задаём ширину финальной платформы
            int finalPlatformWidth = 200;
            // Вычисляем её горизонтальную позицию — у самого конца уровня
            int finalPlatformX = TotalLength - finalPlatformWidth;
            // Задаём вертикальную позицию финальной платформы
            int finalPlatformY = screenSize.Height - 120;
            // Добавляем финальную платформу
            Platforms.Add(new Rectangle(finalPlatformX, finalPlatformY, finalPlatformWidth, 20));
            // Генерируем 3 небольших платформы перед финишем
            for (int i = 1; i <= 3; i++)
            {
                // Вычисляем X-координату платформы — слева от финишной
                int x = finalPlatformX - 150 * i;
                // Поочередно приподнимаем каждую вторую платформу выше
                int y = finalPlatformY - (i % 2 == 0 ? 50 : 0);
                // Добавляем маленькую платформу
                Platforms.Add(new Rectangle(x, y, 80, 15));
            }
            // Обновляем позицию последней платформы
            lastPlatformX = TotalLength;
        }


        /// <summary>
        /// Создаёт финишный флаг на последней платформе.
        /// </summary>
        private void CreateFinishFlag()
        {
            // Получаем последнюю платформу из списка
            var lastPlatform = Platforms[Platforms.Count - 1];
            // Создаём прямоугольник финишного флага чуть правее и выше платформы
            FinishFlag = new Rectangle(
                lastPlatform.Right - 25,
                lastPlatform.Y - 60,
                30,
                60);
        }

        /// <summary>
        /// Обновляет смещение камеры и обновляет врагов и платформы.
        /// </summary>
        /// <param name="playerX">Позиция игрока по оси X.</param>
        public void Update(float playerX)
        {
            // Вычисляем смещение камеры, чтобы игрок был ближе к левому краю
            CameraOffset = (int)(playerX - screenSize.Width / 3);
            // Ограничиваем смещение, чтобы не выйти за пределы уровня
            int maxOffset = TotalLength - screenSize.Width;
            CameraOffset = Math.Min(maxOffset, Math.Max(0, CameraOffset));
            // Обновляем обычных врагов
            foreach (var enemy in Enemies)
            {
                enemy.Update();
            }
            // Обновляем врагов, привязанных к колоннам
            foreach (var enemy in ColumnEnemies)
            {
                enemy.Update();
            }
            // Обновляем движущиеся платформы
            foreach (var platform in MovingPlatforms)
            {
                platform.Update();
            }
        }


        /// <summary>
        /// Проверяет, достиг ли игрок финишного флага и завершает уровень.
        /// </summary>
        /// <param name="player">Игрок, чьё положение проверяется.</param>
        public void CheckCompletion(Player player)
        {
            // Если уровень ещё не завершён и игрок касается финишного флага
            if (!IsLevelCompleted && player.GetBounds().IntersectsWith(FinishFlag))
            {
                // Устанавливаем флаг завершения уровня
                IsLevelCompleted = true;

                // Печатаем сообщение в отладку
                Debug.WriteLine("Уровень пройден!");
            }
        }

        /// <summary>
        /// Отрисовывает все элементы уровня.
        /// </summary>
        /// <param name="g">Поверхность для рисования.</param>
        public void Draw(Graphics g)
        {
            // Если есть текстура блока, отрисовываем платформы с ней
            if (_blockTexture != null)
            {
                foreach (var platform in Platforms)
                {
                    DrawTexturedPlatform(g, platform);
                }
            }
            // Иначе закрашиваем платформы зелёным цветом
            else
            {
                foreach (var platform in Platforms)
                {
                    g.FillRectangle(Brushes.Green, platform);
                }
            }
            // Рисуем финишный флаг: либо текстуру, либо красный прямоугольник
            if (_finishFlagTexture != null)
            {
                g.DrawImage(_finishFlagTexture, FinishFlag);
            }
            else
            {
                g.FillRectangle(Brushes.Red, FinishFlag);
                g.DrawRectangle(Pens.DarkRed, FinishFlag);
            }
            // Отрисовываем ловушки в виде треугольников (шипов)
            foreach (var trap in Traps)
            {
                // Вычисляем количество шипов
                int spikeCount = trap.Width / 20;
                int spikeWidth = trap.Width / spikeCount;
                using (var spikeBrush = new SolidBrush(Color.DarkRed))
                {
                    for (int i = 0; i < spikeCount; i++)
                    {
                        // Создаём координаты треугольного шипа
                        Point[] spike = {
                            new Point(trap.X + i * spikeWidth, trap.Bottom),
                            new Point(trap.X + (i + 1) * spikeWidth, trap.Bottom),
                            new Point(trap.X + i * spikeWidth + spikeWidth / 2, trap.Top)
                        };
                        // Рисуем шип
                        g.FillPolygon(spikeBrush, spike);
                    }
                }
            }
            // Отрисовываем движущиеся платформы
            foreach (var platform in MovingPlatforms)
            {
                platform.Draw(g);
            }
            // Отрисовываем обычных врагов
            foreach (var enemy in Enemies)
            {
                enemy.Draw(g);
            }
            // Отрисовываем врагов, привязанных к колоннам
            foreach (var enemy in ColumnEnemies)
            {
                enemy.Draw(g);
            }
            // Повторная отрисовка движущихся платформ (возможно, дублирование — можно удалить)
            foreach (var platform in MovingPlatforms)
            {
                platform.Draw(g);
            }
        }


        /// <summary>
        /// Проверяет, сталкивается ли игрок с врагами (обычными или колоннами) либо с ловушками.
        /// </summary>
        /// <param name="player">Игрок, для которого проверяется столкновение.</param>
        /// <returns>Возвращает true, если произошло столкновение; иначе false.</returns>
        public bool CheckPlayerCollision(Player player)
        {
            // Получаем границы игрока
            var playerBounds = player.GetBounds();
            // Проверяем столкновение с врагами-колоннами
            foreach (var enemy in ColumnEnemies)
            {
                // Если границы игрока пересекаются с зоной поражения колонны
                if (playerBounds.IntersectsWith(enemy.GetKillZone()))
                    return true;
            }
            // Проверяем столкновение с обычными врагами
            foreach (var enemy in Enemies)
            {
                // Если границы игрока пересекаются с границами врага
                if (playerBounds.IntersectsWith(enemy.Bounds))
                    return true;
            }
            // Проверяем столкновение с ловушками
            foreach (var trap in Traps)
            {
                // Определяем увеличенную зону поражения ловушки
                Rectangle trapKillZone = new Rectangle(
                    trap.X - 3,                // Немного расширяем зону влево
                    trap.Y - 7,                // Поднимаем зону вверх
                    trap.Width + 6,            // Расширяем по ширине
                    trap.Height + 10);         // Увеличиваем по высоте
                // Если границы игрока пересекаются с зоной поражения ловушки
                if (playerBounds.IntersectsWith(trapKillZone))
                    return true;
            }
            // Если ни с чем не столкнулись — возвращаем false
            return false;
        }


        /// <summary>
        /// Отрисовывает платформу с текстурой блоков.
        /// Если платформа шире текстуры, используется заливка с повторяющимся узором.
        /// </summary>
        /// <param name="g">Контекст графики, на котором производится отрисовка.</param>
        /// <param name="platform">Прямоугольник, представляющий платформу.</param>
        private void DrawTexturedPlatform(Graphics g, Rectangle platform)
        {
            // Если ширина платформы больше ширины текстуры
            if (platform.Width > _blockTexture.Width)
            {
                // Создаём кисть с текстурой и режимом повтора (tile)
                using (var brush = new TextureBrush(_blockTexture, WrapMode.Tile))
                {
                    // Сдвигаем текстуру на позицию платформы
                    brush.TranslateTransform(platform.X, platform.Y);
                    // Заполняем прямоугольник платформы текстурированной кистью
                    g.FillRectangle(brush, platform);
                }
            }
            else
            {
                // Если платформа уже или равна по ширине — рисуем обычную текстуру
                g.DrawImage(_blockTexture, platform);
            }
        }

        /// <summary>
        /// Сбрасывает состояние уровня: камера и флаг завершения.
        /// </summary>
        public void Reset()
        {
            // Обнуляем смещение камеры
            CameraOffset = 0;
            // Устанавливаем флаг завершения уровня в false
            IsLevelCompleted = false;
        }
    }
}