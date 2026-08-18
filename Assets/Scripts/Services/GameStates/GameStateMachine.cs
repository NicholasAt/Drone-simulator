using Assets.Scripts.Infrastructure.EntryPoints;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace Assets.Scripts.Services.GameStates
{
    public class GameStateMachine
    {
        private readonly MainMenuEntryPoint.Preparation _mainMenuEntryPoint;

        public GameStateMachine(MainMenuEntryPoint.Preparation mainMenuEntryPoint)
        {
            _mainMenuEntryPoint = mainMenuEntryPoint;
        }
        public async UniTask LoadMainMenu()
        {
           await _mainMenuEntryPoint.Run();
        }
    }
}