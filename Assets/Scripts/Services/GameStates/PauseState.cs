namespace Assets.Scripts.Services.GameStates
{
    public class PauseState
    {
        private readonly GameObserver _gameObserver;

        public PauseState(GameObserver gameObserver)
        {
            _gameObserver = gameObserver;
        }

        public void Run(bool isPause)
        {
            _gameObserver.SendChangePause(isPause);
        }
    }
}