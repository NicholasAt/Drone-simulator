using Assets.Scripts.Infrastructure.EntryPoints;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.GameStates
{
    public class GameStateMachine
    {
        private readonly MainMenuEntryPoint.Preparation _mainMenuEntryPoint;
        private readonly Location1EntryPoint.Preparation _locationEntryPoint;

        public GameStateMachine(MainMenuEntryPoint.Preparation mainMenuEntryPoint, Location1EntryPoint.Preparation gameEntryPoint)
        {
            _mainMenuEntryPoint = mainMenuEntryPoint;
            _locationEntryPoint = gameEntryPoint;
        }

        public async UniTask LoadMainMenu()
        {
            await _mainMenuEntryPoint.Run();
        }

        public async UniTask LoadLocation1()
        {
            await _locationEntryPoint.Run();
        }
    }
}