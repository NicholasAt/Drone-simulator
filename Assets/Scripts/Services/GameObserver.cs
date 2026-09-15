using System;

namespace Assets.Scripts.Services
{
    public class GameObserver
    {
        public bool IsPause { get; private set; }
        public Action OnPauseChange { get; set; }
        public Action OnOutsideMap { get; set; }
        public float CharacterSpeed {  get; private set; }

        public void Cleanup()
        {
            OnPauseChange = null;
            IsPause = false;
            CharacterSpeed = 0;
        }

        public void SendChangePause(bool isPause)
        {
            IsPause = isPause;
            OnPauseChange?.Invoke();
        }
        public void SetCharacterSpeed(float speed)
        {
            CharacterSpeed = speed;
        }

        public void SendOutside()
        {
            OnOutsideMap?.Invoke();
        }
    }
}