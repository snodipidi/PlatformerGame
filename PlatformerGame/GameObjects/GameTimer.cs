using System;
using System.Windows.Forms;

namespace PlatformerGame.GameObjects
{
    public class GameTimer
    {
        private readonly Timer timer;
        public event Action Update;

        public GameTimer(int interval)
        {
            timer = new Timer { Interval = interval };
            timer.Tick += (s, e) => Update?.Invoke();
        }

        public void Start() => timer.Start();
        public void Stop() => timer.Stop();
    }
}