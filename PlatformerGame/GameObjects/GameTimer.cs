using System;
using System.Windows.Forms;

namespace PlatformerGame.GameObjects
{
    /// <summary>
    /// Класс игрового таймера, обеспечивающий вызов обновления игры с фиксированным интервалом
    /// </summary>
    public class GameTimer
    {
        // Базовый таймер Windows Forms
        private readonly Timer timer;

        /// <summary>
        /// Событие, вызываемое каждый интервал таймера
        /// </summary>
        public event Action Update;

        /// <summary>
        /// Создает новый экземпляр игрового таймера
        /// </summary>
        /// <param name="interval">Интервал между обновлениями в миллисекундах</param>
        public GameTimer(int interval)
        {
            // Инициализация таймера с указанным интервалом
            timer = new Timer { Interval = interval };

            // Подписка на событие Tick с вызовом события Update
            timer.Tick += (s, e) => Update?.Invoke();
        }

        /// <summary>
        /// Запускает таймер
        /// </summary>
        public void Start() => timer.Start();

        /// <summary>
        /// Останавливает таймер
        /// </summary>
        public void Stop() => timer.Stop();
    }
}