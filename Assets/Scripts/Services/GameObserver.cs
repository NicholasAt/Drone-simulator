using System;

namespace Assets.Scripts.Services
{
    public class GameObserver
    {
        public bool IsPause { get; private set; }
        public Action OnPauseChange { get; set; }

        public void SendChangePause(bool isPause, bool send = true)
        {
            IsPause = isPause;
            if (send)
                OnPauseChange?.Invoke();
        }
    }
}