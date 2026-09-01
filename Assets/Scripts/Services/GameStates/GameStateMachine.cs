using Assets.Scripts.Infrastructure.EntryPoints;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.GameStates
{
    public class GameStateMachine
    {
        private readonly MainMenuEntryPoint.Preparation _mainMenuEntryPoint;
        private readonly Location1EntryPoint.Preparation _locationEntryPoint;
        private readonly PauseState _pauseState;

        public GameStateMachine(MainMenuEntryPoint.Preparation mainMenuEntryPoint, Location1EntryPoint.Preparation gameEntryPoint,PauseState pauseState)
        {
            _mainMenuEntryPoint = mainMenuEntryPoint;
            _locationEntryPoint = gameEntryPoint;
            _pauseState = pauseState;
        }

        public async UniTask LoadMainMenu()
        {
            await _mainMenuEntryPoint.Run();
        }

        public async UniTask LoadLocation1()
        {
            await _locationEntryPoint.Run();
        }
        public void Pause(bool isPause)
        {
            _pauseState.Run(isPause);
        }
    }
}