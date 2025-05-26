using System.Collections.Generic;
using System.Drawing;
using static PlatformerGame.GameObjects.LevelManager;

namespace PlatformerGame.GameObjects
{
    /// <summary>
    /// Представляет данные, описывающие уровень игры.
    /// Содержит информацию о параметрах генерации, врагах, ловушках и метаданных уровня.
    /// </summary>
    public class LevelData
    {
        /// <summary>
        /// Номер уровня в последовательности.
        /// </summary>
        public int LevelNumber { get; set; }

        /// <summary>
        /// Название уровня, отображаемое в меню.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Краткое описание уровня для отображения игроку.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Признак того, заблокирован ли уровень (true — заблокирован, false — доступен).
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Путь к изображению предпросмотра уровня.
        /// Используется для отображения превью на экране выбора уровней.
        /// </summary>
        public string PreviewImagePath { get; set; }

        /// <summary>
        /// Длина уровня в условных единицах (например, ширина игрового мира).
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Количество платформ, которое необходимо сгенерировать на уровне.
        /// </summary>
        public int PlatformCount { get; set; }

        /// <summary>
        /// Уровень сложности (может влиять на количество врагов, ловушек и другие параметры).
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// Список прямоугольников, представляющих ловушки на уровне.
        /// </summary>
        public List<Rectangle> Traps { get; set; } = new List<Rectangle>();

        /// <summary>
        /// Список врагов, размещённых на уровне.
        /// </summary>
        public List<Enemy> Enemies { get; set; } = new List<Enemy>();

        /// <summary>
        /// Дополнительная информация о врагах, например, их тип, поведение, параметры.
        /// Используется для настройки и генерации врагов на уровне.
        /// </summary>
        public List<EnemyInfo> EnemyInfos { get; set; } = new List<EnemyInfo>();
    }
}
