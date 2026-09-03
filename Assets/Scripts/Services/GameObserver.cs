using System;

namespace Assets.Scripts.Services
{
    public class GameObserver
    {
        public bool IsPause { get; private set; }
        public Action OnPauseChange { get; set; }

        public void Cleanup()
        {
            OnPauseChange = null;
            IsPause = false;
        }
        public void SendChangePause(bool isPause)
        {
            IsPause = isPause;
            OnPauseChange?.Invoke();
        }
    }
}